using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;

public class NetworkHandController : NetworkGenericEnemy {
	
	private Animator m_Anim;            // Reference to the player's animator component.
	public Rigidbody2D rb;
	Transform groundCheck;

	// Period of time in which controls are locked
	float controlLockTimer = 0f;

	/* How fast the hand moves on the ground */
	public float walkSpeed;
	// How fast the hand decelerates when no longer walking
	public float walkDeceleration = .9f;
	bool grounded = true;
	
	public Sprite sourceTex;
	public Rect sourceRect;
	
	// Keep track of the camera
	public GameObject[] mainCams;
	
	// Points on the hand (useful as pivots for rotation, etc)
	private Transform bottomPoint;
	private Transform topPoint;
	private Transform leftPoint;
	private Transform rightPoint;
	// The portion that reaches out to grab objects
	private Transform grabPivot;
	
	// For dealing with grabbing
	private float maxGrabTimer = 6f;
	private float grabTimer;
	private float grabVelocity;
	private Vector3 grabPositionVelocity;
	private bool grabbing = false;
	private float grabScaleY = 0f;
	private bool grabFailed = true;
	private bool grabReturn = false;
	public GameObject grabbedObject = null;
	
	// For checking the hand's position when grabbing, etc
	private Transform handCheck;
	private float handCheckRadius = .5f;
	[SerializeField] private LayerMask m_WhatIsGround;	// A mask determining what is grabbable to the character
	
	// Record of recent possessions, and the max number the player can possess (your possession history)
	private int possessionMemoryLimit = 4;
	public ArrayList possessionMemory = new ArrayList();
	
	// All objects that can be possessed
	public GameObject[] possessables;
	// All objects that are interactive
	public GameObject[] interactables;
	// Object we're hovering over
	GameObject hoveredObject = null;
	SpriteRenderer hoveredObjectRenderer = null;
	GameObject hoveredObjectReference = null;


	// Cloud rendered behind player
	GameObject cloud;
	GameObject cloud2;
	Vector4 cloudColor;
	Vector3 cloudSize;
	Vector4 maxCloudColor;
	Vector3 maxCloudSize;
	
	private bool alreadyShattered = false;

	// Object that displays behind the target when Possessing or Eating them
	GameObject backPartRender;
	SpriteRenderer sr;
	SpriteRenderer backSr;

	// The yellow (eye) layer which we manually update to give a nice glow
	SpriteRenderer yellowSprite;

	// The Action Menu
	public GameObject AttackMenu;
	// Eat button of the attack menu (disabled unless we can eat)
	GameObject AttackMenuEatButton;
	// Possess button of the attack menu (disabled unless we can possess)
	GameObject AttackMenuPossessButton;

	GameObject PossessedAttackMenu;
	Transform selectedTarget = null;
	int attackMenuCloseDelay = 0;
	// Locked position for the AttackMenu when it's not targetting anything
	Vector3 attackMenuLockedPosition = new Vector3(0f, 0f, 0f);

	// Crosshair. Follows a selected target, or just shows a no selection icon
	Transform crosshair;
	public Sprite crosshairSelectionImg;
	public Sprite crosshairNoSelectionImg;
	public Sprite crosshairGrabImg;
	public Sprite crosshairNullImg;


	// Stats that display above an enemy when hovered
	GameObject enemyStats;
	Text enemyIQText;
	Text enemyRankText;
	Text enemyInfoText;

	// Complete list of all entity types we've possessed
	public ArrayList hasPossessedBefore = new ArrayList();

	// no clue what this is for lmao
	int timer = 0;

	//public Sprite backEatSprite;
	//public Vector2 backEatSpriteCount;
	Hashtable backEatIndices;
	public Sprite[] backEatSprites;
	Hashtable backPossessIndices;
	public Sprite[] backPossessSprites;

	// Gets the cursor for reference
	Transform cursor;

	// Target point for wrapping around humans
	Transform wrapAroundPoint;

	// Blood spatter from eating & killing
	GameObject bloodSpatter;
	// Goo spatter when the hand is injured
	GameObject handSpatter;

	// The player's EXP and level(IQ)
	// Player skills go up as skills are learned
	// IQ 100: 		Player can now eat
	// IQ 200: 		Player can now possess humans
	int exp = 0;
	public int iq = 1;

	// Default scale to return to after exiting possession
	Vector3 defaultScale;

	// Objects that've made noise and are over a certain sound threshold (or things that are moving, like toilets)
	ArrayList suspiciousTargets = new ArrayList();

	// We'll LERP between these values to determine the eye color
	Color fullEyeColor = new Color(255f/255f, 204f/255f, 94f/255f, 255f/255f);
	Color minEyeColor = new Color(255f/255f, 26f/255f, 230f/255f, 255f/255f);
	// Eye expands when damaged
	Vector3 eyeBaseScale = new Vector3(1f, 1f, 1f);
	Vector3 eyeDamagedScale = new Vector3(2f, 2f, 2f);
	float eyeReductionRate = 2.1f;

	// The gravity vector
	Vector3 defaultGravitationalForce = new Vector3(0f, -9.81f*3f, 0f);
	Vector3 gravitationalForce;
	float defaultGravityScale = 3f;

	// For sticking to walls
	float stickTimer = 0f;
	float maxStickTimer = .75f;
	Vector3 defaultUpVector = new Vector3(0f, 1f, 0f);
	Vector3 outwardVector = new Vector3(0f, 0f, 0f);

	// Locks the controls of the player
	public float freezeTimer = 0f;

	// List of keys the player is holding
	ArrayList keys = new ArrayList();

	// If there's a nearby enemy that can be eaten/possessed, set this flag as true
	bool immediatelyEdiblePossessibleEnemy = false;
	GameObject possessionCloud;

	// If we just selected a menu action, set a flag to prevent from accidentally grabbing in the same frame
	bool justSelectedMenuAction = false;
	
	//public bool inBackgroundLayer = false;				// Determines whether the player is currently slinking past in the background or not. If so, ignore enemy collisions
	float preBackgroundTransitionZ = 0f;		// Depth of the player before transition to the background
	float backgroundTransitionTimer = 0f;
	float maxBackgroundTransitionTimer = .3f;

	// The range for Human Possession/Eating
	GameObject rangeCircle;
	Vector3 rangeScale;
	float rangeRadius = 1f;
	float rangeExpandTimer = 0f;

	float closeMemoryMenuTimer = 0f;
	float memoryMenuOpenTimer = 0f;

	// The object we're set to possess after the stretch process is complete
	GameObject setToPossess;
	float waitToPossessTimer = 0f;
	float maxWaitToPossessTimer = .2f;
	AudioClip goopSound;
	AudioClip chompishSound;

	// If pulling an object from memory, keep a note of what it is. It'll despawn once left
	GameObject memorizedPossession;
	// Timer to limit how long we can possess something
	public float memoryTimer = 15f;
	public float maxMemoryTimer = 10f;
	float memoryRestoreRate = 3f;

	GameObject memoryMenu;

	// The form that the hand is currently in. This can be the hand itself, or the object it becomes
	public Transform currentObjectForm;

	// Loads the checkpoint data
	void LoadCheckpointPosition(){
		Vector3 pos = CheckpointData.LoadCheckpoint();
		if (pos.x != 0f && pos.y != 0f){
			transform.position = pos;
		}
	}

	// A menu action was just selected
	public void JustSelectedMenuAction(){
		justSelectedMenuAction = true;
	}

	// Passes control to a subobject once we possess it
	public void PassControl(){

	}

	// Opens the memory menu, allowing the player to transform into recent objects
	public void ActivateMemoryMenu(Vector3 position){
		memoryMenu = (GameObject)Resources.Load("MemoryMenu");
		memoryMenu = (GameObject)Instantiate(memoryMenu);
		memoryMenu.transform.position = position;
		memoryMenuOpenTimer = .1f;
		int currentPosition = 0;
		Vector3[] iconPositions =  {new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f), new Vector3(-1f, 0f, 0f)};
		// Determines what objects have been used and enable them on the memory menu
		for (int i = 0; i < possessionMemory.Count; i++){
			string name = (string)possessionMemory[i];
			foreach (Transform child in memoryMenu.transform){
				if (child.name.Contains(name) || name.Contains(child.name)){
					child.gameObject.SetActive(true);
					child.localPosition = iconPositions[currentPosition];
					currentPosition++;
				}
			}
		}
	}

	// Destroys the memory menu
	public void DeactivateMemoryMenu(){
		NormalTime();
		memoryMenu.SetActive(false);
		Destroy(memoryMenu, 0f);
		memoryMenu = null;
		closeMemoryMenuTimer = 0f;
	}

	// Determines whether the player is currently hidden
	public override bool IsHidden(){
		return (inBackgroundLayer);
	}

	// Swaps between the front and back planes of the game
	public void SwapPlanes(float depthTransition){
		if (backgroundTransitionTimer > 0f){
			return;
		}
		backgroundTransitionTimer = maxBackgroundTransitionTimer;
		if (!inBackgroundLayer){
			Vector3 tempPos = transform.position;
			preBackgroundTransitionZ = transform.position.z;
			tempPos.z = transform.position.z + depthTransition;
			transform.position = tempPos;
			inBackgroundLayer = true;
			gameObject.layer = LayerMask.NameToLayer("BackgroundLayer");
		} else {
			Vector3 tempPos = transform.position;
			tempPos.z = preBackgroundTransitionZ;
			transform.position = tempPos;
			inBackgroundLayer = false;
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	// Sets an enemy to be possessed later (actual implementation handled within NetworkPossessableScript -- this is just a reference to the obj)
	public void SetToPossess(GameObject g){
		setToPossess = g;
	}

	// Checks whether controls are locked
	public bool ControlsLocked(){
		return (controlLockTimer > 0f || m_Anim.GetBool("Possessing") || m_Anim.GetBool("Eating"));
	}

	// Gathers sprite sets for the back renderer
	void GatherBackSprites(){
		backEatIndices = new Hashtable();
		for (int i = 0; i < backEatSprites.Length; i++){
			backEatIndices.Add("hand_eat_above_"+i, i);
		}
		backPossessIndices = new Hashtable();
		for (int i = 0; i < backPossessSprites.Length; i++){
			backPossessIndices.Add("hand_possess_"+i, i);
		}
		/*
		backEatSprites = new Sprite[backEatSpriteCount.y][backEatSpriteCount.x];
		for (int y = 0; y < backEatSpriteCount.y; y++){
			for (int x = 0; x < backEatSpriteCount.x; x++){
				//backEatSprites[y][x] = new Sprite(
			}
		}
		*/
	}

	// Overrides damage function to cause the eye to expand when hit
	public override void DelayedDamage(float dmg){
		base.DelayedDamage(dmg);
		yellowSprite.transform.localScale = eyeDamagedScale;
	}

	// Sets the flag that determines whether a nearby edible/possessible enemy is present
	public void SetNearbyEdiblePossessibleEnemy(){
		immediatelyEdiblePossessibleEnemy = true;
	}

	// Startup shit
	private void Awake(){
		handCheck = transform.Find("HandCheck");
		bottomPoint = transform.Find("BottomPoint");
		grabPivot = transform.Find("GrabPivot");
		cloud = transform.Find("Cloud").gameObject;
		cloud2 = transform.Find("Cloud2").gameObject;
		backPartRender = transform.Find("BackPartRenderer").gameObject;
		m_Anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		//AttackMenu = transform.Find("AttackMenu").gameObject;
		AttackMenu = GameObject.Find("HandAttackMenu").gameObject;
		AttackMenuEatButton = AttackMenu.transform.Find("EatIcon").gameObject;
		AttackMenuPossessButton = AttackMenu.transform.Find("PossessIcon").gameObject;

		//PossessedAttackMenu = transform.Find("PossessedAttackMenu").gameObject;
		PossessedAttackMenu = GameObject.Find("PossessedAttackMenu");
		PossessedAttackMenu.SetActive(false);
		enemyStats = GameObject.Find("EnemyStats");
		enemyIQText = enemyStats.transform.Find("IQ").GetComponent<Text>();
		enemyRankText = enemyStats.transform.Find("Rank").GetComponent<Text>();
		enemyInfoText = enemyStats.transform.Find("Info").GetComponent<Text>();

		sr = GetComponent<SpriteRenderer>();
		backSr = backPartRender.GetComponent<SpriteRenderer>();
		yellowSprite = transform.Find("YellowLayer").GetComponent<SpriteRenderer>();

		wrapAroundPoint = transform.Find("WraparoundPoint");
		defaultScale = transform.localScale;

		UnityEngine.Cursor.visible = false;

		bloodSpatter = (GameObject)Resources.Load("BloodSpatter");
		handSpatter = (GameObject)Resources.Load("HandSpatter");

		cursor = GameObject.Find("cursor").transform;

		GatherBackSprites();
	}
	// Use this for initialization
	void Start () {
		grabTimer = 0;

		groundCheck = transform.Find("GroundCheck");
		
		mainCams = GameObject.FindGameObjectsWithTag("MainCamera");
		GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSprite"));

		// Get and separate the hovered object from the Hand
		hoveredObject = transform.Find("HoveredObject").gameObject;
		hoveredObject.transform.parent = null;
		hoveredObjectRenderer = hoveredObject.GetComponent<SpriteRenderer>();

		// For crosshair shit
		crosshair = GameObject.Find("Crosshair").transform;
		crosshair.SetParent(null);

		// Handles gravity settings
		gravitationalForce = defaultGravitationalForce;
		//rb.gravityScale = 0f;
		defaultGravityScale = rb.gravityScale;

		possessionCloud = transform.Find("PossessionCloud").gameObject;

		// For modifying the cloud (displays hand emotion)
		cloudColor = (Vector4) cloud.GetComponent<SpriteRenderer>().color;
		maxCloudColor = new Vector4(1f, .8f, .1f, 1f);
		cloudSize = cloud.transform.localScale;
		maxCloudSize = cloudSize * 1.3f;

		// Loads the range circle
		GameObject g = (GameObject)Resources.Load("RangeCircle");
		rangeCircle = (GameObject)Instantiate(g);
		rangeCircle.transform.position = transform.position;
		rangeCircle.transform.parent = transform;
		rangeScale = rangeCircle.transform.localScale;

		chompishSound = Resources.Load("Audio/chomp-ish") as AudioClip;
		goopSound = Resources.Load("Audio/goopwrap") as AudioClip;

		currentObjectForm = transform;

		// Loads the player at the checkpoint position
		LoadCheckpointPosition();
	}
	void GetPossessables(){
		possessables = GameObject.FindGameObjectsWithTag("Possessable");
	}

	// Sets the hovered object's reference
	public void SetHoveredObjectReference(GameObject o){
		hoveredObjectReference = o;
	}

	// Freezes the player's controls for a period of time
	public void Freeze(float t){
		if (freezeTimer <= t){
			freezeTimer = t;
		}
	}

	// Toggles time slowing
	public void SlowTime(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(.1f);
	}
	public void NormalTime(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}

	// Gets the state of the grab's return
	public bool GrabReturning(){
		return grabReturn;
	}

	// Get Possessed Attack Menu
	public GameObject getPossessedAttackMenu(){
		return PossessedAttackMenu;
	}

	// Gets the player's Selected Target
	public Transform GetSelectedTarget(){
		return selectedTarget;
	}

	// Sets the attack menu to be on
	void ActivateAttackMenu(){
		if (Input.GetMouseButtonDown(1) && !(m_Anim.GetBool("Eat") || m_Anim.GetBool("Possessing") || m_Anim.GetBool("Grabbing"))){
			// Gets cursor position
			Vector3 cursorPoint = Input.mousePosition;
			cursorPoint.z = transform.position.z - mainCams[0].transform.position.z;
			cursorPoint = Camera.main.ScreenToWorldPoint(cursorPoint);

			// Checks if a possessable object is being touched
			grabbedObject = null;

			for (int i = 0; i < possessables.Length; i++){
				GameObject p = possessables[i];
				if (p.GetComponent<NetworkPossessableScript>() && p.GetComponent<NetworkPossessableScript>().CanPossess()){
					grabbedObject = p;
					break;
				}
			}

			// Sees what object is selected and lets the player determine its actions from it
			//print(hoveredObject);

			// Enables the Attack Menu
			if (grabbedObject != null){
				Transform target = grabbedObject.transform;
				NetworkPossessableScript grabbedPs = grabbedObject.GetComponent<NetworkPossessableScript>();
				if (grabbedObject.GetComponent<NetworkPossessableScript>().possessionPoint != null){
					target = grabbedObject.GetComponent<NetworkPossessableScript>().possessionPoint;
				}
				bool clearPath = ClearPathToTarget(target.position);
				if (clearPath){
					SlowTime();
					selectedTarget = target.transform;
					SetAttackMenuStatus(true);
					crosshair.GetComponent<SpriteRenderer>().sprite = crosshairSelectionImg;
				}
				// If not yet able to eat, hide the button; if able to eat but not this enemy, make it transparent
				if (iq < 100){
					AttackMenuEatButton.SetActive(false);
				} else {
					AttackMenuEatButton.SetActive(true);
					if (grabbedObject.GetComponent<NetworkPossessableScript>().edible){
						AttackMenuEatButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
					} else {
						AttackMenuEatButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .25f);
					}
				}
				// Determines the visibility status of the button for possessions
				if (grabbedPs.possessable && (!grabbedPs.isHuman || (grabbedPs.isHuman && iq >= grabbedPs.iq))){
					AttackMenuPossessButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				} else {
					AttackMenuPossessButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .25f);
				}

			// Exits the attack menu if you're in it
			} else if (AttackMenu.activeSelf){
				attackMenuCloseDelay = 3;

			// Enables attack menu, albeit with no object selected
			} else if (!AttackMenu.activeSelf && grabbedObject == null){
				selectedTarget = null;
				SetAttackMenuStatus(true);
				SlowTime();
				//AttackMenu.transform.SetParent(null, false);
				AttackMenuPossessButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .25f);
				AttackMenu.transform.position = cursorPoint;
				crosshair.position = cursorPoint;
				crosshair.GetComponent<SpriteRenderer>().sprite = crosshairNoSelectionImg;
				attackMenuLockedPosition = cursorPoint;
			}

			if (grabbedObject == null){
				AttackMenuEatButton.SetActive(false);
				AttackMenuPossessButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .25f);
			}

		} else if (Input.GetMouseButton(0)){
			if (AttackMenu.activeSelf){
				attackMenuCloseDelay = 3;
			}
		// Closes the attack menu
		} else if (attackMenuCloseDelay >= 0){
			attackMenuCloseDelay -= 1;
			if (attackMenuCloseDelay <= 0){
				NormalTime();
				SetAttackMenuStatus(false);
				crosshair.gameObject.SetActive(false);
			}
		}
		// Makes sure the attack menu faces the correct way
		if (AttackMenu.activeSelf){
			if (selectedTarget){
				crosshair.gameObject.SetActive(true);
				crosshair.transform.position = (selectedTarget.transform.position);
			// If there's no selected target, we need to manually make sure the menu doesn't shift around
			} else {
				AttackMenu.transform.position = attackMenuLockedPosition;
			}
			Vector3 tempScale = AttackMenu.transform.localScale;
			if (transform.localScale.x < 0 && tempScale.x > 0){
				//tempScale.x = -tempScale.x;
			} else if (transform.localScale.x > 0 && tempScale.x < 0){
				//tempScale.x = -tempScale.x;
			}
			AttackMenu.transform.localScale = tempScale;
		}
	}

	// Sets the yellow layer's sprite to be the player's current
	// Also updates its life display
	void SetYellowLayerSprite(){
		yellowSprite.sprite = sr.sprite;
		// Eye changes color to display current life
		yellowSprite.material.color = (Color)Vector4.Lerp(minEyeColor, fullEyeColor, ((float)life)/((float)maxLife));

		// Scales eye back down when hit
		yellowSprite.transform.localScale = Vector3.MoveTowards(yellowSprite.transform.localScale, eyeBaseScale, eyeReductionRate*Time.deltaTime);
	}

	void DisplayEatPossessionGlow(){
		if (immediatelyEdiblePossessibleEnemy){
			//possessionCloud.SetActive(true);
			cloud.transform.localScale = Vector3.MoveTowards(cloud.transform.localScale, maxCloudSize, 2f*Time.deltaTime);
			cloud.GetComponent<SpriteRenderer>().color = (Color)(Vector4.MoveTowards((Vector4)cloud.GetComponent<SpriteRenderer>().color, (Vector4)maxCloudColor, 2f*Time.deltaTime));
			immediatelyEdiblePossessibleEnemy = false;
		} else {
			cloud.transform.localScale = Vector3.MoveTowards(cloud.transform.localScale, cloudSize, 2f*Time.deltaTime);
			cloud.GetComponent<SpriteRenderer>().color = (Color)Vector4.MoveTowards(cloud.GetComponent<SpriteRenderer>().color, cloudColor, 2f*Time.deltaTime);
			//possessionCloud.SetActive(false);
		}
	}

	// Highlights possessable objects
	// Draws an outline around them
	void HighlightObject(){
		Transform grabbedObject = null;
		for (int i = 0; i < possessables.Length; i++){
			GameObject p = possessables[i];
			//if (p.GetComponent<NetworkPossessableScript>() && p.GetComponent<NetworkPossessableScript>().CanPossess()){
			if (p.GetComponent<NetworkPossessableScript>() && p.GetComponent<NetworkPossessableScript>().CanInteract()){
				grabbedObject = p.transform;
				break;
			}
		}
		if (grabbedObject && !m_Anim.GetBool("Possessing") && !m_Anim.GetBool("Eating")){
			enemyStats.SetActive(true);
			// If the object is a sprite, highlight it
			if (grabbedObject.gameObject.GetComponent<SpriteRenderer>()){
				hoveredObjectRenderer.sprite = grabbedObject.gameObject.GetComponent<SpriteRenderer>().sprite;
				//enemyStats.transform.position = new Vector3(hoveredObject.transform.position.x, hoveredObject.transform.position.y + hoveredObjectRenderer.sprite.bounds.max.y, transform.position.z);
				Vector3 tempEnemyStatsPos = new Vector3(cursor.position.x, cursor.position.y + 1f, transform.position.z);
				if (Input.mousePosition.y > Screen.height*.8f){
					tempEnemyStatsPos.y -= 2f;
				}
				enemyStats.transform.position = tempEnemyStatsPos;
			// If it's not a sprite, let it run a special function later
			} else {
				grabbedObject.gameObject.GetComponent<NetworkPossessableScript>().ActivateCustomOutline();
				//enemyStats.transform.position = new Vector3(hoveredObject.transform.position.x, hoveredObject.transform.position.y + hoveredObjectRenderer.sprite.bounds.max.y, transform.position.z);
				Vector3 tempEnemyStatsPos = new Vector3(cursor.position.x, cursor.position.y + 1f, transform.position.z);
				if (Input.mousePosition.y > Screen.height*.8f){
					tempEnemyStatsPos.y -= 2f;
				}
				enemyStats.transform.position = tempEnemyStatsPos;
			}
			SetHoveredObjectReference(grabbedObject.gameObject);
			hoveredObject.transform.position = grabbedObject.position;
			hoveredObject.transform.localScale = grabbedObject.localScale;
			hoveredObject.transform.eulerAngles = grabbedObject.eulerAngles;
			enemyIQText.text = "IQ: "+grabbedObject.gameObject.GetComponent<NetworkPossessableScript>().iq;
			enemyRankText.text = "Rank: "+grabbedObject.gameObject.GetComponent<NetworkPossessableScript>().rank;
			enemyInfoText.text = grabbedObject.gameObject.GetComponent<NetworkPossessableScript>().infoText;
			
		} else {
			hoveredObjectRenderer.sprite = null;
			SetHoveredObjectReference(null);
			enemyStats.SetActive(false);
		}
	}

	// Update'll run normally even when the game is slowed down (use Time.deltaTime for time-dependent events)
	void Update(){
		// Decreases freeze timer of the player (we might want to freeze him during events)
		if (freezeTimer >= 0f){
			freezeTimer -= Time.deltaTime;
			m_Anim.SetBool("Frozen", true);
			Vector3 tempVelocity = rb.velocity;
			tempVelocity.x = 0f;
			rb.velocity = tempVelocity;
			if (freezeTimer <= 0f){
				m_Anim.SetBool("Frozen", false);
			}
		}

		// Recovers the memory timer so we can redraw things from memory
		memoryTimer += Time.deltaTime * memoryRestoreRate;
		if (memoryTimer >= maxMemoryTimer){
			memoryTimer = maxMemoryTimer;
		}

		ManageBackgroundMovement();

		ManageStickiness(); 

		damageTimer -= Time.deltaTime;

		ManualBackAnimation();
		// Enable the attack menu and keep it targeted
		ActivateAttackMenu();
		if (selectedTarget != null){
			AttackMenu.transform.position = selectedTarget.position;
		}
		SetYellowLayerSprite();
		DisplayEatPossessionGlow();

		m_Anim.SetFloat("yVelocity", rb.velocity.y);

		// Gets cursor offset from z so that it's on the same plane as the player
		Vector3 cursorPoint = Input.mousePosition;
		cursorPoint.z = transform.position.z - mainCams[0].transform.position.z;
		cursorPoint = Camera.main.ScreenToWorldPoint(cursorPoint);

		// Triggers a grab if possible and if we did NOT just select a menu action
		//if (Input.GetMouseButtonDown(0) && !m_Anim.GetBool("Grabbing") && !grabbing && !AttackMenu.activeSelf && !justSelectedMenuAction){
		if (Input.GetMouseButtonDown(0) && !m_Anim.GetBool("Grabbing") && !grabbing /*&& (!AttackMenu.activeSelf)*/ && !justSelectedMenuAction){	
			ActivateGrab(cursorPoint);
		}

		ManageRangeCircle();
		ReduceControlLockTimer();

		// Closes memory menu if you click outside the range
		if (memoryMenu != null){
			// Maintains a consistent size for the memory menu
			float scale = 1f;
			scale = Mathf.Abs(Camera.main.transform.position.z - memoryMenu.transform.position.z) / 9f;
			memoryMenu.transform.localScale = new Vector3(scale, scale, scale);
			memoryMenuOpenTimer -= Time.unscaledDeltaTime;
			if (memoryMenuOpenTimer <= 0f){
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
					closeMemoryMenuTimer = .1f;
				}
				// Close the menu once the timer runs out after a click
				if (closeMemoryMenuTimer > 0f){
					closeMemoryMenuTimer -= Time.unscaledDeltaTime;
					if (closeMemoryMenuTimer <= 0f){
						DeactivateMemoryMenu();
					}
				}
			}
		}
	}

	// Manages the range circle
	public void ManageRangeCircle(){
		// Keeps range circle facing the right way
		Vector3 tempRangeCircleScale = rangeCircle.transform.localScale;
		if (transform.localScale.x < 0f){
			tempRangeCircleScale.x = -Mathf.Abs(tempRangeCircleScale.x);
		} else {
			tempRangeCircleScale.x = Mathf.Abs(tempRangeCircleScale.x);
		}
		rangeCircle.transform.localScale = tempRangeCircleScale;
		rangeRadius = Vector3.Distance(transform.position, rangeCircle.transform.Find("Distance").position);

		if (GetAttackMenuStatus()){
			rangeExpandTimer += Time.deltaTime;
			rangeCircle.transform.localScale = Vector3.Lerp(Vector3.zero, rangeScale, rangeExpandTimer*30f);
		}
	}

	// Player is frozen
	public bool IsFrozen(){
		return (freezeTimer > 0f);
	}

	// For managing movement to the background layer
	void ManageBackgroundMovement(){
		backgroundTransitionTimer -= Time.deltaTime;
	}

	// Manages sticking to surface
	void ManageStickiness(){
		if (stickTimer > 0f){
			stickTimer -= Time.deltaTime;
			transform.up = outwardVector;
			Collider2D[] cols = Physics2D.OverlapCircleAll(groundCheck.position, .15f);
			bool onWall = false;
			foreach (Collider2D col in cols){
				if (!col.isTrigger && !(col.tag == "Player" || col.tag == "PlayerChild")){
					onWall = true;
				}
			}
			if (!onWall && stickTimer < (maxStickTimer * .8f)){
				Unstick();
			}

			if (stickTimer <= 0f){
				Unstick();
			}
		} else {
			transform.up = Vector3.RotateTowards(transform.up, defaultUpVector, 20f*Time.deltaTime, 0f);
		}
	}
	// Makes the player stick to a surface
	public void StickToSurface(Vector3 up){
		stickTimer = maxStickTimer;
		transform.up = up;
		outwardVector = up;
		rb.gravityScale = 0f;
		rb.velocity = new Vector2(0f, 0f);
	}
	// Unsticks from a surface 
	public void Unstick(){
		stickTimer = 0f;
		rb.gravityScale = defaultGravityScale;
	}

	// Handles manual animation of the back part
	void ManualBackAnimation(){
		if (m_Anim.GetBool("Eating")){
			if (backEatIndices[sr.sprite.name] != null){
				int spriteIndex = (int)backEatIndices[sr.sprite.name];
				backSr.sprite = backEatSprites[spriteIndex];
			}
		} else if (m_Anim.GetBool("Possessing")){
			if (backPossessIndices[sr.sprite.name] != null){
				int spriteIndex = (int)backPossessIndices[sr.sprite.name];
				backSr.sprite = backPossessSprites[spriteIndex];
			}
		}
	}

	// Levels up the IQ
	public void IncreaseIQ(int incr){
		iq += incr;
	}

	/// <summary>
	/// Sets IQ to a certain value. If val is lower than current IQ, this does nothing
	/// </summary>
	/// <param name="val">Value.</param>
	public void SetIQ(int val){
		if (val > iq){
			iq = val;
		}
	}

	// Activates the level up effect
	public void LevelUpEffect(){
		transform.Find("LevelUp").gameObject.SetActive(true);
	}

	// FixedUpdate is called once per frame
	void FixedUpdate () {
		// Manages gravity
		if (rb.gravityScale <= 0f){
			//rb.velocity += (Vector2)(gravitationalForce * Time.fixedDeltaTime);
		}

		// Updates arraylist with objects which can be possessed
		GetPossessables();

		HighlightObject();
		
		bool isMoving = false;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
			isMoving = true;
			//GetComponent<Rigidbody2D>().velocity = new Vector2(walkSpeed, GetComponent<Rigidbody2D>().velocity.y);
		} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
			isMoving = true;
			//GetComponent<Rigidbody2D>().velocity = new Vector2(-walkSpeed, GetComponent<Rigidbody2D>().velocity.y);
		}
		
		// Keep cameras moving with the player
		//foreach (GameObject mainCam in mainCams){
			//mainCam.transform.position = new Vector3(transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z);
		//}
		
		// Gets cursor offset from z so that it's on the same plane as the player
		Vector3 cursorPoint = Input.mousePosition;
		cursorPoint.z = transform.position.z - mainCams[0].transform.position.z;
		cursorPoint = Camera.main.ScreenToWorldPoint(cursorPoint);
		//print (cursorPoint);
		
		// Slow down movement
		if (!isMoving){
			//GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * walkDeceleration, GetComponent<Rigidbody2D>().velocity.y);
		}
		
		//if (grabbing){
		//	Grab ();
		//}
		// If the player should try grabbing something
		if (m_Anim.GetBool("Grabbing") || grabbing){
			m_Anim.SetBool("AlreadyGrabbed", true);
			float startGrabTimer = m_Anim.GetFloat("GrabTimer")+1f*60f*Time.deltaTime;
			m_Anim.SetFloat("GrabTimer", startGrabTimer);
			
			if (startGrabTimer >= 6f){
				if (!grabReturn){
					Grab();
				}
				else{
					FailedGrab();
				}
			}
		}
		/*
		// Triggers a grab if possible and if we did NOT just select a menu action
		//if (Input.GetMouseButtonDown(0) && !m_Anim.GetBool("Grabbing") && !grabbing && !AttackMenu.activeSelf && !justSelectedMenuAction){
		if (Input.GetMouseButtonDown(0) && !m_Anim.GetBool("Grabbing") && !grabbing && (!AttackMenu.activeSelf) && !justSelectedMenuAction){	
			ActivateGrab(cursorPoint);
		}
		*/

		// Sets the cloud behind the Hand to be a clone of the sprite for blurring
		cloud.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
		//cloud2.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
		
		//print(GetComponent<SpriteRenderer>().sprite.texture.width);
		// If crouching, check to see if the character can stand up
		/*
		if (m_Anim.GetBool("Crouch"))
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(handCheck.position, handCheckRadius, m_WhatIsGrabbable))
			{
				//crouch = true;
			}
		}
		*/

		justSelectedMenuAction = false;
	}

	// Checks if there's a direct path to the object with no walls or barriers
	public bool ClearPathToTarget(Vector3 point){
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - transform.position;
		float distance = rayDirection.magnitude;
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(transform.position, rayDirection, distance);
		for (int i = 0; i < hits.Length; i++){
			RaycastHit2D r = hits[i];
			if (!r.collider.isTrigger){
				if (r.transform.gameObject == gameObject || r.transform.gameObject == grabbedObject){
					
				} else {
					// Ignore one way platforms
					if (r.transform.GetComponent<PlatformEffector2D>() && r.transform.GetComponent<PlatformEffector2D>().useOneWay){
						continue;
					}
					// See what wall we hit, and reset the grab reach based on that
					//float dist = Vector3.Distance(r.point, transform.position);
					float dist = r.distance;
					if (dist < closestCollisionDistance){
						closestCollisionDistance = dist;
						closestCollision = r.point;
					}
					if (dist < distance){
						clearPath = false;
						return clearPath;
					}
				}
			}
		}
		return clearPath;
	}
	

	// Launches a hand towards object to grab it
	public void ActivateGrab(Vector3 pointIn){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");

		Vector3 point = pointIn + Vector3.zero;
		point.z = transform.position.z;
		// Can't grab if frozen
		if (IsFrozen()){
			return;
		}
		m_Anim.SetBool("Grabbing", true);
		m_Anim.SetFloat("GrabTimer", 0f);

		// Unsticks from a surface and resets the up vector
		Unstick();
		transform.up = defaultUpVector;

		grabPivot.gameObject.SetActive(true);
		float angle = Mathf.Atan2(point.y - grabPivot.position.y, point.x - grabPivot.position.x);
		bool flipped = false;
		angle = angle*Mathf.Rad2Deg - 90f;
		if (transform.localScale.x < 0){
			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
			angle *= -1;
			flipped = true;
		}
		grabPivot.eulerAngles = new Vector3(grabPivot.eulerAngles.x, grabPivot.eulerAngles.y, angle);

		Vector3 scale = grabPivot.localScale;
		scale.y = 1f;
		grabPivot.localScale = scale;

		//float size = grabPivot.GetComponent<SpriteRenderer>().sprite.rect.height / grabPivot.GetComponent<BoxCollider2D>().size.y;
		float size = (grabPivot.GetComponent<SpriteRenderer>().sprite.rect.height / grabPivot.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) * grabPivot.transform.lossyScale.y;
		float distance = Vector3.Distance(grabPivot.transform.position, point);

		scale.y = .0f;
		//scale.y = distance / size;
		grabScaleY = distance / size;
		grabPivot.localScale = scale;

		grabVelocity = grabScaleY / maxGrabTimer;
		grabPositionVelocity = (point - grabPivot.transform.position) / maxGrabTimer;
		grabbing = true;

		// Checks to see if an object was selected during the click, and if so, possess that shit
		grabbedObject = null;
		for (int i = 0; i < possessables.Length; i++){
			GameObject p = possessables[i];
			if (p.GetComponent<PossessableScript>() && p.GetComponent<PossessableScript>().CanPossess()){
				grabbedObject = p;
				// Sets velocity to stop once we know we've grabbed an object
				SetVelocity(0f, 0f);
				break;
			}
		}
		// Do a raycast to determine whether the object is hitting a wall before touching the enemy
		// Ignores triggers
		bool hitWall = false;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - transform.position;
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(transform.position, rayDirection, distance);
		for (int i = 0; i < hits.Length; i++){
			RaycastHit2D r = hits[i];
			if (!r.collider.isTrigger){
				if (r.transform.gameObject == gameObject || r.transform.gameObject == grabbedObject){

				} else {
					// Don't want to be blocked by one way platforms
					if (r.transform.GetComponent<PlatformEffector2D>() && r.transform.GetComponent<PlatformEffector2D>().useOneWay){
						continue;
					}
					// Ignores objects on other layers
					if (gameObject.layer == defaultLayer && r.transform.gameObject.layer == bgLayer){
						continue;
					}
					else if (gameObject.layer == bgLayer && (r.transform.gameObject.layer == fgLayer || r.transform.gameObject.layer == defaultLayer)){
						continue;
					}
					// See what wall we hit, and reset the grab reach based on that
					//float dist = Vector3.Distance(r.point, transform.position);
					float dist = r.distance;
					if (dist < closestCollisionDistance){
						print(gameObject.layer);
						print(r.transform.gameObject.layer == bgLayer);
						closestCollisionDistance = dist;
						closestCollision = r.point;
					}
					hitWall = true;
				}
			}
		}
		// Can't grab what's beyond a wall. Readjust the stretch distance to stop at the wall it hit
		if (hitWall){
			grabbedObject = null;
			grabScaleY = closestCollisionDistance / size;

			grabVelocity = grabScaleY / maxGrabTimer;
			grabPositionVelocity = (closestCollision - grabPivot.transform.position) / maxGrabTimer;
		}
		// For when we failed to grab something
		grabFailed = true;
		grabReturn = false;
		m_Anim.SetBool("AlreadyGrabbed", false);
		if (flipped){
			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}
	// Grabs for an object
	public void Grab(){
		// Can't grab if frozen
		if (freezeTimer > 0f){
			return;
		}

		hoveredObjectRenderer.sprite = null;
		Vector3 scale = grabPivot.localScale;

		if (grabTimer < maxGrabTimer){
			SetVelocity(0f, 0f);
			scale.y = scale.y + grabVelocity * 60f*Time.deltaTime;
			grabTimer += 60f*Time.deltaTime;
			grabPivot.localScale = scale;

			// Makes sure the hand keeps moving in the proper direction towards the object
			if (grabbedObject != null){
				Transform target = (grabbedObject.GetComponent<PossessableScript>().possessionPoint != null) ? grabbedObject.GetComponent<PossessableScript>().possessionPoint : grabbedObject.transform;
				float angle = Mathf.Atan2(target.position.y - grabPivot.position.y, target.position.x - grabPivot.position.x);
				bool flipped = false;
				angle = angle*Mathf.Rad2Deg - 90f;
				if (transform.localScale.x < 0){
					// Multiply the player's x local scale by -1.
					Vector3 theScale = transform.localScale;
					theScale.x *= -1;
					transform.localScale = theScale;
					angle *= -1;
					flipped = true;
				}
				grabPivot.eulerAngles = new Vector3(grabPivot.eulerAngles.x, grabPivot.eulerAngles.y, angle);
			}
		} else{
			grabTimer = 0f;
			grabFailed = true;
			grabReturn = true;
			m_Anim.SetBool("Grabbing", false);

			if (grabbedObject != null){
				grabbedObject.GetComponent<PossessableScript>().ActivatePullPlayer();
				//Possess(grabbedObject);
			}
		}
	}
	// Fly towards object during a successful grab
	public void SuccessfulGrab(){
		Vector3 scale = grabPivot.localScale;
		Vector3 position = grabPivot.position;
		if (grabTimer < maxGrabTimer){
			scale.y = scale.y - grabVelocity * 60f*Time.deltaTime;
			grabTimer += 60f*Time.deltaTime * 2f;
			position += 60f*Time.deltaTime * 2f * grabPositionVelocity;
			grabPivot.localScale = scale;
			grabPivot.position = position;
		} else{
			m_Anim.SetBool("Grabbing", false);
			m_Anim.SetFloat("GrabTimer", 0f);
			grabTimer = 0f;
			grabFailed = false;
			grabReturn = false;
			grabbing = false;
			grabPivot.gameObject.SetActive(false);
		}
	}
	// Cleanup for when a grab fails
	public void FailedGrab(){
		Vector3 scale = grabPivot.localScale;
		//print (Time.deltaTime);
		if (grabTimer < maxGrabTimer){
			scale.y = scale.y - grabVelocity * 60f*Time.deltaTime;
			grabTimer += 60f*Time.deltaTime;
			grabPivot.localScale = scale;
		} else{
			m_Anim.SetBool("Grabbing", false);
			m_Anim.SetFloat("GrabTimer", 0f);
			grabTimer = 0f;
			grabFailed = false;
			grabReturn = false;
			grabbing = false;
			grabPivot.gameObject.SetActive(false);
			// If we're not going to possess an enemy after the grab is over, clear out the target
			if (setToPossess == null){
				ClearSelectedTarget();
			}
		}
	}
	// Possesses object and target camera on it
	public void Possess(GameObject grabbedObject){
		// Mark an object in the possessed history
		HasPossessedBefore(grabbedObject.GetComponent<NetworkGenericEnemy>().GetName());
		hoveredObjectRenderer.sprite = null;
		AddToPossessionMemory(grabbedObject);

		grabbedObject.GetComponent<NetworkPossessableScript>().Possess(gameObject, PossessedAttackMenu);
		enemyStats.SetActive(false);
		SetCameraTarget(grabbedObject.transform);
		gameObject.SetActive(false);

		currentObjectForm = grabbedObject.transform;

		SetToPossess(null);

		ClearSelectedTarget();
	}
	// Possesses object and marks it as a memory
	public void PossessMemory(GameObject grabbedObject){
		memorizedPossession = grabbedObject;
		Possess(grabbedObject);
	}

	// Adds an object to the hand's possession memory
	public void AddToPossessionMemory(GameObject obj){
		// NOTE: This checks to see if enemy names are the same
		// Things like BasicSoldier(1) and BasicSoldier are considered NOT THE SAME, so we'll have two soldier copies
		// Be sure to change that
		bool alreadyAddedEnemyType = false;
		char[] sep = {' '};
		string name = obj.name.Split(sep)[0];
		for (int i = 0; i < possessionMemory.Count; i++){
			if (name == (string)possessionMemory[i]){
				alreadyAddedEnemyType = true;
			}
		}
		if (!alreadyAddedEnemyType){
			if (possessionMemory.Count >= possessionMemoryLimit){
				possessionMemory.RemoveAt(0);
			}
			// An exception to the naming convention
			if (name == "TurretFlipped"){
				name = "Turret";
			}
			print(name);
			possessionMemory.Add(name);
		}
	}

	// Nulls the selected target
	void ClearSelectedTarget(){
		selectedTarget = null;
	}

	// Gets the possession list
	public ArrayList GetPossessionMemory(){
		return possessionMemory;
	}
	
	// Hand exits possession
	// Depossesses
	public void ExitPossession(){
		m_Anim.SetBool("ExitPossession", true);
		m_Anim.SetBool("Grabbing", false);
		m_Anim.SetBool("AlreadyGrabbed", false);
		m_Anim.SetFloat("GrabTimer", 0f);
		grabPivot.gameObject.SetActive(false);
		// Destroy the object if we pulled it from memory
		if (memorizedPossession){
			Destroy(memorizedPossession, 0f);
			memorizedPossession = null;
		}
		grabbedObject = null;

		//Camera.main.GetComponent<CameraTarget>().target = transform;
		SetCameraTarget(transform);

		// This might cause an error; remove if so
		// We set grabbing to false here so we can instantly grab another object upon leaving one
		grabbing = false;

		// Launches the player up a bit
		rb.AddForce(new Vector2(0f, 250f));

		GetComponent<PlatformerCharacter2D>().SetLastGroundedPosition(transform.position);

		// Sets the scale back to normal
		ResetScale();

		SetToPossess(null);

		currentObjectForm = transform;

		// No stickiness
		stickTimer = 0f;
	}

	// Sets the camera to follow a specific object
	void SetCameraTarget(Transform t){
		for (int i = 0; i < mainCams.Length; i++){
			mainCams[i].GetComponent<CameraTarget>().target = t;
		}
	}

	// Resets the scale of the object to default scale
	void ResetScale(){
		// Sets the scale back to normal
		if (transform.localScale.x < 0){
			defaultScale.x = -Mathf.Abs(defaultScale.x);
			transform.localScale = defaultScale;
		} else {
			defaultScale.x = Mathf.Abs(defaultScale.x);
			transform.localScale = defaultScale;
		}
	}

	public void SetAttackMenuStatus(bool b){
		if (b){
			rangeCircle.SetActive(true);
		} else {
			rangeCircle.SetActive(false);
			rangeExpandTimer = 0f;
		}
		AttackMenu.SetActive(b);
		crosshair.gameObject.SetActive(b);

		if (b == false){
			// Clears out the grabbed object when closing the menu
			// NOTE: this was added on Feb 24th and may be the source of later bugs.
			//grabbedObject = null;
		}
	}

	public bool GetAttackMenuStatus(){
		return AttackMenu.activeSelf;
	}

	// Plays the chop sound
	public void PlayChompSound(){
		GetComponent<AudioSource>().PlayOneShot(chompishSound);
	}

	// Eats an object
	// eathuman
	public void EatObject(){
		Transform target = selectedTarget;
		NormalTime();
		SetAttackMenuStatus(false);
		if (selectedTarget != null){
			if (target.parent && target.parent.gameObject.GetComponent<NetworkPossessableScript>()){
				target = target.parent;
			}
			// Inedible objects
			if (!target.GetComponent<NetworkPossessableScript>().edible){
				return;
			}

			// Checks if the object is within consumption range
			Transform possessionPoint = target.transform.Find("PossessionPoint");
			bool canEat = false;
			if (Vector3.Distance(transform.position, target.position) < rangeRadius ||
				(possessionPoint != null && Vector3.Distance(transform.position, possessionPoint.position) < rangeRadius)){
				canEat = true;
			}
			if (!canEat){
				return;
			}

			Freeze(.4f);
			GetComponent<AudioSource>().PlayOneShot(goopSound);
			backPartRender.SetActive(true);
			backPartRender.GetComponent<Animator>().SetBool("Eating", true);
			m_Anim.SetBool("Eating", true);
			// Locks the target in place
			target.gameObject.GetComponent<NetworkPossessableScript>().Freeze(wrapAroundPoint);
			
			if (transform.localScale.x < 0){
				transform.localScale = new Vector3(-1.1f, 1f, 1f);
			} else { 
				transform.localScale = new Vector3(1.1f, 1f, 1f);
			}

			// Disables colliders for the spatter
			if (target.gameObject.GetComponent<Rigidbody2D>()){
				target.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
			}
			if (target.gameObject.GetComponent<BoxCollider2D>()){
				target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
			if (target.gameObject.GetComponent<CircleCollider2D>()){
				target.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			}
		} else {
			backPartRender.SetActive(false);
		}
	}

	/// <summary>
	/// Called by objects outside the hand to order the hand to eat it
	/// This functions manually sets the selected target then activates the eating action
	/// </summary>
	/// <param name="eatenObject">Eaten object.</param>
	public void EatObject(Transform eatenObject){
		selectedTarget = eatenObject;
		EatObject();
	}

	// Completes the eating process and adds EXP
	public void CompleteObjectEating(){
		if (selectedTarget != null){
			Transform target = selectedTarget;
			if (target.parent && target.parent.gameObject.GetComponent<NetworkPossessableScript>()){
				target = target.parent;
			}
			AddEXP(target.gameObject);
			
			SpillBlood(selectedTarget);

			// Calls special cleanup functions after an enemy is completely destroyed
			if (selectedTarget.GetComponent<NetworkGenericEnemy>() != null){
				selectedTarget.GetComponent<NetworkGenericEnemy>().FinishBeingDevoured(gameObject);
			}
			if (selectedTarget != target && target.GetComponent<NetworkGenericEnemy>() != null){
				target.GetComponent<NetworkGenericEnemy>().FinishBeingDevoured(gameObject);
			}

			Destroy(target.gameObject, 0f);

			ClearSelectedTarget();
		}
		backPartRender.GetComponent<Animator>().SetBool("Eating", false);
		m_Anim.SetBool("Eating", false);
	}
	// Cleanup for after eating the object
	public void PostObjectEating(){
		ResetScale();
		m_Anim.SetBool("Eating", false);
		backPartRender.GetComponent<Animator>().SetBool("Eating", false);
		backPartRender.SetActive(false);
	}
	// Adds EXP to the player
	void AddEXP(GameObject obj){
		exp += obj.GetComponent<NetworkPossessableScript>().exp;
	}
	// Splashes blood from a consumed object
	void SpillBlood(Transform target){
		float direction = transform.localScale.x;
		direction = (direction > 0) ? 1f : -1f;
		for (int i = 0; i < 25; i++){
			GameObject b = Instantiate(bloodSpatter);
			Vector3 tempPosition = target.transform.position;
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

	// Possesses a human
	// Instead of directly possessing objects, we'll first pull towards it then activate the possession sequence
	public void PossessHuman(){
		Transform target = selectedTarget;
		NormalTime();
		SetAttackMenuStatus(false);
		if (selectedTarget != null){
			if (target.parent && target.parent.gameObject.GetComponent<NetworkPossessableScript>()){
				target = target.parent;
			}
			// Normal grabs for non-humans
			if (!target.GetComponent<NetworkPossessableScript>().isHuman){
				//Possess(target.gameObject);
				ActivateGrab(target.position);
				return;
			}
			backPartRender.SetActive(true);
			backPartRender.GetComponent<Animator>().SetBool("Possessing", true);
			m_Anim.SetBool("Possessing", true);
			// Locks the target in place
			target.gameObject.GetComponent<NetworkPossessableScript>().Freeze(wrapAroundPoint);

			if (transform.localScale.x < 0){
				transform.localScale = new Vector3(-1.1f, 1f, 1f);
			} else { 
				transform.localScale = new Vector3(1.1f, 1f, 1f);
			}
		} else {
			backPartRender.SetActive(false);
		}
	}
	// Sets an object then possesses it as a human
	public void PossessHuman(GameObject g){
		selectedTarget = g.transform;
		PossessHuman();
	}
	// Completes the possession process for a human
	public void CompleteHumanPossession(){
		if (selectedTarget != null){
			Transform target = selectedTarget;
			if (target.parent && target.parent.gameObject.GetComponent<NetworkPossessableScript>()){
				target = target.parent;
			}
			target.gameObject.GetComponent<Animator>().enabled = true;
			// Non-normal lightning is the American intro
			bool showNormalLightning = true;
			if (target.gameObject.name.Contains("merica")){
				showNormalLightning = false;
			}

			Possess(target.gameObject);
			// Timer to zoom out after possessing
			//target.gameObject.GetComponent<NetworkPossessableScript>().PostDramaticEffect();
			Camera.main.GetComponent<CameraTarget>().QuickZoom(Camera.main.GetComponent<CameraTarget>().GetOriginalZoom() + 2.1f);
			Camera.main.GetComponent<CameraTarget>().SetUnzoomDelay(1.1f);

			backPartRender.GetComponent<Animator>().SetBool("Possessing", false);
			m_Anim.SetBool("Possessing", false);
			backPartRender.SetActive(false);
			if (!showNormalLightning){
				Camera.main.GetComponent<CameraTarget>().TriggerAmerican();
			}
			else {
				TriggerLightning();
			}
		}
	}
	// Sets the hand's position
	public void SetPosition(Vector3 p){
		transform.position = p;
	}
	// Disables the in-possession/eating-process target's animation and blanks out the sprite
	public void MakeTargetInvisible(){
		//PlayChompSound();
		Transform target = selectedTarget;
		NormalTime();
		SetAttackMenuStatus(false);
		if (selectedTarget != null){
			if (target.parent && target.parent.gameObject.GetComponent<NetworkPossessableScript>()){
				target = target.parent;
			}
			target.gameObject.GetComponent<Animator>().enabled = false;
			if (target.gameObject.GetComponent<SpriteRenderer>()){
				target.gameObject.GetComponent<SpriteRenderer>().sprite = null;
			}
		}
	}
	// Triggers the flash of lightning for possession
	public void TriggerLightning(){
		Camera.main.GetComponent<CameraTarget>().TriggerLightning();
	}

	// The list of suspicious targets (i.e., objects that've made a lot of noise)
	public ArrayList GetSuspiciousTargets(){
		return suspiciousTargets;
	}
	// Adds to the suspiciousTargets list/increases the timer
	public void AddSuspiciousTarget(GameObject p, float timer){
		if (suspiciousTargets.Contains(p)){
			// Set the current time to 'timer'
		} else {
			suspiciousTargets.Add(p);
		}
	}

	// Handles phantom delays
	void PhantomMovement(){
		//phantomTimer += Time.deltaTime;
		//if (phantomTimer >= maxPhantomTimer){

		//}
		//cloud.GetComponent<SpriteRenderer>().material.SetFloat("timer", 0f);
	}
	void OnCollisionEnter2D(Collision2D col){
		//print (col.gameObject.name);
	}

	// Lock controls for a period of 't'
	public void TimedLockControls(float t){
		controlLockTimer = t;
	}
	// Lock controls for a period of 't'
	public float GetTimedLockControls(){
		return controlLockTimer;
	}
	// Reduces the controlLockTimer
	void ReduceControlLockTimer(){
		controlLockTimer -= Time.deltaTime;
	}

	// Adds a key to the player's inventory
	public void PickUpKey(int k){
		keys.Add(k);
	}

	// Sets the velocity of the player
	public void SetVelocity(float x, float y){
		rb.velocity = new Vector2(x, y);
	}

	// Checks if the player has a key
	public bool HasKey(int k){
		return keys.Contains(k);
	}

	// Injures the hand and splatters goo
	public override void Injured(){
		float direction = transform.localScale.x;
		direction = (direction > 0) ? 1f : -1f;
		for (int i = 0; i < 12; i++){
			GameObject b = Instantiate(handSpatter);
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

	// Checks if an object of this type was previously possessed
	public bool HasPossessedBefore(string s){
		String s2 = s;
		s2 = s2.Split(' ')[0];
		if (hasPossessedBefore.Contains(s2)){
			return true;
		} else {
			hasPossessedBefore.Add(s2);
			return false;
		}
	}

	/// <summary>
	/// Restores the life of the player. May want to use when triggering certain events so that the player can't die
	/// </summary>
	public void RestoreLife(){
		life = maxLife;
	}

	// Aerannian shatter effect
	// Turn a texture into an array of pixels, turn every N pixel into a rect
	public void Shatter(){
		timer++;
		if (timer % 60 != 0)
			return;
		alreadyShattered = true;
		Texture2D originSprite = GetComponent<SpriteRenderer>().sprite.texture;
		Rect bounds = GetComponent<SpriteRenderer>().sprite.textureRect;
		int w = (int)bounds.width;
		int h = (int)bounds.height;
		int startX = (int)bounds.x;
		int startY = (int)bounds.y;
		int n = 0;
		
		Color[] pixels = originSprite.GetPixels(startX, startY, w, h);
		// Goes through the array of pixels, generating blocks
		for (int i = 0; i < w; i += 35){
			for (int j = 0; j < h; j+= 35){
				Color c = pixels[i+j*w];
				if (c.a > 0f){
					GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSprite"));
					s.GetComponent<ShatterParticleScript>().Instantiate(transform.position + new Vector3((i)/190f, (j)/190f - .1f, 0f), pixels[i+j*w]);
					
					n++;
				}
			}
		}
	}
}



