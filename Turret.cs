using UnityEngine;
using System.Collections;

public class Turret : GenericEnemy {
	Transform body;
	Transform legUp;
	Transform legUnder;
	Transform legUpBack;
	Transform legUnderBack;

	Transform legUpR;
	Transform legUnderR;

	float angle = 0f;
	float angleRange = 29f;
	float idlePanSpeed = 15f;
	float idleDirection = 1f;

	// Don't move the thing too smoothly, so have a timer refresh to give it pulses of speed
	float maxRefreshMovementTimer = .5f;
	float refreshMovementTimer = 0f;

	PossessableScript ps;
	bool destroyed = false;
	bool startedAttack = false;
	float delayAttackTimer = 0f;
	Animator m_Anim;

	Transform cursor;

	GameObject gunPivot;
	GameObject gunTip;
	Vector3 gunPivotPos;
	float closestCollisionDistance = 10000f;
	Transform bulletTrail;
	// Trail showing where the bullet went
	GameObject bulletTrailPath;
	float bulletTrailTimer = 0;
	float gunDamageVal = 6f;

	Transform targetObject;
	GameObject hand;

	Transform particles;

	GameObject FastBullet;


	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 					NOTE: IF YOU WANT TO FLIP THE TURRET ON ITS X AXIS, FLIP THE SUBCOMPONENTS (Body, etc)
	///												 --NOT-- THE MAIN OBJECT ITSELF
	/// 					BUT if you're doing rotation, rotate the main object
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



	// Use this for initialization
	void Start () {
		body = transform.Find("Body");
		legUp = body.Find("LegUp");
		legUnder = transform.Find("LegUnder");
		legUpBack = body.Find("LegUpBack");
		legUnderBack = transform.Find("LegUnderBack");
		legUpR = transform.Find("LegUpR");
		legUnderR = transform.Find("LegUnderR");

		ps = body.GetComponent<PossessableScript>();
		m_Anim = body.GetComponent<Animator>();
		cursor = GameObject.Find("cursor").transform;

		gunPivot = body.Find("GunPivot").gameObject;
		gunTip = gunPivot.transform.Find("BulletOrigin").gameObject;
		//gunPivot.SetActive(false);
		gunPivotPos = gunPivot.transform.localPosition;

		bulletTrailPath = gunPivot.transform.Find("BulletTrailOrigin").Find("BulletTrail").gameObject;
		bulletTrailPath.SetActive(false);

		hand = GameObject.Find("TheHand");
		targetObject = hand.transform;

		particles = transform.Find("Particles");

		FastBullet = (GameObject)Resources.Load("FastBullet");

		// If we're flipped on the X axis, rotate the gun pivot to ensure the ** RIGHT VECTOR ** faces the correct way
		if (body.transform.localScale.x < 0){
			gunPivot.transform.Rotate(0f, 180f, 0);
		}
	}

	// Rotates the body idly looking for the player
	void IdleRotation(){
		// Moves in slight pulses
		refreshMovementTimer += Time.deltaTime;
		if (refreshMovementTimer >= maxRefreshMovementTimer){
			refreshMovementTimer = 0f;
		}
		angle += Mathf.Lerp(idlePanSpeed, 0f, refreshMovementTimer/maxRefreshMovementTimer) * Time.deltaTime * idleDirection;
		if (angle > angleRange){
			angle = angleRange;
			idleDirection = -1f;
		} else if (angle <= -angleRange){
			angle = -angleRange;
			idleDirection = 1f;
		}

		Vector3 tempAngle = body.localEulerAngles;
		tempAngle.z = angle;
		body.transform.localEulerAngles = tempAngle;

	}

	// Adjusts the positioning of the feet based on the body
	void AdjustFeetPosition(){
		Vector3 tempUnderAngle = legUnder.transform.localEulerAngles;
		tempUnderAngle.z = angle/2f;
		legUnder.transform.localEulerAngles = tempUnderAngle;
		legUnderBack.transform.localEulerAngles = tempUnderAngle;

		legUnderR.transform.localEulerAngles = tempUnderAngle;

		Vector3 tempUpAngle = legUnder.transform.localEulerAngles;
		tempUpAngle.z = -angle/2f;
		legUp.transform.localEulerAngles = tempUpAngle;
		legUpBack.transform.localEulerAngles = tempUpAngle;

		tempUpAngle.z *= 1.5f;
		legUpR.transform.localEulerAngles = tempUpAngle;
	}


	// Activates the attack
	public override void ActivateAttack(){
		Time.timeScale = 1f;
		if (!destroyed && delayAttackTimer <= 0){
			delayAttackTimer = .175f;

			/*
			// Gets all hits going out from the gun tip
			float distance = 100f;
			Vector3 direction = gunTip.transform.forward;
			direction.x *= body.transform.localScale.x;
			RaycastHit2D[] hits = Physics2D.RaycastAll(gunTip.transform.position, direction, distance);
			float closestCollisionDistance = distance;
			GameObject closestCollision = null;
			bool blocked = false;
			print(hits.Length);
			for (int i = 0; i < hits.Length; i++){
				print(hits[i].collider.gameObject);
			}
			print("\n");
			// If possessed, shots will hit the nearest object
			bool possessed = ps.possessed;
			// Inital pass to test for wall collisions
			foreach (RaycastHit2D r in hits){
				if (!r.collider.isTrigger || (possessed && r.collider.isTrigger)){
					if (r.transform != transform){
						// See what wall we hit, and reset the grab reach based on that
						float dist = Vector3.Distance(r.point, gunTip.transform.position);
						// Shot blocked by a wall
						if (dist < closestCollisionDistance){
							// Collisions only register if:
							// 1: The object hit is solid and NOT a trigger
							// OR
							// 2: The object hit IS a trigger, but is the target
							if (!r.collider.isTrigger){
								blocked = true;
								closestCollisionDistance = dist;
								closestCollision = r.collider.gameObject;
							} else if (r.collider.gameObject == targetObject && !possessed){
								closestCollisionDistance = dist;
								closestCollision = r.collider.gameObject;
							} else if (possessed && r.collider.GetComponent<GenericEnemy>()){
								closestCollisionDistance = dist;
								closestCollision = r.collider.gameObject;
							}
						}					
					} 
				}
			}
			
			//audio.Play();
			
			// Register the hit
			if (closestCollision != null && closestCollision.transform.parent){
				closestCollision = closestCollision.transform.parent.gameObject;
			}
			if (closestCollision != null && closestCollision.GetComponent<GenericEnemy>()){
				closestCollision.GetComponent<GenericEnemy>().DelayedDamage(gunDamageVal);
			// If the nearest hit is a wall and not a target, and the turret isn't possessed, ignore
			} else if (ps.possessed == false){
				print("Won't fire. Closest collision is "+closestCollision);
				return;
			}

			*/
			GameObject tempBullet = (GameObject)Instantiate(FastBullet);
			tempBullet.transform.position = gunPivot.transform.position;
			tempBullet.GetComponent<FastBullet>().Fire(gunPivot.transform.right);
			tempBullet.GetComponent<FastBullet>().spawn = gameObject;

			Camera.main.GetComponent<CameraTarget>().AddShake(.05f);

			//ps.DisableDepossession();
			startedAttack = true;
			//ps.depossessable = false;
			m_Anim.SetBool("Attacking", true);

			// Enable the bullet trail
			bulletTrailPath.SetActive(true);
			bulletTrailTimer = Time.fixedDeltaTime*4f;
			Vector3 tempScale = bulletTrailPath.transform.localScale;
			tempScale.y = closestCollisionDistance;
			bulletTrailPath.transform.localScale = tempScale;
			particles.GetComponent<ParticleSystem>().Emit(3);
		}
	}
	
	void Attack(){
		//m_Anim.SetBool("Destroyed", true);
	}
	
	public void FinishAttack(){
		//if (!destroyed){
		startedAttack = false;
		Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
		//destroyed = true;
		m_Anim.SetBool("Attacking", false);
		ps.EnableDepossession();
	}

	void PlayerControls(){
		float angleToCursor = Mathf.Atan2(cursor.position.y - transform.position.y, cursor.position.x - transform.position.x)*Mathf.Rad2Deg;// - transform.eulerAngles.z;
		// Does angle corrections for a body flipped along X
		if (body.localScale.x <= 0f){
			if (angleToCursor < 0f){
				angleToCursor += 180f;
			} else {
				angleToCursor -= 180f;
			}

			if (angleToCursor > angleRange){
				angleToCursor = angleRange;
			} else if (angleToCursor < -angleRange){
				angleToCursor = -angleRange;
			}
			angle = angleToCursor;
		} else {
			if (angleToCursor > angleRange){
				angleToCursor = angleRange;
			} else if (angleToCursor < -angleRange){
				angleToCursor = -angleRange;
			}
			angle = angleToCursor;
		}


		Vector3 tempAngle = body.transform.localEulerAngles;
		tempAngle.z = angle;
		body.transform.localEulerAngles = tempAngle;

		if (Input.GetMouseButtonDown(0) && ps.attackMenu.activeSelf == false){
			ActivateAttack();
		}
		delayAttackTimer -= Time.deltaTime;
	}
	void OnTriggerEnter2D(Collider2D col){

	}
	void OnTriggerExit2D(Collider2D col){

	}

	// Aims at player or other enemies
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && col.gameObject.GetComponent<GenericEnemy>().alliedSide != GetComponent<GenericEnemy>().alliedSide){
			float yOffset = -.5f;
			try{
				float angleToCursor = Mathf.Atan2(col.bounds.center.y - body.transform.position.y + yOffset, col.bounds.center.x - body.transform.position.x)*Mathf.Rad2Deg;
				// Does angle corrections for a body flipped along X
				if (body == null) return;
				if (body.localScale.x <= 0f){
					if (angleToCursor < 0f){
						angleToCursor += 180f;
					} else {
						angleToCursor -= 180f;
					}

					if (angleToCursor > angleRange){
						angleToCursor = angleRange;
					} else if (angleToCursor < -angleRange){
						angleToCursor = -angleRange;
					}
					angle = angleToCursor;
				} else {
					if (angleToCursor > angleRange){
						angleToCursor = angleRange;
					} else if (angleToCursor < -angleRange){
						angleToCursor = -angleRange;
					}
					angle = angleToCursor;
				}


				Vector3 tempAngle = body.transform.localEulerAngles;
				tempAngle.z = angle;
				body.transform.localEulerAngles = tempAngle;

				if (ClearPathToTarget(col.transform, transform, 20f)){
					ActivateAttack();
				}

				delayAttackTimer -= Time.deltaTime * .25f;
			}catch{
				Debug.Log("Turret error");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		delayAttackTimer -= Time.deltaTime;
		// Makes sure the bullet trail is off
		if (bulletTrailTimer > 0f){
			bulletTrailTimer -= Time.deltaTime;
			if (bulletTrailTimer <= 0f){
				bulletTrailPath.SetActive(false);
			}
		}

		if (ps.possessed){
			PlayerControls();
		} else {
			IdleRotation();
		}
		AdjustFeetPosition();

		if (life <= 0f){
			Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
			GameObject dustCloud = (GameObject)Resources.Load("Dustcloud");
			GameObject d = (GameObject)Instantiate(dustCloud);
			d.transform.position = transform.position;
			//ps.EnableDepossession();
			//ps.Depossess();
			//GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		}
	}
}
