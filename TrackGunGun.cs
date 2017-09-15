using UnityEngine;
using System.Collections;

public class TrackGunGun : GenericEnemy {
	PossessableScript ps;
	GameObject targetObject;
	float gunDamageVal = 2f;
	GameObject FastBullet;
	float shootTimer = 0f;
	float collisionTimer = .1f;
	Animator anim;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		anim = GetComponent<Animator>();
		targetObject = GameObject.Find("TheHand");

		FastBullet = (GameObject)Resources.Load("FastBullet");
	}

	void FireBullet(){
		GameObject tempBullet = (GameObject)Instantiate(FastBullet);
		tempBullet.transform.position = transform.position + transform.right * transform.localScale.x;
		tempBullet.GetComponent<FastBullet>().Fire(transform.right * transform.localScale.x);
		tempBullet.GetComponent<FastBullet>().spawn = gameObject;

		Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
		shootTimer = .3f;
	}

	void Shoot(){
		shootTimer -= Time.deltaTime;
		if (collisionTimer <= 0f){
			return;
		}
		//collisionTimer -= Time.deltaTime;
		// Gets all hits going out from the gun tip
		float distance = 100f;
		Vector3 direction = transform.right * transform.localScale.x;
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance);
		float closestCollisionDistance = distance;
		GameObject closestCollision = null;
		bool blocked = false;
		// If possessed, shots will hit the nearest object
		bool possessed = ps.possessed;
		// Inital pass to test for wall collisions
		foreach (RaycastHit2D r in hits){
			//print(r.collider.gameObject);
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
	
	// Update is called once per frame
	void Update () {
		GetComponent<Animator>().SetBool("possessed", ps.possessed);
		Shoot();
		if (ps.possessed){
			shootTimer = 2f;	// We constantly set a shoot timer here so that the player has a delay after depossessing before being attacked
			if (Input.GetMouseButtonDown(0)){
				FireBullet();
			}
		}
	}
}
