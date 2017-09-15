using UnityEngine;
using System.Collections;

public class BulletBossHandManager : MonoBehaviour {
	public Vector3 startPos;
	Vector3 parentPos;
	Vector3 preAttackPos; 				// A reference position to be updated at the beginning of each attack
	const int idling = 1;
	const int rotating = 2;
	const int clapping = 3;
	const int bothRotating = 4; 		// Both hands do a rotate/shooting attack
	const int fireRaising = 5; 			// Fire at the bottom of the map gets larger and more dangerous
	int state = 1;
	public bool handPositionFixed = true;
	float idleRotationDist = .075f;
	float rotationAngle = 0f;
	float outwardSpeed = 2f;
	float idleRotationalVelocity = 180f*Mathf.Deg2Rad;

	bool readyToRotateAttack = false;
	float attackRotationXDist = .75f;
	float attackRotationYDist = .5f;
	float attackRotationDist = 3f;
	float attackRotationalVelocity = 50f*Mathf.Deg2Rad;
	float maxAttackRotationalVelocity = 80f*Mathf.Deg2Rad;
	float firingRotationalVelocity = 14f*Mathf.Deg2Rad;
	float firingTurnDirection = 1f; 	// Direction to turn when firing a barrage of shots
	float attackHandTurnSpeed = 2f;
	float slowHandTurnSpeed = .4f;

	public float direction = -1f;

	Transform hand;

	Animator m_anim;

	// Number of shots we'll fire in a row
	int shotReps = 0;
	int maxShotReps = 8;
	float timeBetweenReps = 0f;
	float maxTimeBetweenReps = .40f;

	// Number of sets of shots to fire
	int shotSets = 0;
	int maxShotSets = 3;
	float timeBetweenSets = 0f;
	float maxTimeBetweenSets = 1.75f;

	// Time to wait after we fully take aim
	float postAimWaitTimer = 0f;
	float maxPostAimWaitTimer = .6f;

	GameObject bullet;
	float shootTimer = 0f;
	float maxShootTimer = 1.5f;


	// Hands swap control back and forth; use this to determine which is attacking
	public bool currentlyAttacking = false;
	// If in control, we'll grab the other hand and set whether it should attack and then attack only when the other isn't
	public bool inControl = false;

	// Inverts rotation
	public bool invertRotation = false;

	BulletBossHandManager otherHand;

	// Velocity for clapping
	Vector3 velocity = new Vector3(0f, 0f, 0f);
	float clapDamage = 2f;
	float clapPullBackSpeed = -10f;		// Does a brief pull back before clapping
	float clapMaxSpeed = 23f;
	float clapSpeedUpTime = .6f;		// After starting a clap, this is how long it takes to reach full speed
	bool handsCollided = false;

	// Number of claps/punches we'll have in a row
	public int punchReps = 0;
	int maxPunchReps = 4;
	public float timeBetweenPunches = 0f;
	float maxTimeBetweenPunches = 1.3f;
	// Time to wait after we fully take aim
	float prePunchWaitTimer = 0f;
	float maxPrePunchWaitTimer = 2.5f;

	// For time between the attacks
	int[] AttackCycle = new int[]{rotating, fireRaising, clapping, bothRotating, clapping, fireRaising, clapping, bothRotating, clapping, fireRaising, clapping};
	int attackIndex = 0;
	int tankBeginning = 5; // When the attack index reaches this, activate the tanks
	float idleTimer = 0f;
	float maxIdleTimer = 2.75f;
	float tankIdleTimer = 10f; 	// How long to idle when tanks are on screen
	public bool tanksActive = false;

	// Displays a shadow around the hands
	Transform shadow1;
	Vector3 lastPosition;
	Transform shadow2;
	Vector3 lastLastPosition;
	float updateLastPositionTimer = 0f;
	float maxUpdateLastPositionTimer = .05f;
	Vector3 lastRotation;
	Vector3 lastLastRotation;

	// Handles all things pertaining to flame attack management
	float preRaiseFlameTimer = 0f;
	float maxPreRaiseFlameTimer = 2f;
	float flameRiseTimer = 0f;
	float maxFlameRiseTimer = 2.25f;
	float flameSpeed = 2.25f;
	Color flameAltColor = new Color(255f/255f, 94f/255f, 244f/255f, 1f);
	public ArrayList flames = new ArrayList();
	ArrayList flameDefaultScales = new ArrayList();
	Transform flameTip;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		parentPos = transform.Find("Center").position;

		hand = GameObject.Find("TheHand").transform;

		m_anim = GetComponent<Animator>();

		bullet = (GameObject)Resources.Load("MouthBullet");

		// If in control, we grab the back hand; if not, grab the front
		if (inControl){
			otherHand = transform.parent.Find("BackHand").GetComponent<BulletBossHandManager>();
		} else {
			otherHand = transform.parent.Find("FrontHand").GetComponent<BulletBossHandManager>();
		}

		if (inControl){
			//StartRotating();
		}

		shadow1 = transform.Find("Shadow1");
		shadow1.SetParent(transform.parent);
		lastPosition = transform.position;
		shadow2 = transform.Find("Shadow2");
		shadow2.SetParent(transform.parent);
		lastLastPosition = transform.position;
		lastRotation = transform.eulerAngles;
		lastLastRotation = transform.eulerAngles;

		// Gets the flames
		foreach (Transform t in transform.parent){
			if (t.name.Contains("Flame")){
				t.GetComponent<Animator>().speed *= Random.Range(.9f, 1.1f);
				flames.Add(t);
				flameDefaultScales.Add(t.localScale);
				t.SetParent(null);
			}
		}
		// Finds the spot we'll move the hands to when bringing up the flames
		flameTip = GameObject.Find("FlameTip").transform;
	}
	
	// Update is called once per frame
	void Update () {
		Idle();
		Rotating();
		Clapping();
		RaiseFire();

		if (state == rotating && currentlyAttacking){
			m_anim.SetBool("Attacking", true);
		} else if (state == clapping){
			m_anim.SetBool("Attacking", true);
		} else if (state == fireRaising && inControl){
			m_anim.SetBool("Attacking", true);
		} else {
			m_anim.SetBool("Attacking", false);
		}

		// Has phantoms follow the hands
		updateLastPositionTimer += Time.deltaTime;
		if (updateLastPositionTimer >= maxUpdateLastPositionTimer){
			updateLastPositionTimer = 0f;
			lastLastPosition = lastPosition;
			lastPosition = transform.position;
			lastLastRotation = lastRotation;
			lastRotation = transform.eulerAngles;

			shadow1.transform.position = lastPosition;
			shadow1.transform.eulerAngles = lastRotation;
			shadow1.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
			shadow2.transform.position = lastLastPosition;
			shadow2.transform.eulerAngles = lastLastRotation;
			shadow2.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
		}

		// Disables flames if the parent is dying
		/*
		if (transform.parent.GetComponent<BulletBoss>().startDying){
			foreach (Transform t in flames){
				t.gameObject.SetActive(false);
			}
		}
		*/
	}

	// Hand rotates and fires bullets
	void Rotating(){
		if (state == rotating || state == bothRotating){
			if (!readyToRotateAttack){
				transform.position = Vector3.MoveTowards(transform.position, parentPos, outwardSpeed * Time.deltaTime);
				// Within range of the start and ready to rotate
				// Position has been fixed
				if (Vector3.Distance(transform.position, parentPos) <= attackRotationDist){
					readyToRotateAttack = true;
					rotationAngle = Mathf.Atan2(transform.position.y - parentPos.y, transform.position.x - parentPos.x);
				}
			} else if (currentlyAttacking) {
				// Rotates to shoot bullets
				//Vector3 tempRot = Vector3.RotateTowards(transform.right, (hand.position - transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
				//tempRot.z = 0f;
				//transform.right = tempRot.normalized;

				shootTimer += Time.deltaTime;
				if (shootTimer >= maxShootTimer){
					//shootTimer = 0f;
					//GameObject tempBullet = Instantiate(bullet);
					//tempBullet.transform.position = transform.position;
					//tempBullet.GetComponent<MouthBullet>().SetAngle(transform.eulerAngles.z);
				}

				// Turns somewhat quickly to face the player
				if (timeBetweenSets < maxTimeBetweenSets){
					// Rotates around center point quickly
					rotationAngle += attackRotationalVelocity * Time.deltaTime * direction;
					transform.position = parentPos + new Vector3(Mathf.Cos(rotationAngle)*attackRotationXDist, Mathf.Sin(rotationAngle)*attackRotationYDist, 0f);

					timeBetweenSets += Time.deltaTime;
					// Turns to face player
					if (invertRotation){
						Vector3 tempRot = Vector3.RotateTowards(transform.right, (-hand.position + transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
						tempRot.z = 0f;
						transform.right = tempRot.normalized;
					} else {
						Vector3 tempRot = Vector3.RotateTowards(transform.right, (hand.position - transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
						tempRot.z = 0f;
						transform.right = tempRot.normalized;
					}

				} else {
					// Rotates around centerpoint slowly while attacking
					rotationAngle += firingRotationalVelocity * Time.deltaTime * direction;
					transform.position = parentPos + new Vector3(Mathf.Cos(rotationAngle)*attackRotationXDist, Mathf.Sin(rotationAngle)*attackRotationYDist, 0f);

					// Holds for a brief pause before shooting
					postAimWaitTimer += Time.deltaTime;
					// Fires shots
					if (postAimWaitTimer >= maxPostAimWaitTimer){
						// Turns to face player
						//Vector3 tempRot = Vector3.RotateTowards(transform.right, (hand.position - transform.position).normalized, slowHandTurnSpeed*Time.deltaTime, slowHandTurnSpeed*Time.deltaTime);
						//tempRot.z = 0f;
						//transform.right = tempRot.normalized;
						Vector3 turnDirection = new Vector3(0f, 0f, slowHandTurnSpeed * Time.deltaTime * firingTurnDirection * Mathf.Rad2Deg);
						if (invertRotation){
							transform.Rotate(-turnDirection);
						} else {
							transform.Rotate(turnDirection);
						}

						// Waits a bit to fire
						timeBetweenReps += Time.deltaTime;
						// Fires a shot
						if (timeBetweenReps >= maxTimeBetweenReps){
							GameObject tempBullet = Instantiate(bullet);
							tempBullet.transform.position = transform.position;
							if (invertRotation){
								tempBullet.GetComponent<MouthBullet>().SetAngle(transform.eulerAngles.z + 180f);
							} else {
								tempBullet.GetComponent<MouthBullet>().SetAngle(transform.eulerAngles.z);
							}

							shotReps += 1;
							timeBetweenReps = 0f;
							// Finish a set of shots
							if (shotReps >= maxShotReps){
								shotSets += 1;
								shotReps = 0;
								timeBetweenSets = 0f;

								// Finish off all sets
								if (shotSets >= maxShotSets){
									postAimWaitTimer = 0f;
									
									// Starts idling again
									StartIdling();
									otherHand.StartIdling();
								} else if (state != bothRotating) {
									// Switches to have the other hand attack
									SwapAttack();
								}
							}
						}
					// Sets the direction to turn when firing a barrage
					} else { 
						Vector3 tempRot = Vector3.RotateTowards(transform.right, (hand.position - transform.position).normalized, slowHandTurnSpeed*Time.deltaTime, slowHandTurnSpeed*Time.deltaTime);
						tempRot.z = 0f;

						float angleDiff = Mathf.Atan2(tempRot.y - transform.right.y, tempRot.x - transform.right.x);
						if (angleDiff > 0f){
							firingTurnDirection = 1f;
						} else {
							firingTurnDirection = -1f;
						}
					}
				}
			// We still want hands to rotate even if not attacking
			} else if (!currentlyAttacking){
				// Rotates around center point quickly
				rotationAngle += attackRotationalVelocity * Time.deltaTime * direction;
				transform.position = parentPos + new Vector3(Mathf.Cos(rotationAngle)*attackRotationXDist, Mathf.Sin(rotationAngle)*attackRotationYDist, 0f);

				// Turns to face player
				if (invertRotation){
					Vector3 tempRot = Vector3.RotateTowards(transform.right, (-hand.position + transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
					tempRot.z = 0f;
					transform.right = tempRot.normalized;
				} else {
					Vector3 tempRot = Vector3.RotateTowards(transform.right, (hand.position - transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
					tempRot.z = 0f;
					transform.right = tempRot.normalized;
				}
			}
		}
	}

	// Idles between attacks and selects the next attack
	void Idle(){
		if (state == idling){
			// Selects the next attack
			idleTimer += Time.deltaTime;
			float maxTimer = maxIdleTimer;
			if (tanksActive){
				maxTimer = tankIdleTimer;
			}
			if (idleTimer >= maxIdleTimer){
				idleTimer = 0f;
				attackIndex += 1;
				if (attackIndex >= AttackCycle.Length){
					attackIndex = 0;
				}
				// Activates the tanks
				if (attackIndex >= tankBeginning){
					tanksActive = true;
					otherHand.tanksActive = true;
					transform.parent.GetComponent<BulletBoss>().ActivateTanks();
				}
				int tempState = AttackCycle[(int)attackIndex];
				if (tempState == clapping){
					StartClap();
					otherHand.StartClap();
				} else if (tempState == rotating){
					StartRotating();
				} else if (tempState == bothRotating){
					StartRotating();
					otherHand.StartRotating();
					//BothStartRotating();
				} else if (tempState == fireRaising){
						StartFire();
						otherHand.StartFire();
						//BothStartRotating();
				}
			}
			// Keeps the hand facing the normal direction when idle
			Vector3 tempRot = Vector3.RotateTowards(transform.right, Vector3.right, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
			tempRot.z = 0f;
			transform.right = tempRot.normalized;

			// Far from the start position and needs to move back
			if (!handPositionFixed){
				transform.position = Vector3.MoveTowards(transform.position, startPos, outwardSpeed * Time.deltaTime);
				// Within range of the start and ready to rotate
				// Position has been fixed
				if (Vector3.Distance(transform.position, startPos) <= idleRotationDist){
					handPositionFixed = true;
					rotationAngle = Mathf.Atan2(transform.position.y - startPos.y, transform.position.x - startPos.x);
				}
			} else {
				rotationAngle += idleRotationalVelocity * Time.deltaTime * direction;
				transform.position = startPos + new Vector3(Mathf.Cos(rotationAngle)*idleRotationDist, Mathf.Sin(rotationAngle)*idleRotationDist, 0f);
			}
		}
	}

	// Claps hands to crush the player
	void Clapping(){
		if (state != clapping){
			return;
		}
		// Hand rotates to face the other hand
		// Have the up vector point away from the other
		if (inControl){
			Vector3 tempRot = Vector3.RotateTowards(transform.up, -(otherHand.transform.position - transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
			tempRot.z = 0f;
			transform.up = tempRot.normalized;
		} else {
			Vector3 tempRot = Vector3.RotateTowards(transform.up, (otherHand.transform.position - transform.position).normalized, attackHandTurnSpeed*Time.deltaTime, attackHandTurnSpeed*Time.deltaTime);
			tempRot.z = 0f;
			transform.up = tempRot.normalized;
		}

		prePunchWaitTimer += Time.deltaTime;
		if (prePunchWaitTimer >= maxPrePunchWaitTimer){
			timeBetweenPunches += Time.deltaTime;
			// Slams together
			if (timeBetweenPunches >= maxTimeBetweenPunches){
				velocity = otherHand.transform.position - transform.position;
				float tempClapSpeed = Mathf.Lerp(clapPullBackSpeed, clapMaxSpeed, (timeBetweenPunches - maxTimeBetweenPunches) / clapSpeedUpTime);
				velocity = velocity.normalized * tempClapSpeed * Time.deltaTime;
				transform.position += velocity;

				// Hands pull back
				if (handsCollided){
					timeBetweenPunches = 0f;
					punchReps += 1;
					otherHand.timeBetweenPunches = 0f;
					otherHand.punchReps += 1;
					if (inControl && punchReps >= maxPunchReps){
						punchReps = 0;
						otherHand.punchReps = 0;
						StartIdling();
						otherHand.StartIdling();
					}
				}
				handsCollided = false;
			}
			// Pulls apart
			else {
				float offset = 3f;
				if (!inControl){
					offset = -offset;
				}
				// Moves the hand out
				Vector3 destPos = new Vector3(startPos.x + offset, transform.position.y, transform.position.z);
				transform.position = Vector3.MoveTowards(transform.position, destPos, 5f * Time.deltaTime);
				// Moves the hands towards The Hand
				destPos = new Vector3(transform.position.x, hand.position.y, transform.position.z);
				transform.position = Vector3.MoveTowards(transform.position, destPos, 1.5f * Time.deltaTime);
			}
		} else {
			float offset = 3f;
			if (!inControl){
				offset = -offset;
			}
			// Moves the hand out
			Vector3 destPos = new Vector3(startPos.x + offset, transform.position.y, transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, destPos, 5f * Time.deltaTime);
			// Moves the hands towards The Hand
			destPos = new Vector3(transform.position.x, hand.position.y, transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, destPos, 1.5f * Time.deltaTime);
		}
	}

	// Starts raising the level of the fire
	public void StartFire(){
		preAttackPos = transform.position;
		flameSpeed = 1f; 			// Set the flame speed to 1f so that it moves forward in time
		flameRiseTimer = 0f;
		preRaiseFlameTimer = 0f;
		state = fireRaising;
		otherHand.state = fireRaising;
	}

	// Fire rises from the ground
	public void RaiseFire(){
		if (state != fireRaising){
			return;
		}
		if (!inControl){
			return;
		}
		// Moves the dominant hand to wear the flame will rise
		preRaiseFlameTimer += Time.deltaTime;
		if (inControl){
			Vector3 tempFlameTip = flameTip.position;
			tempFlameTip.z = preAttackPos.z;
			transform.position = Vector3.Lerp(preAttackPos, tempFlameTip, preRaiseFlameTimer * 1.5f / maxPreRaiseFlameTimer);
		}
		if (preRaiseFlameTimer < maxPreRaiseFlameTimer){
			return;
		}
		// Ends attack
		if (flameRiseTimer < 0f){
			flameRiseTimer = 0f;
			preRaiseFlameTimer = 0f;
			StartIdling();
			otherHand.StartIdling();
		} else if (flameRiseTimer >= maxFlameRiseTimer){
			flameSpeed = -(Mathf.Abs(flameSpeed));
		}
		// Moves the flames up
		for (int i = 0; i < flames.Count; i++){
			//float x = ((Vector3)flameDefaultScales[i]).x;
			Vector3 tempScale = Vector3.Lerp((Vector3)flameDefaultScales[i], (Vector3)flameDefaultScales[i] * 5f, flameRiseTimer/maxFlameRiseTimer);
			//tempScale.x = x;
			((Transform)flames[i]).localScale = tempScale;
			((Transform)flames[i]).GetComponent<SpriteRenderer>().color = Vector4.Lerp(new Vector4(1f, 1f, 1f, 1f), flameAltColor, 3f*flameRiseTimer/maxFlameRiseTimer);
		}
		flameRiseTimer += flameSpeed * Time.deltaTime;
	}

	// Starts the clap attack
	public void StartClap(){
		preAttackPos = transform.position;
		punchReps = 0;
		otherHand.punchReps = 0;
		state = clapping;
	}

	// Swaps attack to the other hand
	public void SwapAttack(){
		preAttackPos = transform.position;
		currentlyAttacking = false;
		otherHand.StartRotating();
	}

	// Sets the hand to start rotating
	public void StartRotating(){
		preAttackPos = transform.position;
		readyToRotateAttack = false;
		state = rotating;
		currentlyAttacking = true;
		//m_anim.SetBool("Attacking", true);
	}

	// Sets both hands to start rotating
	public void BothStartRotating(){
		preAttackPos = transform.position;
		readyToRotateAttack = false;
		state = bothRotating;
		otherHand.state = bothRotating;
		currentlyAttacking = true;
		//m_anim.SetBool("Attacking", true);
	}

	// Sets the hand to just idle
	public void StartIdling(){
		handPositionFixed = false;
		state = idling;
		m_anim.SetBool("Attacking", false);

		// Dialogue with boss saying how it'll die
		if (attackIndex == (tankBeginning - 2)){
			if (!transform.parent.GetComponent<BulletBoss>().bossWeaknessDialogue){
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("firstbossweakness"));
				transform.parent.GetComponent<BulletBoss>().bossWeaknessDialogue = true;
			}
		}
	}

	// Handles hits to the player when clapping
	public void OnTriggerStay2D(Collider2D col){
		if (timeBetweenPunches > maxTimeBetweenPunches && (col.transform.gameObject.tag == "Player" || col.transform.gameObject.tag == "PlayerChild")){
			hand.GetComponent<HandController>().DelayedDamage(clapDamage);
		}
		// If it hits the other hand, pull back from the clap
		if (col.GetComponent<BulletBossHandManager>()){
			Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
			handsCollided = true;
		}
	}
}
