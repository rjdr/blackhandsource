using UnityEngine;
using System.Collections;

public class SwingBeam : GrabBeam {
	/*
	Transform leftPoint;
	Transform rightPoint;
	float maxTimer = .1f;
	float timer = 0f;
	Vector3 startPosition = new Vector3(0f, 0f, 0f);
	float velocity = 0f;
	GameObject player;
	MeshRenderer mr;
	Color c = new Color(1f, .2f, .6f);
	Color black = new Color(.1f, .1f, .1f);
	*/
	float angle = 0f;
	float speed = 11f;
	float radius = 1f;
	float rotateDirection = -1f;
	bool spinning = false;
	float launchSpeed = 22f;

	Transform handImage;
	Vector3 handScale;
	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer>();
		//leftPoint = transform.Find("LeftPoint");
		//rightPoint = transform.Find("RightPoint");
		mr.material.SetColor("_OutlineColor", black);
		player = GameObject.Find("TheHand");
		// Set the bar to be at the same depth as the player (otherwise it'll look fucked up)
		//Vector3 tempPos = transform.position;
		//tempPos.z = player.transform.position.z;
		//transform.position = tempPos;
		handImage = transform.Find("HandImage");
		handImage.gameObject.SetActive(false);
		handScale = handImage.localScale;
	}

	// Pulls the player up once the hand starts returning
	void FixedUpdate(){
		if (timer > 0f){
			// Zeros out velocity to prevent the weird falling problem
			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			if (player.GetComponent<HandController>().GrabReturning()){
				timer -= Time.fixedDeltaTime;
				// Don't mess with player's Z depth
				Vector3 destVect = transform.position;
				destVect.z = player.transform.position.z;
				player.transform.position = Vector3.MoveTowards(player.transform.position, destVect, velocity);
				if (timer <= 0f){
					// Enables a simple sprite representation of the player
					handImage.gameObject.SetActive(true);
					spinning = true;
					FlipPlayer();
				}
			}
		}
		// Once the timer ends, spin
		else if (spinning){
			Spin();
		}
	}

	// Sets the direction of the spin
	void SetDirection(){
		if ((player.transform.position.x < transform.position.x && player.transform.position.y < transform.position.y) ||
			(player.transform.position.x > transform.position.x && player.transform.position.y > transform.position.y)){
			rotateDirection = 1f;
		} else {
			rotateDirection = -1f;
		}
		// Flip the image so that it looks like the hand is facing the direction it's moving
		handScale.x = -Mathf.Abs(handScale.x)*rotateDirection;
		handImage.localScale = handScale;
	}

	// Keeps the player locked to the beam, spinning, until released
	void Spin(){
		player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		Vector3 tempPosition = transform.position;
		tempPosition.z = player.transform.position.z;
		tempPosition.x += radius * Mathf.Cos(angle);
		tempPosition.y += radius * Mathf.Sin(angle);
		angle += Time.deltaTime * speed * rotateDirection;
		player.transform.position = tempPosition;

		handImage.localEulerAngles = new Vector3(-90f, 0f, angle * Mathf.Rad2Deg);

		// End spinning if the player clicks elsewhere or changes form
		if (Input.GetMouseButtonDown(0) || player.name != "TheHand"){
			spinning = false;
		}
		// Launch off if jumping
		if (Input.GetKeyDown("space")){
			// Take an angle 90 degrees behind where the player is currently spinning, and launch in that direction
			float newAngle = angle * Mathf.Rad2Deg;
			newAngle += rotateDirection * 90f;
			newAngle *= Mathf.Deg2Rad;
			player.GetComponent<Rigidbody2D>().velocity = new Vector2(launchSpeed * Mathf.Cos(newAngle), launchSpeed * Mathf.Sin(newAngle));
			spinning = false;
		}
		if (spinning == false){
			handImage.gameObject.SetActive(false);
		}
		player.GetComponent<Animator>().SetBool("Swinging", spinning);
	}

	// Makes the player stick to the surface (since the GrabBeam's up vector faces down, have the player's up vector be the opposite of that
	void FlipPlayer(){
		Vector3 up = -transform.up;
		up.z = 0f;
		up = up.normalized;
		player.GetComponent<HandController>().StickToSurface(up);
	}

	/// <summary>
	/// When clicked, the player moves to the the beam
	/// 			NOTE:
	/// If there's another collider (including triggers) in front of the beam, the beam will be unclickable
	/// Make sure the grab beam is always closer to the camera than other potential colliders
	/// </summary>
	void OnMouseDown(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		if (player.activeSelf){
			// Escape if the player and beam are on different layers
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.layer == bgLayer && (gameObject.layer == fgLayer || gameObject.layer == defaultLayer)){
				return;
			}
			if (player.layer == defaultLayer && gameObject.layer == bgLayer){
				return;
			}
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.GetComponent<HandController>().AttackMenu.activeSelf == false || player.GetComponent<HandController>().grabbedObject == null){
				// Checks if player can make contact with the center or edges of the beam
				if (player.GetComponent<HandController>().ClearPathToTarget(transform.position)){
					// Disables attack menu to pull towards a rod
					if (player.GetComponent<HandController>().AttackMenu.activeSelf){
						player.GetComponent<HandController>().SetAttackMenuStatus(false);
						player.GetComponent<HandController>().NormalTime();
					}

					player.GetComponent<HandController>().TimedLockControls(maxTimer);
					//player.transform.position = transform.position;
					timer = maxTimer;
					startPosition = player.transform.position;
					velocity = Vector3.Distance(player.transform.position, transform.position) / (maxTimer/(Time.fixedDeltaTime));
					SetDirection();
				}
			}
		}
	}

	public void OnMouseEnter(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		if (player.activeSelf){
			// Escape if the player and beam are on different layers
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.layer == bgLayer && (gameObject.layer == fgLayer || gameObject.layer == defaultLayer)){
				return;
			}
			if (player.layer == defaultLayer && gameObject.layer == bgLayer){
				return;
			} else {
				mr.material.SetColor("_OutlineColor", c);
			}
		}
	}

	public void OnMouseExit(){
		mr.material.SetColor("_OutlineColor", black);
	}
}
