using UnityEngine;
using System.Collections;

public class RotateGun : GenericEnemy {
	// How far the gun turns in each direction
	public float leftAngle = -90f;
	public float rightAngle = 90f;
	float angle = 0f;
	float startAngle = 0f;
	float direction = -1f;
	public float speed = 45f;
	PossessableScript ps;
	GameObject targetObject;
	GameObject FastBullet;
	Transform tip;
	float collisionTimer = 0f;
	float shootTimer = 0f;
	float maxShootTimer = .2f;

	void FireBullet(){
		GameObject tempBullet = (GameObject)Instantiate(FastBullet);
		tempBullet.transform.position = tip.position;
		tempBullet.GetComponent<FastBullet>().Fire(transform.right * transform.localScale.x);
		tempBullet.GetComponent<FastBullet>().spawn = gameObject;
		tempBullet.layer = gameObject.layer;

		Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
		shootTimer = maxShootTimer;
	}

	void Shoot(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		shootTimer -= Time.deltaTime;
		if (collisionTimer <= 0f || shootTimer > 0f){
			return;
		}
		collisionTimer -= Time.deltaTime;
		// Gets all hits going out from the gun tip
		float distance = 100f;
		Vector3 direction = transform.right;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance);
		float closestCollisionDistance = distance;
		GameObject closestCollision = null;
		bool blocked = false;
		// If possessed, shots will hit the nearest object
		bool possessed = ps.possessed;
		// Inital pass to test for wall collisions
		foreach (RaycastHit2D r in hits){
			//print(r.collider.gameObject);
			// Ignores objects on other layers
			if (gameObject.layer == defaultLayer && r.transform.gameObject.layer == bgLayer){
				continue;
			}
			else if (gameObject.layer == bgLayer && (r.transform.gameObject.layer == fgLayer || r.transform.gameObject.layer == defaultLayer)){
				continue;
			}
			if (!r.collider.isTrigger || (possessed && r.collider.isTrigger)){
				if (r.transform != transform){
					// See what wall we hit, and reset the grab reach based on that
					float dist = Vector3.Distance(r.point, transform.position);
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
		print(closestCollision);
		// Register the hit
		if (closestCollision != null && closestCollision.transform.parent){
			closestCollision = closestCollision.transform.parent.gameObject;
		}
		if (closestCollision != null && closestCollision.GetComponent<GenericEnemy>() && shootTimer <= 0f){
			//closestCollision.GetComponent<GenericEnemy>().DelayedDamage(gunDamageVal);
			FireBullet();
			// If the nearest hit is a wall and not a target, and the turret isn't possessed, ignore
		} else if (ps.possessed == false){
			return;
		}

	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player"){
			collisionTimer = .1f; 
		}
	}

	// Use this for initialization
	void Start () {
		startAngle = transform.eulerAngles.z;
		leftAngle = angle + leftAngle;
		rightAngle = angle + rightAngle;
		ps = GetComponent<PossessableScript>();
		targetObject = GameObject.Find("TheHand");

		FastBullet = (GameObject)Resources.Load("FastBullet");
		tip = transform.Find("Tip");
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Animator>().SetBool("possessed", ps.possessed);
		Shoot();
		angle += Time.deltaTime * speed * direction;
		if (direction < 0f && angle <= leftAngle){
			direction = 1f;
		} else if (direction > 0f && angle >= rightAngle){
			direction = -1f;
		}
		Vector3 tempAngle = transform.eulerAngles;
		tempAngle.z = startAngle + angle;
		transform.eulerAngles = tempAngle;

		if (ps.possessed){
			shootTimer = 2f;	// We constantly set a shoot timer here so that the player has a delay after depossessing before being attacked
			if (Input.GetMouseButtonDown(0)){
				FireBullet();
			}
		}
	}
}
