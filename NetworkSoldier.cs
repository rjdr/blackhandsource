using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSoldier : NetworkGenericEnemy {
	// For checking if an object is possessed
	NetworkPossessableScript ps;
	Animator m_Anim;
	private Rigidbody2D m_Rigidbody2D;

	float walkSpeed = 1.15f;
	float curiousSpeed = 2f;
	float chaseSpeed = 2.75f;
	float runSpeed = 4.15f;
	public float moveDirection = -1f;
	int facing = 1;

	float patrolTimer = 0f;						// How long the soldier patrols for
	float maxPatrolTimer = 320f;
	float patrolWaitTimer = 0f;					// How long the soldier waits between walk periods
	float maxPatrolWaitTimer = 180f;
	bool patrolling = true;

	float maxLookDist = 7f;
	float maxLookTimer = .125f;
	float lookTimer = 0f;
	float lookAngle = 40f * Mathf.Deg2Rad;		// Typical view range
	float gunAimAngle = 30f * Mathf.Deg2Rad; 	// Max range of the gun's aiming
	float gunAngleOffset = 8f; 					// Slight offset of gun's pointed direction for aesthetic purposes
	float fullLookAngle = 89f * Mathf.Deg2Rad; 	// Max view range when alerted
	float lastSawTimer = 100f;
	float maxTurnLastSawTimer = .55f; 			// How long the window is in which the human will turn around when the player goes behind it
	float maxAlertLastSawTimer = 6f; 			// How long the soldier's alerted/chasing after last seeing the player

	ArrayList allTargets = new ArrayList();
	public GameObject targetObject;
	GameObject player;
	//Transform playerTransform;
	LookScript look;
	bool sawPlayer = false;
	Transform eye;
	
	public Sprite gunPivotImg;
	public Sprite possessedGunPivotImg;
	public Sprite shotgunImage;
	public Sprite possessedShotgunImage;
	GameObject gunPivot;
	Vector3 gunPivotPos;
	GameObject gunTip;
	bool aiming = false;
	bool walkingBackwards = false;
	float minDangerDistance = 2.5f;				// Distance the soldier will try to maintain from the player
	float minRunDist = 4.5f;					// When the soldier is > this distance and is moving towards the player, he'll run
	float runTimer = 0f;

	// How long the soldier must wait between shots
	const float maxShotTimer = 2f;
	// How much the soldier has to wait to shoot upon first seeing the player
	const float postAimShotTimer = .5f;
	float shotTimer = postAimShotTimer;
	// change in gun rotation from having shot (backfire)
	float maxBackfireAngle = 5f;
	float backFireAngle = 0f;
	float reduceBackfireAngle = .5f;
	// Brief delay in which the soldier takes aim, freezes its aim point, then fires in that direction
	float aimSetTimer = 0f;
	float maxAimSetTimer = .455f;
	// The rotation vector of the gun before firing
	Vector3 preShotGunRotation = new Vector3(0f, 0f, 0f);

	int ammo = 15;								// How many rounds of ammunition the character has

	// This is for angles and dramatic scrolling (e.g., showing the train from the front)
	Vector3 rotationAngle;

	// Point for the enemy to walk to while chasing
	// Use this to move backwards when aiming up
	Vector3 chaseDest = new Vector3(0f, 0f, 0f);

	Object dustCloud;

	Transform bulletTrail;

	// Arm piece that shows under the body
	GameObject underArm;
	// Legs for animating while aiming
	GameObject legs;
	// Draws the body sprite
	GameObject bodyRenderer;
	// Trail showing where the bullet went
	GameObject bulletTrailPath;
	GameObject bulletTrailPath2;
	GameObject bulletTrailPath3;
	float bulletTrailTimer = 0f;

	// Gives the player a bit of a bob while walking & aiming
	Vector3 aimBob = new Vector3(0f, 0f, 0f);
	float aimBobAng = 0f;
	float aimBobDAng = .22f;

	// Damage value of the soldier's attacks
	float gunDamageVal = 5;
	float gunVolume = 8.25f;

	// Soldier has a shotgun which shoots in a spread shot
	public bool hasShotgun = false;

	// Displays before the enemy attacks
	Transform headSpark;

	GameObject bloodSpatter;

	// Gunshot sound
	AudioSource audio;

	// Doesn't patrol (stares at a wall or something
	public bool nonPatrollingEntity = false;

	// Carries a key (0 = no key; >0 = key number)
	public int carriedKey = 0;

	// Last n frames' position (check displacement to climb block)
	ArrayList positionQueue = new ArrayList();
	int numRetainedPositions = 5; 	// How many past positions we should retain
	float maxClimbTriggerDisplacement = .1f;	// If you move < this dist, climbing will be checked
	float maxClimbTimer = 1/60f;
	float climbTimer = 0f;
	bool climbing = false;
	Vector3 preClimbPosition = new Vector3(0f, 0f, 0f);
	Vector3 postClimbPosition = new Vector3(0f, 0f, 0f);
	float climbVelocity = 8f;
	bool climbMoving = false;
	bool finishingClimb = false;

	bool climbingLadder = false;

	// The character will walk through doors when patrolling normally
	public bool patrolThroughDoors = false;

	// Object that notes when the character turns around
	Transform turnAroundObject;

	// A bullet object
	GameObject solidBullet;

	bool finishedInit = false;

	// The side that the character was originally allied with
	// This changes briefly once the player kills an allied character
	public int originalAlliedSide = 0;

	// The player is in control of themselves
	public bool inControl = true;

	// Small delay between shots so we can't fire like crazy
	float shotDelayTimer = 0f;
	float maxShotDelayTimer = .2f;

	// If it's on the ground, we can jump
	float lastJumpTimer = 0f;
	bool onGround = false;
	bool jumping = false;
	float jumpVel = 9.25f;
	public Vector2 currentVel;

	// Actions to take when starting
	public override void OnStartLocalPlayer(){
		//GetComponent<MeshRenderer>().material.color = Color.red;
		// Enables arrow to show who the player is
		transform.Find("Marker").GetComponent<SpriteRenderer>().enabled = true;
		Camera.main.GetComponent<NetCameraTarget>().target = transform;
	}
	void Start(){
		Init();
	}

	// Climbs over an obstacle
	public override void ClimbBlock(){
		climbTimer += Time.fixedDeltaTime;
		if (climbTimer >= maxClimbTimer && !climbing){
			climbTimer = 0f;
			positionQueue.Add(transform.position);
			if (positionQueue.Count > numRetainedPositions){
				positionQueue.RemoveAt(0);
			}
			// Checks for a block to climb
			GameObject block = null;
			// Make sure we're moving
			if (moveDirection != 0){
				try{
					if (Vector3.Distance((Vector3)positionQueue[numRetainedPositions-1], (Vector3)positionQueue[0]) < maxClimbTriggerDisplacement){
						if (Vector3.Distance((Vector3)positionQueue[0], (Vector3)positionQueue[(numRetainedPositions-1)/2]) < maxClimbTriggerDisplacement){
							// Find if there's a block to climb over
							block = CanClimbBlock();
							// Set the player to climb and get the position it starts from and moves to
							if (block != null){
								climbing = true;
								m_Anim.SetBool("Climbing", true);
								preClimbPosition = transform.position;
								postClimbPosition = transform.Find("ClimbToPoint").position;
								print(preClimbPosition+","+postClimbPosition);
							}
						}
					}
				} catch{}
			}
		}
	}

	// Climb movement
	void ClimbActions(){
		if (climbing){
			m_Rigidbody2D.velocity = new Vector3(0f, 0f, 0f);
			if (climbMoving){
				m_Rigidbody2D.isKinematic = true;
				transform.position = Vector3.MoveTowards(transform.position, postClimbPosition, climbVelocity*Time.fixedDeltaTime);
			}
		}
	}

	// Activates movement portion of climb
	public void ActivateClimbMovement(){
		climbMoving = true;
	}

	// Finishes climb movement
	public void CompleteClimbMovement(){
		finishingClimb = true;
		climbMoving = false;
		m_Rigidbody2D.isKinematic = false;
		m_Anim.SetBool("FinishingClimb", true);
	}

	// Finishes a climb
	public void CompleteClimb(){
		m_Rigidbody2D.isKinematic = false;
		climbing = false;
		climbMoving = false;
		finishingClimb = true;
		m_Anim.SetBool("Climbing", false);
		m_Anim.SetBool("FinishingClimb", false);
	}

	public override void Init(){
		// Don't call this function in the Start() if already externally instantiated
		if (finishedInit) return;
		originalAlliedSide = alliedSide;

		gameObject.tag = "Possessable";
		m_Anim = GetComponent<Animator>();
		ps = GetComponent<NetworkPossessableScript>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		GetComponent<Shatterable>().pivot = new Vector3(0f, 2f, 0f);

		look = GetComponent<LookScript>();
		//player = GameObject.Find("NetworkHand");
		//playerTransform = player.transform;
		eye = transform.Find("Eye");

		gunPivot = transform.Find("GunPivot").gameObject;
		gunTip = gunPivot.transform.Find("BulletOrigin").gameObject;
		gunPivot.SetActive(false);
		gunPivotPos = gunPivot.transform.localPosition;

		dustCloud = Resources.Load("Dustcloud");

		bulletTrail = transform.Find("BulletTrailOrigin");

		rotationAngle = transform.eulerAngles;

		underArm = gunPivot.transform.Find("UnderArm").gameObject;
		legs = transform.Find("Legs").gameObject;
		legs.SetActive(false);
		bodyRenderer = transform.Find("BodyRenderer").gameObject;
		bulletTrailPath = transform.Find("GunPivot").Find("BulletTrailOrigin").Find("BulletTrail").gameObject;
		bulletTrailPath.SetActive(false);

		headSpark = transform.Find("HeadSpark");
		headSpark.gameObject.SetActive(false);

		bloodSpatter = (GameObject)Resources.Load("BloodSpatter");

		audio = GetComponent<AudioSource>();

		solidBullet = (GameObject)Resources.Load("NetFastBullet");

		// If the soldier has a shotgun, swap out the image of the gunpivot
		if (hasShotgun){
			gunPivotImg = shotgunImage;
			possessedGunPivotImg = possessedShotgunImage;
			gunPivot.GetComponent<SpriteRenderer>().sprite = gunPivotImg;
			try{
				bulletTrailPath2 = transform.Find("GunPivot").Find("BulletTrailOrigin2").Find("BulletTrail").gameObject;
				bulletTrailPath3 = transform.Find("GunPivot").Find("BulletTrailOrigin3").Find("BulletTrail").gameObject;
			}
			catch{
				bulletTrailPath2 = Instantiate(transform.Find("GunPivot").Find("BulletTrailOrigin").gameObject);
				bulletTrailPath2.transform.SetParent(transform.Find("GunPivot"));
				bulletTrailPath2 = bulletTrailPath2.transform.Find("BulletTrail").gameObject;
				bulletTrailPath3 = Instantiate(transform.Find("GunPivot").Find("BulletTrailOrigin").gameObject);
				bulletTrailPath3.transform.SetParent(transform.Find("GunPivot"));
				bulletTrailPath3 = bulletTrailPath3.transform.Find("BulletTrail").gameObject;
			}
			bulletTrailPath2.SetActive(false);
			bulletTrailPath3.SetActive(false);
		}
		GameObject p = (GameObject)Resources.Load("TurnAround");
		//turnAroundObject = (GameObject)Resources.Load("TurnAround");
		turnAroundObject = ((GameObject)Instantiate(p)).transform;

		// Adds a ViewRange object to show where the character is looking
		if (!transform.Find("LookRange")){
			GameObject g = (GameObject)Resources.Load("ViewRange");
			g = (GameObject)Instantiate(g);
			g.transform.position = eye.position;
			g.transform.parent = transform;
		}

		// Starts the cursor
		GameObject.Find("NetCursor").GetComponent<NetworkCursor>().Init(gameObject);
		GameObject.Find("NetCamera").GetComponent<NetCameraTarget>().Init(gameObject);

		finishedInit = true;
	}

	// Make sure to disable extra little sprites when possessing
	public override void Possess(){
		base.Possess();
		legs.SetActive(false);
		bodyRenderer.SetActive(false);
		gunPivot.SetActive(false);
		gunPivot.GetComponent<SpriteRenderer>().sprite = possessedGunPivotImg;
	}

	// Make sure to disable extra little sprites when possessing
	public override void Depossess(){
		base.Depossess();
		StopAiming();
		gunPivot.GetComponent<SpriteRenderer>().sprite = gunPivotImg;
	}

	// Starts aiming if gun isn't already out
	public override void ActivateAttack(){
		base.ActivateAttack();
		if (aiming){
			Shoot();
		} else {
			StartAiming();
		}
	}

	// Climbs a ladder
	public override void ClimbLadder(){
		if (touchingLadder == true){
			// If possessed, climb up the ladder
			if (ps.possessed || inControl){
				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
					m_Anim.SetBool("LadderClimb", true);
					climbingLadder = true;
					ladderDestination = ladder.GetComponent<ClimbableLadder>().StartClimb(gameObject);
					ladderDirection = ladder.GetComponent<ClimbableLadder>().GetDirection(gameObject);
				}
			}
		}

		touchingLadder = false;
	}
	[Command]
	public override void CmdDeath(){
		turnAroundObject.gameObject.SetActive(false);
		float direction = transform.localScale.x;
		direction = (direction > 0) ? 1f : -1f;
		for (int i = 0; i < 9; i++){
			GameObject b = Instantiate(bloodSpatter);
			Vector3 tempPosition = eye.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += Random.Range(-1f, 1f);
			tempPosition.y += Random.Range(-1f, 1f);
			tempPosition.z += Random.Range(-1f, 1f);
			tempAngles.z = Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= Random.Range(.5f, 1f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(1f, 5f)*direction, Random.Range(-3f, 1f));
		}
	}

	// Crouches
	[Command]
	public void CmdCrouch(){
		DisableSubcomponents();
		m_Anim.SetBool("Crouch", true);
		StopAiming();
	}

	// Uncrouches
	[Command]
	public void CmdUncrouch(){
		m_Anim.SetBool("Crouch", false);
	}

	// Disables the display of body sub-components
	public override void DisableSubcomponents(){
		//print("Subcomponents disabled");
		//underArm.SetActive(false);
		legs.SetActive(false);
		bodyRenderer.SetActive(false);
		gunPivot.SetActive(false);
	}

	// Sets the target to be whatever last damaged the soldier
	public override void Injured(GameObject attacker){
		//Alert();
		if (attacker.GetComponent<NetworkGenericEnemy>()){
			if (attacker.GetComponent<NetworkGenericEnemy>().alliedSide != alliedSide){
				SetTargetObject(attacker);
				Alert();
				SetTargetObject(attacker);
				recentlyInjuredTimer = 3f;
			}
		}
	}

	// Sets the targeted object
	void SetTargetObject(GameObject g){
		if (g == gameObject) return;
		allTargets.Add(g);
		targetObject = g;
	}

	// Gets the targeted object
	GameObject GetTargetObject(){
		if (falseTarget){
			return falseTarget;
		}
		return targetObject;
	}

	// Sets to target to actual player, disregarding distractions
	void TargetRealPlayer(){
		//SetTargetObject(playerTransform.gameObject);
		falseTarget = null;
		falseTargetTimer = 0f;
	}

	// Checks if something is within the range of view
	bool InViewAngle(Transform t){
		Vector3 lookPoint = t.GetComponent<BoxCollider2D>().bounds.center;
		float angle = Mathf.Atan2(lookPoint.y - eye.position.y, lookPoint.x - eye.position.x);
		if (angle*Mathf.Rad2Deg < 0f){
			angle += Mathf.Deg2Rad*360f;
		}
		// Checks if the angle is within the look angle range
		// Check left looking by default, but right if facing right
		// When facing left, it's based around 180 degrees; when facing right, it's based around 0 degrees
		// Have it set to right first just to give it a value
		float angleDifference = Mathf.Abs(Mathf.DeltaAngle(angle*Mathf.Rad2Deg, 0f)*Mathf.Deg2Rad);
		if (transform.localScale.x < 0){
			angleDifference = Mathf.Abs(Mathf.DeltaAngle(angle*Mathf.Rad2Deg, 180f)*Mathf.Deg2Rad);
		}
		float viewAngle = lookAngle;

		return angleDifference <= viewAngle;
	}

	// Looks at the target
	void Look(){
		/*
		if (1 == 1) return;
		// The player
		Transform targetTransform = GetTargetObject().transform;
		//if (objectAbove){
		//	SetTargetObject(stairs);
		//}

		// Suspicious targets (if their threshold is sufficiently high, look at them too)
		ArrayList suspiciousTargets = playerTransform.GetComponent<NetworkHandController>().GetSuspiciousTargets();
		// Checks if the object saw the player
		lookTimer -= Time.fixedDeltaTime;
		lastSawTimer += Time.fixedDeltaTime;
		if (lookTimer <= 0f){
			// Just make sure we don't meme up by walking backwards when we shouldn't
			walkingBackwards = false;

			Vector3 lookPoint = GetTargetObject().GetComponent<BoxCollider2D>().bounds.center;
			float angle = Mathf.Atan2(lookPoint.y - eye.position.y, lookPoint.x - eye.position.x);
			if (angle*Mathf.Rad2Deg < 0f){
				angle += Mathf.Deg2Rad*360f;
			}
			// Checks if the angle is within the look angle range
			// Check left looking by default, but right if facing right
			// When facing left, it's based around 180 degrees; when facing right, it's based around 0 degrees
			// Have it set to right first just to give it a value
			float angleDifference = Mathf.Abs(Mathf.DeltaAngle(angle*Mathf.Rad2Deg, 0f)*Mathf.Deg2Rad);
			if (transform.localScale.x < 0){
				angleDifference = Mathf.Abs(Mathf.DeltaAngle(angle*Mathf.Rad2Deg, 180f)*Mathf.Deg2Rad);
			}
			//float angleDifference = 0f;
			// Start aiming if within look range
			// If not alerted, the look angle is very limited
			// If alerted, the look range is larger
			float viewAngle = lookAngle;
			if (lastSawTimer < maxAlertLastSawTimer){
				viewAngle = fullLookAngle;
			}
			// Looks for the player OR target object
			// Sometimes the target may be a false Target (distraction), so we don't want it to override the presence of the player
			if (angleDifference <= viewAngle){
				bool sawRealPlayer = ClearPathToTarget(playerTransform, eye, lookPoint, maxLookDist);
				sawPlayer = (sawRealPlayer) || (GetTargetObject() != playerTransform && ClearPathToTarget(GetTargetObject().transform, eye, lookPoint, maxLookDist));
				if (sawPlayer){
					// Nulls out having seen a false target and pursues the player
					if (sawRealPlayer && InViewAngle(playerTransform)){
						TargetRealPlayer();
					}
					Alert();
				}
				// Set to walk backwards if the target is right above the soldier, in order to get a better shot
				if (angleDifference > gunAimAngle){
					walkingBackwards = true;
					// For accurate destination position
					//float destX = (lookPoint.y - eye.position.y)*Mathf.Tan(lookAngle);
					//chaseDest = new Vector3(transform.x + destX, transform.y, transform.z);
					// For simply moving left or right until it works
					float moveDiff = 3f;
					if (lookPoint.x > transform.position.x){
						chaseDest = new Vector3(transform.position.x - moveDiff, transform.position.y, transform.position.z);
					} else {
						chaseDest = new Vector3(transform.position.x + moveDiff, transform.position.y, transform.position.z);
					}
				}
				// Sets the target to be the player's last seen position
				else {
					if (lastSawTimer < maxTurnLastSawTimer){
						chaseDest = targetTransform.position;
					}
				}
			}

			lookTimer = maxLookTimer;
		}
		*/
	}

	/// <summary>
	/// Manages the curiousity of an object (its target and remaining time)
	/// </summary>
	public override void ManageCuriousity(){
		// Sets the object's direction of movement to be towards the point of curiousity
		bool wasCurious = curious;
		curious = false;
		if (curiousityTimer > 0f && !alerted){
			curiousityTimer -= Time.fixedDeltaTime;
			chaseDest = curiousityPoint;
			curious = true;
			// prevents QuestionSound from playing endlessly
			if (!wasCurious){
				PlayQuestionSound();
			}
		}
		// Interest in last sound fades and new loud noises take priority (although quiet ones don't)
		lastVolume -= Time.fixedDeltaTime;
		// Object becomes less paranoid as time goes on
		paranoiaLevel -= Time.fixedDeltaTime*paranoiaFadeRate;
	}
	/// <summary>
	/// Becomes curious and moves towards an object if it's notably loud
	/// </summary>
	/// <param name="loc">Location</param>
	public override void TriggerCuriousity(Transform trans, float vol){
		if (vol >= lastVolume){
			curiousityTimer = maxCuriousityTimer;
			curiousityPoint = trans.position;
			if (!trans.GetComponent<SoundRing>().AlreadyHeardBy(gameObject) && trans.GetComponent<SoundRing>().spawn != gameObject){
				if (trans.GetComponent<SoundRing>().spawn.GetComponent<NetworkPossessableScript>() == null || 
				 (trans.GetComponent<SoundRing>().spawn.GetComponent<NetworkPossessableScript>() && trans.GetComponent<SoundRing>().spawn.GetComponent<NetworkPossessableScript>().possessed)){
					if (paranoiaLevel <= 0f){
						paranoiaLevel = 0f;
					}
					paranoiaLevel += vol/soundSensitivity;
					if (paranoiaLevel >= 1f && trans.GetComponent<SoundRing>().spawn != null){
						falseTarget = trans.GetComponent<SoundRing>().spawn.gameObject;
						falseTargetTimer = maxFalseTargetTimer;
						Alert();
					}
				}
			}
		}
	}

	// Is alerted and starts chasing target
	public override void Alert(){
		// Only target living objects
		if (GetTargetObject().GetComponent<NetworkGenericEnemy>().life <= 0f){
			return;
		}
		if (!alerted){
			PlayExclamationSound();
		}
		lastSawTimer = 0f;
		alerted = true;
		SetParanoia(1.1f); // Maxes out the paranoia level
		paranoiaLevel = 1f;
		StartAiming();
	}

	public void Move(){
		// Move the character
		m_Rigidbody2D.velocity = new Vector2(moveDirection*walkSpeed, m_Rigidbody2D.velocity.y);
		//Look();

		if (moveDirection > 0 && facing < 0){
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (moveDirection < 0 && facing > 0){
			Flip();
		}

		// Patrols an area
		if (patrolling && !curious){
			Patrol();
		// Approaches a target of interest
		} else if (curious){
			ApproachTarget();
		}
	}

	// Fires a single shot in a specified direction
	[Command]
	void CmdFireSingleShot(Vector3 dir, GameObject bTrail){
		// Gets all hits going out from the gun tip
		float distance = 100f;
		Vector3 direction = dir;
		//direction.x *= transform.localScale.x; 		// Sign flip on the direction
		RaycastHit2D[] hits = Physics2D.RaycastAll(gunTip.transform.position, direction, distance);
		float closestCollisionDistance = distance;
		GameObject closestCollision = null;
		bool blocked = false;
		
		// If possessed, shots will hit the nearest object
		bool possessed = ps.possessed;

		
		audio.Play();

		GameObject b2 = (GameObject)Instantiate(solidBullet);
		b2.transform.position = gunTip.transform.position;
		b2.GetComponent<FastBullet>().spawn = gameObject;
		b2.GetComponent<FastBullet>().Fire(direction);

		NetworkServer.Spawn(b2);
	}

	// Fires the gun
	void Shoot(){
		shotTimer = maxShotTimer;
		backFireAngle = maxBackfireAngle;
		aimSetTimer = maxAimSetTimer;

		// Fires one shot if a normal rifle, and a couple extras at slight offsets if a shotgun
		CmdFireSingleShot(gunTip.transform.forward, bulletTrailPath);
		if (hasShotgun){
			Vector3 tempVec = gunTip.transform.forward;
			tempVec.x *= facing;
			tempVec = Vector3.RotateTowards(gunTip.transform.forward, Vector3.up, Random.Range(5f * Mathf.Deg2Rad, 10f * Mathf.Deg2Rad), 1f);
			CmdFireSingleShot(tempVec, bulletTrailPath2);
			tempVec = Vector3.RotateTowards(gunTip.transform.forward, -Vector3.up, Random.Range(5f * Mathf.Deg2Rad, 10f * Mathf.Deg2Rad), 1f);
			CmdFireSingleShot(tempVec, bulletTrailPath3);
		}
		

		// Shake camera a bit
		Camera.main.GetComponent<NetCameraTarget>().AddShake(.05f);
		// Add a dust cloud
		GameObject p = (GameObject)Instantiate(dustCloud);
		p.transform.position = gunTip.transform.position;
		p.transform.localScale *= .3f;
		p.GetComponent<Dustcloud>().velocity = gunTip.transform.forward * Time.fixedDeltaTime;
		p.GetComponent<Dustcloud>().velocity.x *= transform.localScale.x;
		// Add a sound ring
		AddSoundRing(gunTip.transform.position, gunVolume);
	}

	// Starts aiming animation/enables display
	void StartAiming(){
		// We want to have a small delay before shooting outright so that the player has a second to respond
		if (!aiming){
			shotTimer = postAimShotTimer;
		}
		// Activate all aiming related stuff
		m_Anim.SetBool("Aim", true);
		aiming = true;
		gunPivot.SetActive(true);
		if (ps.possessed || inControl){
			bodyRenderer.SetActive(true);
		}
		legs.SetActive(true);
	}

	// Ends aiming and stops displaying arms, whether disalarmed or frozen
	void StopAiming(){
		m_Anim.SetBool("Aim", false);
		m_Anim.SetBool("Run", false);
		aiming = false;
		gunPivot.SetActive(false);
		bodyRenderer.SetActive(false);
		legs.SetActive(false);
		DisableSubcomponents();
		EndAlert();
	}

	// Determines the minimum distance that should be maintained from an object
	float GetMinDist(float suggestedDist){
		/*
		if (GetTargetObject().transform != playerTransform){
			//return 0f;
			return suggestedDist;
		} else {
			//if suspiciousLevel >50f
			// return suggestedDist * 1.25f;
			// else
			return suggestedDist;
		}
		*/
		return 100f;
	}
	// Determines whether the player should run
	void DetermineRun(){
		// Can't run while preparing to shoot
		if (shotTimer < 0f){
			bodyRenderer.SetActive(true);
			gunPivot.SetActive(true);
			legs.SetActive(true);
			m_Anim.SetBool("Run", false);

			return;
		}
		// Can't run backwards
		if (!walkingBackwards){
			float minDist = minRunDist;
			if (Mathf.Abs(chaseDest.x - transform.position.x) > minRunDist){
				// We set a minimum time to run to prevent those nasty rapid stops/goes
				runTimer = Time.fixedDeltaTime * 5f;
				Vector2 tempVelocity = m_Rigidbody2D.velocity;
				if (chaseDest.x - minDist > transform.position.x){
					tempVelocity.x = runSpeed;
				} else if (chaseDest.x + minDist < transform.position.x){
					tempVelocity.x = -runSpeed;
				}
				m_Rigidbody2D.velocity = tempVelocity;

				bodyRenderer.SetActive(false);
				gunPivot.SetActive(false);
				legs.SetActive(false);

				m_Anim.SetBool("Run", true);
			} else {
				if (runTimer > 0f){
					runTimer -= Time.fixedDeltaTime;

					bodyRenderer.SetActive(false);
					gunPivot.SetActive(false);
					legs.SetActive(false);
					m_Anim.SetBool("Run", true);
				} else {
					bodyRenderer.SetActive(true);
					gunPivot.SetActive(true);
					legs.SetActive(true);
					m_Anim.SetBool("Run", false);
				}
			}
		}
	}

	// Moves towards a target of interest
	// NOT a full chase
	void ApproachTarget(){
		// Right now this is just a basic move on the x towards an object algo
		// Make it more advanced and include some path finding
		Vector2 tempVelocity = m_Rigidbody2D.velocity;
		float minDist = GetMinDist(minDangerDistance);
		minDist /= 2f;
		if (chaseDest.x - minDist > transform.position.x){
			tempVelocity.x = curiousSpeed;
			legs.GetComponent<Animator>().SetFloat("Speed", 1f);
			moveDirection = 1f;
		} else if (chaseDest.x + minDist < transform.position.x){
			tempVelocity.x = -curiousSpeed;
			moveDirection = -1f;
			legs.GetComponent<Animator>().SetFloat("Speed", 1f);
		} else {
			moveDirection = 0f;
			legs.GetComponent<Animator>().SetFloat("Speed", 0f);
		}
		m_Rigidbody2D.velocity = tempVelocity;
		if (Mathf.Abs(m_Rigidbody2D.velocity.x) >= .1f){
			m_Anim.SetFloat("Speed", 1f);
		} else {
			m_Anim.SetFloat("Speed", 0f);
		}
	}

	// Player input
	public void PlayerControls(){
		// Won't execute if it's another player
		if (!isLocalPlayer){
			return;
		}
		// Jumps if on the ground
		if (onGround && Input.GetButtonDown("Jump")){
			lastJumpTimer = .1f;
			m_Anim.SetBool("Jump", false);
			Vector2 tempV = m_Rigidbody2D.velocity;
			tempV.y = jumpVel;
			m_Rigidbody2D.velocity = tempV;
			onGround = false;
			jumping = true;
		}
		shotDelayTimer -= Time.deltaTime;
		if (!ps.inIntroAnim && !climbing){
			walkingBackwards = false;
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				moveDirection = 1;
			} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
				moveDirection = -1;
			} else {
				moveDirection = 0;
			}
			// Move the character
			m_Rigidbody2D.velocity = new Vector2(moveDirection*chaseSpeed, m_Rigidbody2D.velocity.y);

			// Starts aiming/shoots when left clicking
			//if (!ps.attackMenu.activeSelf){
			if (shotDelayTimer <= 0f){
				if (Input.GetMouseButtonDown(0)){
					shotDelayTimer = maxShotDelayTimer;
					ActivateAttack();
				}
			}
			//}

			// Starts running
			if (moveDirection != 0 && (ps.IsRunning() || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))){
				bodyRenderer.SetActive(false);
				gunPivot.SetActive(false);
				legs.SetActive(false);
				m_Anim.SetBool("Run", true);
			} else if (moveDirection == 0){
				m_Anim.SetBool("Run", false);
			}
			// Sets run velocity
			if (m_Anim.GetBool("Run")){
				m_Rigidbody2D.velocity = new Vector2(moveDirection*runSpeed*1.25f, m_Rigidbody2D.velocity.y);
				// If no longer running and we were aiming, resume aiming
			} else {
				if (aiming){
					if (!legs.activeSelf){
						StopAiming();
						StartAiming();
					}
					bodyRenderer.transform.position -= aimBob;
					gunPivot.transform.position -= aimBob;
					aimBob = new Vector3(0f, 0f, 0f);
					if (m_Rigidbody2D.velocity.x != 0f){
						legs.GetComponent<Animator>().SetFloat("Speed", 1f);

						// Bobs up and down while walking
						float bobRadius = .040f;
						aimBob = new Vector3(0f, Mathf.Abs(bobRadius*Mathf.Sin(aimBobAng)), 0f);
						if (aimBob.y > bobRadius*.66f){
							aimBob.y = bobRadius*.66f;
						} else if (aimBob.y > bobRadius*.33f){
							aimBob.y = bobRadius*.33f;
						} else {
							aimBob.y = 0f;
						}
						aimBobAng += aimBobDAng;
						bodyRenderer.transform.position += aimBob;
						gunPivot.transform.position += aimBob;
						//legs.transform.position -= aimBob;

						// Sets the legs to move in reverse if necessary
						if ((transform.localScale.x > 0 && m_Rigidbody2D.velocity.x < 0) || (transform.localScale.x < 0 && m_Rigidbody2D.velocity.x > 0)){
							walkingBackwards = true;
						}

					} else {
						legs.GetComponent<Animator>().SetFloat("Speed", 0f);
						aimBobAng = 0f;
					}
					// Handles the aiming
					// Rotate gun towards proper angle
					Vector3 cursorPoint = Input.mousePosition;
					cursorPoint.z = transform.position.z - Camera.main.transform.position.z;
					cursorPoint = Camera.main.ScreenToWorldPoint(cursorPoint);
					Vector3 tempRot = gunPivot.transform.eulerAngles;

					//Flips if the cursor is facing the other side of the player
					if (cursorPoint.x > transform.position.x && transform.localScale.x < 0f){
						Flip();
					} else if (cursorPoint.x < transform.position.x && transform.localScale.x > 0f){
						Flip();
					}

					// Slight offset of gun rotation for aesthetic purposes
					/*
					tempRot.z = look.GetAngleToObj(eye.transform.position, cursorPoint) - gunAngleOffset;
					if (tempRot.z < 0f){
						tempRot.z += 360f;
					}
					// Axis center is 180 degrees when facing left
					if (tempRot.z > gunAimAngle*Mathf.Rad2Deg && tempRot.z < 180f){
						tempRot.z = gunAimAngle*Mathf.Rad2Deg;
					} else if (tempRot.z > 180f && tempRot.z < 360f - gunAimAngle*Mathf.Rad2Deg){
						tempRot.z = 360f - gunAimAngle*Mathf.Rad2Deg;
					}
					// Does a smoother transition to the new angle
					if (Mathf.DeltaAngle(gunPivot.transform.eulerAngles.z, tempRot.z) < 75){
						tempRot.z = Mathf.MoveTowardsAngle(gunPivot.transform.eulerAngles.z, tempRot.z, 10f);
					}
					*/

					//////
					tempRot.z = look.GetAngleToObj(eye.transform.position, cursorPoint) - gunAngleOffset;
					if (tempRot.z < 0f){
						tempRot.z += 360f;
					}
					// Axis center is 180 degrees when facing left
					if (tempRot.z > gunAimAngle*Mathf.Rad2Deg && tempRot.z < 180f){
						tempRot.z = gunAimAngle*Mathf.Rad2Deg;
					} else if (tempRot.z > 180f && tempRot.z < 360f - gunAimAngle*Mathf.Rad2Deg){
						tempRot.z = 360f - gunAimAngle*Mathf.Rad2Deg;
					}
					// Flips the aim when the soldier is aiming left
					if (transform.localScale.x < 0){
						if (tempRot.z < 180f){
							tempRot.z = 360f - tempRot.z;
						} else {
							tempRot.z = 360f - tempRot.z;
						}
					}
					//////
					gunPivot.transform.eulerAngles = tempRot;

					// Keeps arm from stretching too much when aiming
					gunPivot.transform.localPosition = gunPivotPos;
					Vector3 tempGunPivotPos = gunPivotPos + new Vector3(0f, -.12f * (Mathf.Sin(tempRot.z*Mathf.Deg2Rad)), 0f);
					gunPivot.transform.localPosition = tempGunPivotPos;
				}
			}
			legs.GetComponent<Animator>().SetBool("WalkBackwards", walkingBackwards);

			if (!aiming){
				if (m_Rigidbody2D.velocity.x < -.1f && transform.localScale.x > 0f){
					Flip();
				} else if (m_Rigidbody2D.velocity.x > .1f && transform.localScale.x < 0f){
					Flip();
				}
			}

			// Crouches
			if (Input.GetKey(KeyCode.S)){
				m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
				CmdCrouch();
			} else {
				CmdUncrouch();
			}

			ClimbBlock();

			// Sync up animations
			if (m_Rigidbody2D.velocity.x != 0f){
				m_Anim.SetFloat("Speed", 1f);
			} else {
				m_Anim.SetFloat("Speed", 0f);
			}
		}
	}
	
	// Aims/shoots
	void Aim(){
		//if (lastSawTimer <= maxTurnLastSawTimer){
		// Bobs up and down while walking
		bodyRenderer.transform.position -= aimBob;
		gunPivot.transform.position -= aimBob;
		//legs.transform.position += aimBob;
		aimBob = new Vector3(0f, 0f, 0f);

		// Checks whether the soldier just locked on
		bool shotTimerWasOverZero = (shotTimer >= 0f);
		// Shoots and fires
		shotTimer -= Time.fixedDeltaTime;
		// Can't aim while running
		if (shotTimer < 0f){
			if (aimSetTimer >= maxAimSetTimer){
				// Shows a brief spark to show that the character's aim is set
				// Right before the spark, check to see if an allied object is between the Soldier and the Target
				// If so, don't shoot and reduce the aimSetTimer a bit
				if (!headSpark.gameObject.activeSelf){
					// Gets all hits going out from the gun tip
					float distance = 100f;
					Vector3 direction = gunTip.transform.forward;
					direction.x *= transform.localScale.x;
					RaycastHit2D[] hits = Physics2D.RaycastAll(gunTip.transform.position, direction, distance);
					float closestCollisionDistance = distance;
					GameObject closestCollision = null;
					bool blocked = false;
					// If possessed, shots will hit the nearest object
					bool possessed = ps.possessed || inControl;
					// Inital pass to test for wall collisions
					foreach (RaycastHit2D r in hits){
						if (!r.collider.isTrigger || (possessed && r.collider.isTrigger) || (r.collider.isTrigger && r.collider.GetComponent<NetworkGenericEnemy>())){
							if (r.transform != transform){
								// See what wall we hit, and reset the grab reach based on that
								//float dist = Vector2.Distance(r.point, gunTip.transform.position);
								float dist = r.distance;
								// Shot blocked by a wall
								if (dist < closestCollisionDistance){
									// Collisions only register if:
									// 1: The object hit is solid and NOT a trigger
									// OR
									// 2: The object hit IS a trigger, but is the target

									// NOTE: as it is, the soldier's bullets will pass around all non-target objects UNLESS they're humans
									if (r.collider.gameObject.tag == "Player"){
										continue;
									}
									if (!r.collider.isTrigger){
										blocked = true;
										closestCollisionDistance = dist;
										closestCollision = r.collider.gameObject;
									} else if (r.collider.gameObject == GetTargetObject() && !possessed){
										closestCollisionDistance = dist;
										closestCollision = r.collider.gameObject;
									} else if (possessed && r.collider.GetComponent<NetworkGenericEnemy>()){
										closestCollisionDistance = dist;
										closestCollision = r.collider.gameObject;
									} else if (r.collider.gameObject.GetComponent<NetworkPossessableScript>() && r.collider.gameObject.GetComponent<NetworkPossessableScript>().isHuman){
										closestCollisionDistance = dist;
										closestCollision = r.collider.gameObject;
									} 
								}
							}
						}
					}
					// Don't shoot if an ally is in the way and take more time to prepare a shot
					if (closestCollision != null && closestCollision.GetComponent<NetworkGenericEnemy>() && closestCollision.GetComponent<NetworkGenericEnemy>().alliedSide == alliedSide){
						shotTimer = maxShotTimer / 2f;
						aimSetTimer = maxAimSetTimer;
					// Otherwise, get the headspark active and get ready to shoot
					} else {
						headSpark.gameObject.SetActive(true);
					}
				}
			}
			aimSetTimer -= Time.fixedDeltaTime;
			if (aimSetTimer <= 0f && !m_Anim.GetBool("Run")){
				//headSpark.gameObject.SetActive(false);
				if (warnTarget && warnLowerTimer <= 0f){
					warnLowerTimer = 5f;
					Camera.main.GetComponent<NetCameraTarget>().AddMinichat(eye.position + new Vector3(0f, 1f, 0f), DialogueTable.LowRank());
				}
				if (!warnTarget || Vector2.Distance(transform.position, GetTargetObject().transform.position) < warnDist){
					Shoot();
				}
			// Make sure to have a period of aiming before shooting when stopping from a run
			} else if (m_Anim.GetBool("Run")){
				aimSetTimer = maxAimSetTimer;

				bodyRenderer.SetActive(true);
				gunPivot.SetActive(true);
				legs.SetActive(true);
				m_Anim.SetBool("Run", false);
			}
		} else {
			// Makes sure the soldier has to take a fraction of a second to stabilize its aim, giving the player time to jump out of the way
			aimSetTimer = maxAimSetTimer;
			headSpark.gameObject.SetActive(false);
		}
		
		// Soldier can only take aim before he's ready to fire the shot
		if (shotTimer > 0f){
			// Cool down from backfire
			if (backFireAngle >= .1f){
				gunPivot.transform.eulerAngles = preShotGunRotation + new Vector3(0f, 0f, backFireAngle);
			}
			// Otherwise, take aim again
			else {
				if (GetTargetObject().transform.position.x > transform.position.x && transform.localScale.x < 0){
					Vector3 tempScale = transform.localScale;
					tempScale.x = -tempScale.x;
					transform.localScale = tempScale;
				} else if (GetTargetObject().transform.position.x < transform.position.x && transform.localScale.x > 0){
					Vector3 tempScale = transform.localScale;
					tempScale.x = -tempScale.x;
					transform.localScale = tempScale;
				}

				// Rotate gun towards proper angle
				Vector3 tempRot = gunPivot.transform.eulerAngles;
				// Slight offset of gun rotation for aesthetic purposes
				tempRot.z = look.GetAngleToObj(eye.transform.position, GetTargetObject().GetComponent<BoxCollider2D>().bounds.center) - gunAngleOffset;
				if (tempRot.z < 0f){
					tempRot.z += 360f;
				}
				// Axis center is 180 degrees when facing left
				if (tempRot.z > gunAimAngle*Mathf.Rad2Deg && tempRot.z < 180f){
					tempRot.z = gunAimAngle*Mathf.Rad2Deg;
				} else if (tempRot.z > 180f && tempRot.z < 360f - gunAimAngle*Mathf.Rad2Deg){
					tempRot.z = 360f - gunAimAngle*Mathf.Rad2Deg;
				}
				// Flips the aim when the soldier is aiming left
				if (transform.localScale.x < 0){
					if (tempRot.z < 180f){
						tempRot.z = 360f - tempRot.z;
					} else {
						tempRot.z = 360f - tempRot.z;
					}
				}
				// Does a smoother transition to the new angle
				if (Mathf.DeltaAngle(gunPivot.transform.eulerAngles.z, tempRot.z) < 75){
					tempRot.z = Mathf.MoveTowardsAngle(gunPivot.transform.eulerAngles.z, tempRot.z, 10f);
				}
				gunPivot.transform.eulerAngles = tempRot;
				
				// Keeps arm from stretching too much when aiming
				gunPivot.transform.localPosition = gunPivotPos;
				Vector3 tempGunPivotPos = gunPivotPos + new Vector3(0f, -.12f * (Mathf.Sin(tempRot.z*Mathf.Deg2Rad)), 0f);
				gunPivot.transform.localPosition = tempGunPivotPos;

				/*
				// Walk backwards if the target is above the look angle
				if (walkingBackwards){
					Vector2 tempVelocity = m_Rigidbody2D.velocity;
					if (chaseDest.x > transform.position.x){
						tempVelocity.x = chaseSpeed;
					} else if (chaseDest.x < transform.position.x){
						tempVelocity.x = -chaseSpeed;
					}
					m_Rigidbody2D.velocity = tempVelocity;
				}
				// Walk closer if not walking backwards and the gun is fairly aligned
				else {
					Vector2 tempVelocity = m_Rigidbody2D.velocity;
					float minDist = GetMinDist(minDangerDistance);
					if (chaseDest.x - minDist > transform.position.x){
						tempVelocity.x = chaseSpeed;
					} else if (chaseDest.x + minDist < transform.position.x){
						tempVelocity.x = -chaseSpeed;
					}
					m_Rigidbody2D.velocity = tempVelocity;
				}
				*/
				Chase();
				preShotGunRotation = gunPivot.transform.eulerAngles;
			}
		}
		// Leg animation stuff
		legs.GetComponent<Animator>().SetBool("WalkBackwards", walkingBackwards);
		if (m_Rigidbody2D.velocity.x != 0f){
			legs.GetComponent<Animator>().SetFloat("Speed", 1f);
			
			// Bobs up and down while walking
			float bobRadius = .040f;
			aimBob = new Vector3(0f, Mathf.Abs(bobRadius*Mathf.Sin(aimBobAng)), 0f);
			if (aimBob.y > bobRadius*.66f){
				aimBob.y = bobRadius*.66f;
			} else if (aimBob.y > bobRadius*.33f){
				aimBob.y = bobRadius*.33f;
			} else {
				aimBob.y = 0f;
			}
			aimBobAng += aimBobDAng;
			bodyRenderer.transform.position += aimBob;
			gunPivot.transform.position += aimBob;
			//legs.transform.position -= aimBob;
			
			// Determines whether the soldier should run and handles cleanup code
			DetermineRun();
			
		} else {
			legs.GetComponent<Animator>().SetFloat("Speed", 0f);
			aimBobAng = 0f;
		}
		//}
	}

	// Chases the player
	private void Chase(){
		// Walk backwards if the target is above the look angle
		if (walkingBackwards){
			Vector2 tempVelocity = m_Rigidbody2D.velocity;
			if (chaseDest.x > transform.position.x){
				tempVelocity.x = chaseSpeed;
			} else if (chaseDest.x < transform.position.x){
				tempVelocity.x = -chaseSpeed;
			}
			m_Rigidbody2D.velocity = tempVelocity;
		}
		// Walk closer if not walking backwards and the gun is fairly aligned
		else {
			Vector2 tempVelocity = m_Rigidbody2D.velocity;
			float minDist = GetMinDist(minDangerDistance);
			if (chaseDest.x - minDist > transform.position.x){
				tempVelocity.x = chaseSpeed;
			} else if (chaseDest.x + minDist < transform.position.x){
				tempVelocity.x = -chaseSpeed;
			}
			m_Rigidbody2D.velocity = tempVelocity;
		}

		// If the player is on another level, follow the path leading towards that level
		//if(1 == 1){

		//}
	}

	// Sets scale to be normal & unflipped
	private void FixFlip(){
		Vector3 tempScale = transform.localScale;
		tempScale.x = Mathf.Abs(tempScale.x) * facing;

		transform.localScale = tempScale;
	}

	// Returns to the location of patrol
	private void ReturnToPatrolPoint(){
		// First we determine if we can return to the patrol point
		bool canReturnToPatrolPoint = true;

		// If we can, head that way
		if (canReturnToPatrolPoint){

		}
	}

	// Walks back and forth
	private void Patrol(){
		// Doesn't patrol
		if (nonPatrollingEntity){
			Vector3 tempVelocity = m_Rigidbody2D.velocity;
			tempVelocity.x = 0f;
			m_Rigidbody2D.velocity = tempVelocity;
			return;
		}

		if (m_Rigidbody2D.velocity.x > 0f){
			facing = 1;
		} else if (m_Rigidbody2D.velocity.x < 0f){
			facing = -1;
		}
		turnAroundObject.GetComponent<TimerDispatcher>().SetTimer((maxPatrolTimer + maxPatrolWaitTimer) / (patrolTimer + patrolWaitTimer), eye.position + new Vector3(0f, .5f, 0f));
		if (patrolTimer >= maxPatrolTimer){
			moveDirection = 0f;

			patrolWaitTimer += Time.deltaTime * 60f;
			if (patrolWaitTimer >= maxPatrolWaitTimer){
				patrolWaitTimer = 0f;
				patrolTimer = 0f;

				// Flips direction
				if (facing < 0){
					moveDirection = 1f;
				} else {
					moveDirection = -1f;
				}
			}
		} else {
			patrolTimer += Time.deltaTime * 60f;
		}
		walkingBackwards = false;

		FixFlip();

		// Checks if there's ground available to walk across
		if (transform.Find("FallCheck")){
			if (!transform.Find("FallCheck").GetComponent<WallCheck>().IsColliding()){
				Vector3 tempVelocity = m_Rigidbody2D.velocity;
				tempVelocity.x = 0f;
				m_Rigidbody2D.velocity = tempVelocity;
			}
		}

		// Sync up animations
		if (Mathf.Abs(m_Rigidbody2D.velocity.x) >= .1f){
			m_Anim.SetFloat("Speed", 1f);
		} else {
			m_Anim.SetFloat("Speed", 0f);
		}
	}
	// Starts patrolling normally
	void ResumePatrolling(){
		patrolling = true;
		patrolTimer = 0f;
		EndAlert();
	}

	// Warns those who are too low ranked to enter an area
	void WarnLowerRank(){

	}

	// Toggles the display of the turnaround timer above the soldider
	void ToggleTurnAroundDisplay(){
		if (ps.possessed || curious || inControl){
			turnAroundObject.gameObject.SetActive(false);
		} else {
			turnAroundObject.gameObject.SetActive(true);
		}
	}

	void Update(){
		ToggleTurnAroundDisplay();
		// reverts allied side after a brief period
		if (alliedSideChangeTimer > 0f){
			alliedSideChangeTimer -= Time.deltaTime;
			if (alliedSideChangeTimer <= 0f){
				alliedSide = originalAlliedSide;
			}
		}
		ClimbLadder();
	}

	public override void StopClimbingLadder(){
		m_Anim.SetBool("LadderClimb", false);
		climbingLadder = false;
		m_Rigidbody2D.isKinematic = false;
	}

	private void FixedUpdate(){
		damageTimer -= Time.fixedDeltaTime;
		warnLowerTimer -= Time.fixedDeltaTime;
		lastJumpTimer -= Time.fixedDeltaTime;

		// Climbs up ladder
		if (climbingLadder){
			m_Rigidbody2D.isKinematic = true;
			m_Rigidbody2D.velocity = new Vector2(0f, 0f);
			transform.position += new Vector3(0f * Time.fixedDeltaTime, ladderDirection * 7f * Time.fixedDeltaTime, 0f);
			if (ladderDirection > 0){
				if (transform.position.y >= ladderDestination.y){
					StopClimbingLadder();
				}
			} else {
				if (transform.position.y <= ladderDestination.y){
					StopClimbingLadder();
				}
			}
			return;
		}

		// Sets the targeted object
		warnTarget = false;

		allTargets.Clear();
		// Sets the target to be a False Target
		if (falseTarget != null){
			allTargets.Add(falseTarget);
			SetTargetObject(falseTarget);
			falseTargetTimer -= Time.fixedDeltaTime;
		}
		/*
		// Have a brief focus on other objects that hurt the character
		recentlyInjuredTimer -= Time.fixedDeltaTime;
		if (targetObject == null){
			recentlyInjuredTimer = 0f;
		}
		// If not distracted/alerted by a false target, follow the player
		// Also, ignore this if recently attacked by another enemy
		if (recentlyInjuredTimer <= 0f){
			if (!player.activeSelf){
				GameObject g = GameObject.FindGameObjectWithTag("Player");
				if (g.GetComponent<NetworkGenericEnemy>().alliedSide != 1 && g.GetComponent<NetworkGenericEnemy>().alliedSide != alliedSide){
					SetTargetObject(g);
				}
				// For targets on the same side but lower rank, mark them as targets to warn instead of directly attacking
				else if (alliedSide == g.GetComponent<NetworkGenericEnemy>().alliedSide &&
				 ps.stopLowerRanks && (g.GetComponent<NetworkPossessableScript>().rank != 0 && g.GetComponent<NetworkPossessableScript>().rank < ps.rank)){
					warnTarget = true;
					SetTargetObject(g);
					// Set target as the Hand so that the possessed object isn't targeted
				} else {
					//SetTargetObject(player);
				}
			} else {
				//SetTargetObject(player);
			}
		}
		*/
		// Manages curiousity
		ManageCuriousity();

		ClimbActions();

		// Sets paranoia display
		SetParanoia(paranoiaLevel);

		// Makes sure the bullet trail is off
		if (bulletTrailTimer > 0f){
			bulletTrailTimer -= Time.fixedDeltaTime;
			if (bulletTrailTimer <= 0f){
				bulletTrailPath.SetActive(false);
				if (hasShotgun){
					bulletTrailPath2.SetActive(false);
					bulletTrailPath3.SetActive(false);
				}
			}
		}
		// Reduces the backfire amount
		backFireAngle = Mathf.MoveTowards(backFireAngle, 0f, reduceBackfireAngle);
		// Can't do anything if frozen
		if (!ps.frozen){
			if (ps.possessed || inControl){
				//rigid.isKinematic = false;
				m_Anim.SetBool("Possessed", true);
				PlayerControls();
			} else {
				
			}
		} else {
			StopAiming();
		}

		// If running, disable body subcomponents
		if (m_Anim.GetBool("Run")){
			DisableSubcomponents();
		}

		// Sets the body's sprite
		bodyRenderer.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		if (moveDirection > 0) {
			facing = 1;
		} else if (moveDirection < 0){
			facing = -1;
		}
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}



	// Check if it came in contact with the player
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Door" && !patrolThroughDoors){
			patrolTimer = maxPatrolTimer;
		}
		if (ps.ActionsDisabled()){
			return;
		}
		if (col.gameObject == GetTargetObject()){
			Alert();
		}
	}

	// Activates other soldiers if it touches them while alerted
	void OnCollisionEnter2D(Collision2D col){
		if (alerted && col.gameObject.GetComponent<BasicSoldierScript>()){
			if (!col.gameObject.GetComponent<BasicSoldierScript>().IsAlerted()){
				col.gameObject.GetComponent<BasicSoldierScript>().Alert();
			}
		}
	}

	// For checking if the soldier is on the ground
	void OnCollisionStay2D(Collision2D col){
		if (lastJumpTimer <= 0f){
			m_Anim.SetBool("Jump", false);
			if (col.contacts[0].point.y <= transform.position.y + .1f){
				onGround = true;
				jumping = false;
			}
		}
	}

}
