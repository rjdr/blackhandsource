using UnityEngine;
using System.Collections;

// Nothing more than methods to override
public class GenericEnemy : MonoBehaviour {
	/// <summary>
	/// How suspicious the entity is at the moment (0-100, 100 being undeniably a shapeshifter)
	/// </summary>
	protected int ownSuspicionLevel = 0;

	/// <summary>
	/// How paranoid/suspicious the character is of their surroundings. Over 1f triggers an alert
	/// </summary>
	public float paranoiaLevel = 0f;
	protected float paranoiaFadeRate = .1f;

	// Entity's life
	public float maxLife = 10;
	public float life = 10;
	protected float damageTimer = 0f;
	float maxDamageTimer = .25f;
	protected bool canTakeDamage = true;	// Whether an object can be damaged

	// Enemy was just damaged
	protected bool justDamaged = false;

	// Points used for enemies for climbing up blocks
	protected Transform climbableWallPoint; // Checks for an obstacle in front of the player
	protected Transform freeClimbSpace; 	// Checks if there's a free space above the obstacle
	public float climbCollisionRadius = .3f;

	protected float curiousityTimer = 0f;
	public float maxCuriousityTimer = 3f; 	// How long an object is curious for
	protected Vector3 curiousityPoint = new Vector3(0f, 0f, 0f);
	protected bool curious = false;
	protected float lastVolume = 0f;

	protected bool alerted = false;

	// Timer for how long an idle enemy has before turning around
	// This must be manually set by the object
	protected float turnAroundTimer = 0f;
	public bool showTurnAroundTimer = false;

	// Used to represent the side of the combat an entity supports
	// 1 is germans/axis, 2 is allies
	public int alliedSide = 1;

	protected bool killable = true;


	/// <summary>
	/// Target object should be warned before being attacked
	/// </summary>
	protected bool warnTarget = false;
	protected float warnDist = 2f;
	protected float warnLowerTimer = 0f;
	/// <summary>
	/// Object falsely believed to be player (used to throw off enemy)
	/// </summary>
	protected GameObject falseTarget = null;
	/// <summary>
	/// Timer for which the object follows a false target (once it falls to 0, it retargets the player)
	/// </summary>
	protected float falseTargetTimer = 0f;
	protected float maxFalseTargetTimer = 4f;

	/// <summary>
	/// How sensitive the object is to sound (higher is less sensitive, 1 is very sensitive)
	/// </summary>
	protected float soundSensitivity = 40f;

	// Enemy is in the background
	public bool inBackgroundLayer = false;

	AudioClip exclaimSound;
	AudioClip questionSound;

	protected bool touchingLadder = false;
	protected int ladderDirection = 0;
	protected Vector3 ladderDestination;
	protected GameObject ladder;

	// For reverting the ID after having committed an act of friendly fire
	public float alliedSideChangeTimer = 0f;
	float maxAlliedSideChangeTimer = 5f;

	// If an object is from memory, it'll despawn once depossessed
	public bool fromMemory = false;

	public float recentlyInjuredTimer = 0f;

	public void Start(){
		
	}

	public virtual void Init(){

	}

	// Heats/ignites an object
	public virtual void Heat(){

	}

	// Sets an object as touching a ladder
	public void SetTouchingLadder(bool b, GameObject l){
		touchingLadder = b;
		ladder = l;
	}

	// A smooth LERP that weights the speed more towards the center, giving it a sense of acceleration/decceleration
	public static float SmoothLERP(float a, float b, float t)
	{
		float n = ((t) * (t) * (3 - 2 * (t)));
		float x = (a * n) + (b * (1 - n));
		return x;
	}
	// A smooth LERP that weights the speed more towards the center, giving it a sense of acceleration/decceleration
	public static Vector3 SmoothLERP(Vector3 a, Vector3 b, float t)
	{
		Vector3 v = new Vector3(SmoothLERP(a.x, b.x, t), SmoothLERP(a.y, b.y, t), SmoothLERP(a.z, b.y, t));
		return v;
	}

	// Makes note of a child object having touched something
	public virtual void ChildCollided(GameObject child, GameObject hit){

	}
	public virtual void ChildTriggered(GameObject child, GameObject hit){

	}
	public virtual void ChildTriggerStayed(GameObject child, GameObject hit){
		
	}

	// Climbs a ladder
	public virtual void ClimbLadder(){

	}

	public virtual void StopClimbingLadder(){

	}

	// Plays the exclamation sound
	public void PlayExclamationSound(){
		if (exclaimSound == null){
			exclaimSound = Resources.Load("Audio/exclaim") as AudioClip;
		}
		GetComponent<AudioSource>().PlayOneShot(exclaimSound);
	}

	// Plays the question sound
	public void PlayQuestionSound(){
		if (questionSound == null){
			questionSound = Resources.Load("Audio/question") as AudioClip;
		}
		GetComponent<AudioSource>().PlayOneShot(questionSound);
	}

	// Returns how suspicious an object currently is
	public int GetSuspiciousLevel(){
		return ownSuspicionLevel;
	}

	// Gets the point on a bezier curve
	public virtual Vector3 BezierPosition(Vector3 End, Vector3 bezier1, Vector3 Start, float t){
		return End*t*t + bezier1*2*t*(1-t) + Start*(1-t)*(1-t);
	}

	public virtual void SetParanoia(float f){
		Camera.main.GetComponent<CameraTarget>().SetParanoia(f);
	}

	// Checks if the character can climb up a block
	public GameObject CanClimbBlock(){
		// Loads the points if they're not already there
		if (climbableWallPoint == null){
			climbableWallPoint = transform.Find("ClimbableWallPoint");
			freeClimbSpace = transform.Find("FreeClimbSpace");
		}
		//bool canClimb = false;
		GameObject block = null;
		// If there's nothing in the free space, check if there's a block to climb on top of
		Collider2D[] freeSpaceCols = Physics2D.OverlapCircleAll(freeClimbSpace.position, climbCollisionRadius);
		//bool freeSpaceOpen = true;
		foreach (Collider2D c in freeSpaceCols){
			if (!c.isTrigger){
				//freeSpaceOpen = false; 
				return null;
			}
		}
		// Check that there's a non-trigger box to climb on top of
		Collider2D[] climbableBlocks = Physics2D.OverlapCircleAll(climbableWallPoint.position, climbCollisionRadius);
		foreach (Collider2D c in climbableBlocks){
			if (!c.isTrigger && (c.tag == "Wall")){
				block = c.gameObject;
			}
		}
		return block;
	}

	// Climbs a block
	public virtual void ClimbBlock(){

	}

	/// <summary>
	/// Is alerted and starts chasing an object
	/// </summary>
	public virtual void Alert(){

	}

	/// <summary>
	/// Ends the alert status of an object
	/// </summary>
	public virtual void EndAlert(){
		alerted = false;
	}

	/// <summary>
	/// Returns whether the entity is currently alerted
	/// </summary>
	/// <returns><c>true</c> if this instance is alerted; otherwise, <c>false</c>.</returns>
	public virtual bool IsAlerted(){
		return alerted;
	}

	/// <summary>
	/// Manages the curiousity of an object (its target and remaining time)
	/// </summary>
	public virtual void ManageCuriousity(){
		curiousityTimer -= Time.deltaTime;
		lastVolume -= Time.deltaTime;
	}

	/// <summary>
	/// Triggers an objects curiosity and makes it investigate an area
	/// </summary>
	/// <param name="loc">Location</param>
	public virtual void TriggerCuriousity(Transform trans, float vol){

	}

	// Generates a dust cloud
	public virtual void DustCloud(){

	}

	// Changes the allied ID in cases of friendly fire, etc
	public void ChangeAlliedID(int i){
		alliedSide = i;
		alliedSideChangeTimer = maxAlliedSideChangeTimer;
	}

	// Triggers an event when damaged
	public virtual void DamagedEvent(GameObject attacker){
		Alert();
		// In case of friendly fire by the player, have the player's allied ID change to make it an enemy
		// This prevents triggering an alert, but also makes enemies respond to the player upon sight
		if (attacker.GetComponent<Weapon>()){
			if (GetComponent<PossessableScript>() && GetComponent<PossessableScript>().possessed == false){
				if (attacker.GetComponent<Weapon>().spawn.gameObject.tag == "Player"){
					attacker.GetComponent<Weapon>().spawn.GetComponent<GenericEnemy>().ChangeAlliedID(600);
				}
			}
		}
	}

	// Does instant damage to the object
	public virtual void InstantDamage(float dmg){
		if (canTakeDamage == false){
			return;
		}
		life -= dmg;
		damageTimer = maxDamageTimer;
		if (life <= 0){
			if (GetComponent<Shatterable>()){
				Death();
				if (killable){
					GetComponent<Shatterable>().Shatter();
					Destroy(gameObject, 0f);
				}
			}
		}
		justDamaged = true;
		Injured();
	}
	// Does instant damage to the object
	public virtual void InstantDamage(float dmg, GameObject attacker){
		InstantDamage(dmg);
		Injured(attacker);
	}

	// Does damage to an object
	public virtual void DelayedDamage(float dmg){
		if (canTakeDamage == false){
			return;
		}
		if (damageTimer <= 0f){
			life -= dmg;
			damageTimer = maxDamageTimer;
			if (life <= 0){
				if (GetComponent<Shatterable>()){
					Death();
					if (killable){
						GetComponent<Shatterable>().Shatter();
						Destroy(gameObject, 0f);
					}
				}
			}
			justDamaged = true;
			Injured();
		}
	}
	// Does damage to an object
	public virtual void DelayedDamage(float dmg, GameObject attacker){
		DelayedDamage(dmg);
		Injured(attacker);
	}
	// Called when the object is damaged
	public virtual void Injured(){

	}
	public virtual void Injured(GameObject attacker){

	}

	// Disables the display of extraneous things
	public virtual void DisableSubcomponents(){

	}

	// Becomes possessed
	public virtual void Possess(){

	}

	// Adds a slow text object
	public FastText AddSlowText(string s, float x, float y, bool cascading = false, bool increaseSize = false){
		GameObject fastText = (GameObject)Resources.Load("SlowText");
		fastText = (GameObject)Instantiate(fastText);
		fastText.GetComponent<FastText>().SetText(s);
		fastText.GetComponent<FastText>().SetPosition(x, y);
		fastText.GetComponent<FastText>().cascading = cascading;
		fastText.GetComponent<FastText>().increaseSize = increaseSize;

		return fastText.GetComponent<FastText>();
	}

	// Adds a fast text object
	public FastText AddFastText(string s, float x, float y, bool cascading = false, bool increaseSize = false){
		GameObject fastText = (GameObject)Resources.Load("FastText");
		fastText = (GameObject)Instantiate(fastText);
		fastText.GetComponent<FastText>().SetText(s);
		fastText.GetComponent<FastText>().SetPosition(x, y);
		fastText.GetComponent<FastText>().cascading = cascading;
		fastText.GetComponent<FastText>().increaseSize = increaseSize;

		return fastText.GetComponent<FastText>();
	}

	// Gets the name of an object
	public string GetName(){
		return (this.GetType().ToString());
	}

	/// <summary>
	/// Cleanup code/special actions after an object is eaten
	/// </summary>
	/// <param name="eater">Eater.</param>
	public virtual void FinishBeingDevoured(GameObject eater){

	}

	/// <summary>
	/// Any extra depossession actions go here.
	/// Note that PossessableScript calls this function, so referring back to the PossessableScript causes a Stack Overflow
	/// </summary>
	public virtual void Depossess(){
	
	}
	public virtual void ActivateAttack(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}
	public virtual void ActivateTransmit(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}
	public virtual void ActivateEat(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}
	public virtual void ActivatePossess(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}
	public virtual void ActivateNoise(){
		Camera.main.GetComponent<CameraTarget>().SetTimeScale(1f);
	}
	// For highlighting special objects
	public virtual void SpecialHighlighting(){

	}
	// For death-related shit
	public virtual void Death(){
		if (GetComponent<PossessableScript>()){
			if (GetComponent<PossessableScript>().possessed){
				GetComponent<PossessableScript>().Depossess();
			}
		}
	}

	/// <summary>
	/// Adds a sound ring through the camera
	/// Sound rings have a visible range:
	/// A circle is shown as the circumference, and an inner circle moves out toward it, then dissipates upon contact.
	/// </summary>
	/// <param name="location">Location.</param>
	/// <param name="volume">Volume.</param>
	public void AddSoundRing(Vector3 location, float volume){
		Camera.main.GetComponent<CameraTarget>().AddSoundRing(location, volume, transform);
	}

	public virtual bool IsHidden(){
		return false;
	}

	// Checks for a clear path to an object
	public bool ClearPathToTarget(Transform p, Transform self, Vector3 targetPoint, float maxDist){
		// If the object isn't active, it shouldn't be visible at all
		if (!p.gameObject.activeSelf){
			return false;
		}
		if (p.GetComponent<GenericEnemy>() && p.GetComponent<GenericEnemy>().IsHidden()){
			return false;
		}

		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");

		Vector3 point = targetPoint;
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - self.position;
		float distance = rayDirection.magnitude;
		if (distance > maxDist){
			return false;
		}
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		//float closestCollisionDistance = distance;
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(self.position, rayDirection, distance);
		foreach (RaycastHit2D r in hits){
			// Ignores objects on other layers
			if (gameObject.layer == defaultLayer && r.transform.gameObject.layer == bgLayer){
				continue;
			}
			else if (gameObject.layer == bgLayer && (r.transform.gameObject.layer == fgLayer || r.transform.gameObject.layer == defaultLayer)){
				continue;
			}
			if (!r.collider.isTrigger){
				if (r.transform != p && r.transform != self){
					// See what wall we hit, and reset the grab reach based on that
					float dist = Vector3.Distance(r.point, self.position);
					if (dist < closestCollisionDistance && !r.collider.isTrigger){
						closestCollisionDistance = dist;
						closestCollision = r.point;
						clearPath = false;
					}
					
				} 
			}
		}
		return clearPath;
	}
	
	// Checks for a clear path to an object
	public bool ClearPathToTarget(Transform p, Transform self, float maxDist){
		// If the object isn't active, it shouldn't be visible at all
		if (!p.gameObject.activeSelf){
			return false;
		}
		
		Vector3 point = p.position;
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - self.position;
		float distance = rayDirection.magnitude;
		if (distance > maxDist){
			return false;
		}
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		//float closestCollisionDistance = distance;
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(self.position, rayDirection, distance);
		foreach (RaycastHit2D r in hits){
			if (!r.collider.isTrigger){
				if (r.transform != p && r.transform != self){
					// See what wall we hit, and reset the grab reach based on that
					float dist = Vector3.Distance(r.point, self.position);
					if (dist < closestCollisionDistance && !r.collider.isTrigger){
						closestCollisionDistance = dist;
						closestCollision = r.point;
						clearPath = false;
					}
					
				} 
			}
		}
		return clearPath;
	}

	// Gets the point of the cursor
	public Vector3 GetCursorPoint(){
		Vector3 cursorPoint = Input.mousePosition;
		cursorPoint.z = transform.position.z - Camera.main.transform.position.z;
		cursorPoint = Camera.main.ScreenToWorldPoint(cursorPoint);
		return cursorPoint;
	}

	// Gets the proper angle given the face direction
	public float GetAngleToObj(Vector3 fromPos, Vector3 toPos){
		// Sets player to face the cursor's position]
		float ang;
		if (transform.localScale.x < 0){
			ang = -Mathf.Atan2(toPos.y-fromPos.y, toPos.x-fromPos.x);
		} else {
			ang = Mathf.Atan2(toPos.y-fromPos.y, toPos.x-fromPos.x);
		}
		ang *= Mathf.Rad2Deg;
		
		if (transform.localScale.x < 0){
			ang += 180f;
		}
		
		return ang;
	}
}
