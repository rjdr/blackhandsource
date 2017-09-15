using UnityEngine;
using System.Collections;

public class WaterSpirit : GenericEnemy {
	Vector3 startPos;
	float radius = .5f;
	float ang = 0f;
	Transform farPoint;
	Transform upperPoint;
	Vector3 Spawn;
	Vector3 SpawnDestination;
	Vector3 ActivationPoint;
	Vector3 PreActivePosition;
	Vector3 WaveDownPoint;
	Vector3 WaveUpPoint;
	bool activated = false;
	bool reachedSpawnPoint = false;
	bool spawnComplete = false;
	bool stopSpawnMoving = false;
	bool movedYBorder = false;
	Vector3 spawnVelocity = new Vector3(30f, 0f, 0f);
	Vector3 reduceSpawnVelocity = new Vector3(28f, 0f, 0f);
	Vector3 escapePoint;

	Transform theHand;

	// Adjusted camera position
	Vector3 desiredCamPosition = new Vector3(0f, 0f, -13f);
	Vector3 defaultDesiredCamPosition = new Vector3(0f, 0f, -13f);
	Vector3 waterAttackDesiredCamPosition = new Vector3(0f, 0f, -15f);

	// Attack object
	GameObject WaterSpiritAttack;

	const int WATERSPIRIT = 0;
	const int LARGEWAVE = 1;
	const int PYRAMIDBEAM = 2;
	int[] AttackModes = new int[] {WATERSPIRIT, LARGEWAVE, PYRAMIDBEAM, WATERSPIRIT, PYRAMIDBEAM};
	int AttackMode = 0;
	float attackDelay = 0f;
	float maxAttackDelay = 3f;

	// Timer for launching a wave
	float largeWaveTimer = 0f;
	const float downWaveTimer = .8f;
	const float moveBackWaveTimer = downWaveTimer + 5f;
	const float moveUpWaveTimer = moveBackWaveTimer + .75f;
	const float waitUpWaveTimer = moveUpWaveTimer + 3f;
	const float returnWaveTimer = waitUpWaveTimer + 2.5f;
	GameObject WaveGroup;
	bool spawnedWave = false;

	// Timer for spawning WaterSpiritAttacks
	float waterAttackTimer = 0f;
	const float startWaterAttackInterval = 1f;
	float waterAttackInterval = startWaterAttackInterval; 		// Start point for launching the attacks
	float waterAttackIntervalIncr = 1.2f;	// Interval at which we release attacks
	float maxWaterAttackTimer = 9f;

	// For the pyramid beam attacks
	float moveToPyramidPositionTimer = 0f;
	float maxMoveToPyramidPositionTimer = 1f;
	float pyramidBeamTimer = 0f;
	float returnFromPyramidTimer = 0f;
	float maxReturnFromPyramidTimer = .5f;
	public bool pyramidBeamFinished = false;

	bool startedDestruction = false;
	bool preparedToDie = false;

	int bodyLayer = 0;
	int armsLayer = 0;

	// Position before starting an attack, for smooth lerps
	Vector3 preAttackPosition;

	// The Laser attack object
	GameObject LaserAttack;

	// Blood to splatter when hit
	GameObject blood;

	GameObject IllumHead1;
	GameObject IllumHead2;
	GameObject SubmarineObj;

	// Started escape sequence
	bool startedEscape = false;
	bool escaping = false;
	float escapeTimer = 0f;
	float maxEscapeTimer = 2f;
	Vector3 escapeCenterPoint;
	bool escapeAttackActive = false;

	float moveAngle = 0f;

	float escapeAttackTimer = 0f;
	float escapeAttackInterval = 1f;
	float escapeAttackIntervalIncr = .5f;

	GameObject RepeatingUnderwaterObj;	// Repeating moving background object
	GameObject underWaterBoulderObj; 	// Boulder object for underwater attack
	GameObject WildMoon2; 				// The underwater moon

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		killable = false;

		farPoint = transform.Find("FarPoint");
		farPoint.SetParent(null);
		upperPoint = transform.Find("UpperPoint");
		upperPoint.SetParent(null);
		escapePoint = transform.Find("EscapePoint").position;

		Spawn = transform.Find("Spawn").position;
		SpawnDestination = transform.Find("SpawnDestination").position;
		ActivationPoint = transform.Find("ActivationPoint").position;
		PreActivePosition = transform.Find("PreActivePosition").position;
		WaveDownPoint = transform.Find("WaveDownPoint").position;
		WaveUpPoint = transform.Find("WaveUpPoint").position;

		theHand = GameObject.Find("TheHand").transform;

		WaterSpiritAttack = (GameObject)Resources.Load("WaterSpiritAttack");
		WaveGroup = (GameObject)Resources.Load("MovingWaveGroup");

		LaserAttack = (GameObject)Resources.Load("Laser");

		bodyLayer = transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder;
		armsLayer = transform.Find("Arms").GetComponent<SpriteRenderer>().sortingOrder;

		blood = (GameObject)Resources.Load("BloodSpatter");

		IllumHead1 = transform.Find("IllumHeadEnemy").gameObject;
		IllumHead2 = transform.Find("IllumHeadEnemy2").gameObject;
		// Find the Submarine and deactivate it until this phase of the battle begins
		SubmarineObj = GameObject.Find("Submarine");
		SubmarineObj.SetActive(false);

		RepeatingUnderwaterObj = GameObject.Find("RepeatingUnderwater");
		RepeatingUnderwaterObj.SetActive(false);


		transform.Find("IllumHeadSub1").gameObject.SetActive(false);
		transform.Find("IllumHeadSub2").gameObject.SetActive(false);

		underWaterBoulderObj = (GameObject)Resources.Load("UnderwaterBoulderAttack");

		WildMoon2 = GameObject.Find("WildMoon2");
		WildMoon2.SetActive(false);
	}

	// Sets the order to 0 to make sure the waves render properly
	void SetOrderToZero(){
		transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = 0;
		transform.Find("Arms").GetComponent<SpriteRenderer>().sortingOrder = 0;
	}

	// Sets the order to the original positions
	void SetOrderToOriginal(){
		transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = bodyLayer;
		transform.Find("Arms").GetComponent<SpriteRenderer>().sortingOrder = armsLayer;
	}

	// Sets whether the boss can take damage
	void CanTakeDamage(bool b){
		canTakeDamage = b;
	}

	// Spawns a WaterSpiritAttack
	void LaunchWaterSpiritAttack(){
		GameObject g = (GameObject)Instantiate(WaterSpiritAttack);
		g.transform.position = transform.Find("AttackSpawn").position;
	}
	void Flip(float direction){
		Vector3 v = transform.localScale;
		v.x = Mathf.Abs(v.x)*direction;
		transform.localScale = v;
	}

	// Sets the pyramids loose on the player
	void PyramidBeam(){
		moveToPyramidPositionTimer += Time.deltaTime;
		if (moveToPyramidPositionTimer <= maxMoveToPyramidPositionTimer){
			CanTakeDamage(false);
			transform.position = BezierPosition(upperPoint.position + new Vector3(0f, -7f, 3f), upperPoint.position + new Vector3(0f, -10f, 10f),
				SpawnDestination, moveToPyramidPositionTimer / maxMoveToPyramidPositionTimer);
		}
		// Starts attack briefly after getting everything in position
		if (moveToPyramidPositionTimer > maxMoveToPyramidPositionTimer + .2f){
			if (pyramidBeamTimer <= 0f){
				desiredCamPosition = waterAttackDesiredCamPosition;
				// Moves one head to the top left of the screen
				IllumHead1.GetComponent<IllumHead>().StartMoveAway();
				IllumHead1.GetComponent<IllumHead>().awayDest = farPoint.transform.position + new Vector3(2f, 5f, 0f);
				IllumHead1.GetComponent<IllumHead>().nextPoint = upperPoint.position;
				IllumHead1.GetComponent<IllumHead>().finalAttackDirection = Vector3.right * -1f;
				// Moves one head to the Bottom Right of the screen
				IllumHead2.GetComponent<IllumHead>().StartMoveAway();
				IllumHead2.GetComponent<IllumHead>().awayDest = SpawnDestination + new Vector3(2f, 0f, 0f);
				IllumHead2.GetComponent<IllumHead>().nextPoint = new Vector3(upperPoint.position.x, SpawnDestination.y, SpawnDestination.z);
				IllumHead2.GetComponent<IllumHead>().finalAttackDirection = Vector3.right * 1f;
			}
			pyramidBeamTimer += Time.deltaTime;
			// Ends attack
			if (pyramidBeamFinished){
				returnFromPyramidTimer += Time.deltaTime;
				transform.position = BezierPosition(SpawnDestination, upperPoint.position + new Vector3(0f, -10f, 10f),
					upperPoint.position + new Vector3(0f, -7f, 3f), returnFromPyramidTimer / maxReturnFromPyramidTimer);
				if (returnFromPyramidTimer >= maxReturnFromPyramidTimer){
					returnFromPyramidTimer = 0f;
					desiredCamPosition = defaultDesiredCamPosition;
					moveToPyramidPositionTimer = 0f;
					pyramidBeamTimer = 0f;
					pyramidBeamFinished = false;

					IllumHead1.GetComponent<IllumHead>().EndAttack();
					IllumHead2.GetComponent<IllumHead>().EndAttack();
					EndAttack();
					CanTakeDamage(true);
				}
			}
		}
	}

	// Launches beams of water
	void WaterSpiritAttackController(){
		waterAttackTimer += Time.deltaTime;
		if (waterAttackTimer >= waterAttackInterval){
			desiredCamPosition = waterAttackDesiredCamPosition;
			LaunchWaterSpiritAttack();
			waterAttackInterval += waterAttackIntervalIncr;
		}
		// Move towards the left
		if (waterAttackTimer < maxWaterAttackTimer / 2f){
			if (theHand.transform.position.x < transform.position.x){
				Flip(-1f);
			} else {
				Flip(1f);
			}
			transform.position = BezierPosition(farPoint.position, upperPoint.position, SpawnDestination, waterAttackTimer / (maxWaterAttackTimer / 2f));
		} else if (waterAttackTimer > maxWaterAttackTimer / 2f){
			if (theHand.transform.position.x < transform.position.x){
				Flip(-1f);
			} else {
				Flip(1f);
			}
			transform.position = BezierPosition(SpawnDestination, upperPoint.position, farPoint.position, (waterAttackTimer - (maxWaterAttackTimer/2f)) / (maxWaterAttackTimer / 2f));
		}
		// Ends the attack
		if (waterAttackTimer > maxWaterAttackTimer){
			Flip(-1f);

			desiredCamPosition = defaultDesiredCamPosition;
			waterAttackInterval = startWaterAttackInterval;
			waterAttackTimer = 0f;
			EndAttack();
		}
	}

	// Force ends an attack to move to the next phase, cleaning up all attack-finishing code
	void ForceEndAttack(){
		Flip(-1f);

		desiredCamPosition = defaultDesiredCamPosition;
		waterAttackInterval = startWaterAttackInterval;
		waterAttackTimer = 0f;

		CanTakeDamage(true);
		largeWaveTimer = 0f;

		returnFromPyramidTimer = 0f;
		desiredCamPosition = defaultDesiredCamPosition;
		moveToPyramidPositionTimer = 0f;
		pyramidBeamTimer = 0f;
		pyramidBeamFinished = false;

		IllumHead1.GetComponent<IllumHead>().EndAttack();
		IllumHead2.GetComponent<IllumHead>().EndAttack();
		EndAttack();
	}

	// Ends an attack
	void EndAttack(){
		desiredCamPosition = defaultDesiredCamPosition;
		attackDelay = 0f;
		AttackMode = (AttackMode + 1) % AttackModes.Length;
		CanTakeDamage(true);
	}

	// Handles the LargeWave attack
	// Enemy goes down, then comes up quickly and brings a large wave with it
	void LargeWave(){
		//print(largeWaveTimer);
		if (largeWaveTimer == 0f){
			preAttackPosition = transform.position;
			spawnedWave = false;
		}
		// Moves down into the water
		if (largeWaveTimer < downWaveTimer){
			Vector3 bezMid = preAttackPosition + new Vector3(-3f, 0f, 0f);
			Vector3 tempPos = BezierPosition(WaveDownPoint, bezMid, preAttackPosition, largeWaveTimer / downWaveTimer);
			transform.position = tempPos;
			if (transform.position.y < theHand.position.y){
				CanTakeDamage(false);
			}
			//transform.position = Vector3.Lerp(preAttackPosition, WaveDownPoint, largeWaveTimer / downWaveTimer);
		// Moves backwards to where the enemy will pop up from the water
		} else if (largeWaveTimer < moveBackWaveTimer){
			SetOrderToZero();
			CanTakeDamage(false);
			Vector3 backPoint = WaveDownPoint + new Vector3(0f, 0f, 19f);
			transform.position = Vector3.Lerp(WaveDownPoint, backPoint, (largeWaveTimer-downWaveTimer) / (moveBackWaveTimer-downWaveTimer));
			if (largeWaveTimer > moveBackWaveTimer - 2f){
				Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
			}
		// Flies up, spawning a wave
		} else if (largeWaveTimer < moveUpWaveTimer){
			Vector3 backPoint = WaveDownPoint + new Vector3(0f, 0f, 19f);
			transform.position = Vector3.Lerp(backPoint, WaveUpPoint, (largeWaveTimer-moveBackWaveTimer) / (moveUpWaveTimer-moveBackWaveTimer));
			if (!spawnedWave && largeWaveTimer > moveBackWaveTimer + .2f){
				spawnedWave = true;
				GameObject g = (GameObject)Instantiate(WaveGroup);
			}
		// Idles in the air
		} else if (largeWaveTimer < waitUpWaveTimer){
		
		// Returns 
		} else if (largeWaveTimer < returnWaveTimer){
			SetOrderToOriginal();
			transform.position = Vector3.Lerp(WaveUpPoint, SpawnDestination, (largeWaveTimer-waitUpWaveTimer) / (returnWaveTimer-waitUpWaveTimer));
		}
		// Ends the attack
		else if (largeWaveTimer > returnWaveTimer){
			CanTakeDamage(true);
			largeWaveTimer = 0f;
			EndAttack();
		}
			
		largeWaveTimer += Time.deltaTime;
	}

	// Shoots blood everywhere when hit
	public override void Injured(){
		float direction = transform.localScale.x;
		direction = (direction > 0) ? 1f : -1f;
		for (int i = 0; i < 12; i++){
			GameObject b = Instantiate(blood);
			Vector3 tempPosition = transform.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += UnityEngine.Random.Range(-1f, 1f);
			tempPosition.y += UnityEngine.Random.Range(-3f, 2f);
			tempPosition.z += UnityEngine.Random.Range(-1f, 1f);
			tempAngles.z = UnityEngine.Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= UnityEngine.Random.Range(.5f, 1f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(1f, 5f)*direction, UnityEngine.Random.Range(-3f, 1f));
		}
	}

	// Moves the camera out
	void MoveCameraOut(){
		Vector3 camPos = Camera.main.transform.position;
		Vector3 destPos = new Vector3(camPos.x, camPos.y, desiredCamPosition.z);
		float zoomSpeed = 2f;
		Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, destPos, zoomSpeed*Time.deltaTime);
	}

	// Initial appearance of the monster
	void AppearOnScreen(){
		// Keeps out of the way if the player hasn't yet reached the scene
		if (!activated){
			// Moves the Y Border up a bit
			if (!movedYBorder){
				GameObject.Find("YBorder").transform.position += new Vector3(0f, 8f, 0f);
				movedYBorder = true;
			}
			transform.position = PreActivePosition;
			if (Camera.main.GetComponent<CameraTarget>().GetCurrentPlayer().position.x > ActivationPoint.x){
				AddFastText(DialogueTable.GetChat("waterspiritappear").text, -200f, 100f, true, true);
				transform.position = Spawn;
				activated = true;
			}
			return;
		} else if (activated && !spawnComplete) {
			if (!reachedSpawnPoint){
				transform.position += Time.deltaTime * spawnVelocity;
				if (transform.position.x > SpawnDestination.x){
					reachedSpawnPoint = true;
				}
				// Negatively accelerate and move backwards
			} else {
				transform.position += Time.deltaTime * spawnVelocity;
				spawnVelocity -= Time.deltaTime * reduceSpawnVelocity;
				// At the final destination for the spawn
				if (spawnVelocity.x < 0f && transform.position.x < SpawnDestination.x){
					AddFastText(DialogueTable.GetChat("waterspiritappear2").text, -200f, 100f, true, true);
					spawnComplete = true;
				}
			}
		} 
		/*
		else if (spawnComplete && !stopSpawnMoving){
			spawnVelocity = Vector3.MoveTowards(spawnVelocity, Vector3.zero, 50f*Time.deltaTime);
			transform.position += Time.deltaTime* spawnVelocity;
			if (spawnVelocity.x == 0f){
				stopSpawnMoving = true;
			}
		}
		*/
		MoveCameraOut();
	}

	// Makes the enemy bigger for the final phase of battle
	public void Enlarge(){
		transform.localScale *= 1.5f;
		RepeatingUnderwaterObj.SetActive(true);
	}

	// Sets to being in the proper escape position
	public void SetEscapeCenterPoint(Vector3 p){
		escapeAttackActive = true;
		escapeCenterPoint = p;
	}

	// Moves away
	void Escape(){
		if (escaping){
			escapeTimer += Time.deltaTime;
			// Moves along a curve to the point
			transform.position = BezierPosition(escapePoint, escapePoint + new Vector3(-2f, -15f, 0f), SpawnDestination, escapeTimer / maxEscapeTimer);
			if (escapeTimer >= maxEscapeTimer){
				// Activates the other heads
				transform.Find("IllumHeadSub1").gameObject.SetActive(true);
				transform.Find("IllumHeadSub2").gameObject.SetActive(true);
				// Deactivates the current heads
				transform.Find("IllumHeadEnemy").gameObject.SetActive(false);
				transform.Find("IllumHeadEnemy2").gameObject.SetActive(false);

				escaping = false;
			}
		}
		// Moves up and down in a sine wave during the attack
		else {
			if (escapeAttackActive){
				transform.position = escapeCenterPoint + new Vector3(2f * Mathf.Cos(moveAngle - 90f*Mathf.Deg2Rad), 6f * Mathf.Sin(moveAngle * 1.5f), 0f);
				moveAngle += Time.deltaTime;

				// Unleashes boulder attacks
				escapeAttackTimer += Time.deltaTime;
				if (escapeAttackTimer > escapeAttackInterval){
					escapeAttackInterval += escapeAttackIntervalIncr;
					GameObject g = (GameObject)Instantiate(underWaterBoulderObj);
					g.transform.position = transform.position + new Vector3(14f, Random.Range(-5f, 5f), 0f);
				}
			}
		}
	}

	// Escapes once life is below a certain point
	void StartEscape(){
		ForceEndAttack();
		// Just use the Spawn Destination here as the value from which we'll move since it won't be used anymore
		SpawnDestination = transform.position;
		startedEscape = true;
		escaping = true;
		AddFastText(DialogueTable.GetChat("waterspiritescape").text, -200f, 100f, true, true);
		SubmarineObj.SetActive(true);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("m")){
			//life = 22f;
		}
		// Stop moving once life is under 0 and let the Submarine handle the death scene
		if (life <= 0f){
			SubmarineObj.GetComponent<GenericEnemy>().life = 20f;
			transform.Find("IllumHeadSub1").gameObject.SetActive(false);
			transform.Find("IllumHeadSub2").gameObject.SetActive(false);
			WildMoon2.SetActive(true);
			return;
		}
			
		//print(startedDestruction);
		if (startedDestruction){
			gameObject.SetActive(false);
			if (!preparedToDie){
				preparedToDie = true;
				//theHand.GetComponent<HandController>().EatObject(transform, true);
				//theHand.GetComponent<HandController>().Freeze(10f);
			}
			return;
		}
		damageTimer -= Time.deltaTime;

		// Escapes once life is low
		if (startedEscape){
			Escape();
			return;
		}

		// Typical attack cycle
		if (spawnComplete){
			// Idle around when not attacking
			if (attackDelay <= maxAttackDelay){
				ang += Time.deltaTime;
				Vector3 tempPos = startPos + new Vector3(radius * -Mathf.Cos(ang*2f), radius * -Mathf.Sin(4*ang), 0f);
				transform.position = tempPos;

				attackDelay += Time.deltaTime;
			// Call the proper attack function
			} else {
				if (AttackModes[AttackMode] == LARGEWAVE){
					LargeWave();
				} else if (AttackModes[AttackMode] == WATERSPIRIT){
					WaterSpiritAttackController();
				} else if (AttackModes[AttackMode] == PYRAMIDBEAM){
					PyramidBeam();
				}
			}
		}
		// Starts an escape once its life is in half
		// NOTE: death code will be handed by the sub, not by the WaterSpirit
		if (life < (maxLife * .55f) && startedEscape == false){
			StartEscape();
			/*
			startedDestruction = true;
			GameObject g = GameObject.FindGameObjectWithTag("Player");
			if (g.GetComponent<HandController>()){
				g.GetComponent<HandController>().Depossess();					
			} else {
				g.GetComponent<PossessableScript>().Depossess();
			}
			*/
			//theHand.GetComponent<HandController>().EatObject(transform, true);
		}
		AppearOnScreen();
	}
}
