using UnityEngine;
using System.Collections;

public class WallCrawler : GenericEnemy {
	RippleController rc;
	Animator anim;
	PossessableScript ps;
	float gravity = 0f;
	float fadeTimer = 0f;
	float maxFadeTimer = 5f;
	float damageVal = 5f;
	float delayAttackTimer = 0f;
	float walkTimer = 0f;
	float maxWalkTimer = 3f;
	float speed = 2.5f;
	float lastLife;
	GameObject collided;
	// Use this for initialization
	void Start () {
		GetComponent<PossessableScript>().possessable = false;
		rc = GetComponent<RippleController>();
		ps = GetComponent<PossessableScript>();
		anim = GetComponent<Animator>();
		lastLife = life;
		gravity = GetComponent<Rigidbody2D>().gravityScale;
	}
	
	// Update is called once per frame
	void Update () {
		damageTimer -= Time.deltaTime;
		if (ps.possessed){
			// Controls movement
			float v = 0f;
			if (Input.GetAxisRaw("Horizontal") < 0f){
				v = -1f;
				anim.SetBool("walk", true);
			} else if (Input.GetAxisRaw("Horizontal") > 0f){
				v = 1f;
				anim.SetBool("walk", true);
			} else {
				anim.SetBool("walk", false);
			}
			Vector2 vel = GetComponent<Rigidbody2D>().velocity;
			vel.x = v * speed;
			GetComponent<Rigidbody2D>().velocity = vel;

			// Flips the image when moving
			Vector3 tempScale = transform.localScale;
			if (v != 0f){
				tempScale.x = Mathf.Abs(tempScale.x)*v;
				transform.localScale = tempScale;
			}

			// Handles attacks
			if (delayAttackTimer > 0f){
				delayAttackTimer -= Time.deltaTime;
				if (delayAttackTimer <= 0f){
					fadeTimer = 0f;
					if (transform.Find("Ripple").GetComponent<BoxCollider2D>().IsTouching(collided.GetComponent<Collider2D>())){
						collided.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
					}
				}
			}
			// Attacks
			if (Input.GetButtonDown("Fire1")){
				//delayAttackTimer = .1f;
				//anim.SetBool("attack", true);
			}

			return;
		}
		// Moves towards player
		if (walkTimer > 0f && anim.GetBool("attack") == false){
			float playerX = Camera.main.GetComponent<CameraTarget>().GetCurrentPlayer().position.x;
			// Moves towards the player
			if (Mathf.Abs(playerX - transform.position.x) > .5f){
				float direction = 1f;
				Vector3 tempScale = transform.localScale;
				if (playerX < transform.position.x){
					direction = -1f;
				}
				tempScale.x = Mathf.Abs(tempScale.x)*direction;
				transform.localScale = tempScale;

				Vector2 vel = GetComponent<Rigidbody2D>().velocity;
				vel.x = direction * speed;
				GetComponent<Rigidbody2D>().velocity = vel;
				anim.SetBool("walk", true);
			} else {
				Vector2 vel = GetComponent<Rigidbody2D>().velocity;
				vel.x = 0f;
				GetComponent<Rigidbody2D>().velocity = vel;
				anim.SetBool("walk", false);
			}
			walkTimer -= Time.deltaTime;
		}
		else {
			anim.SetBool("walk", false);
			Vector2 vel = GetComponent<Rigidbody2D>().velocity;
			vel.x = 0f;
			GetComponent<Rigidbody2D>().velocity = vel;
		}
		//anim.SetBool("attack", false);
		fadeTimer += Time.deltaTime;
		//rc.SetRippleVisibility(fadeTimer / maxFadeTimer);
		if (delayAttackTimer > 0f){
			delayAttackTimer -= Time.deltaTime;
			if (delayAttackTimer <= 0f){
				fadeTimer = 0f;
				if (transform.Find("Ripple").GetComponent<BoxCollider2D>().IsTouching(collided.GetComponent<Collider2D>())){
					collided.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
				}
			}
		}

		// Starts moving if hurt
		if (lastLife > life){
			lastLife = life;
			walkTimer = maxWalkTimer;
		}
	}

	public override void ChildTriggered(GameObject child, GameObject col){
		if (col.GetComponent<GenericEnemy>()){
			collided = col;
			delayAttackTimer = .1f;
			anim.SetBool("attack", true);
		}
	}

	// Dies and spawns an illum body
	public override void Death(){
		GameObject g = (GameObject)Resources.Load("IllumBody");
		g = (GameObject)Instantiate(g);
		g.GetComponent<IllumBody>().spawnGravity = gravity;
		g.transform.position = transform.position;
		Destroy(gameObject, 0f);
	}
	/*
	void OnTriggerEnter2D(Collider2D col){
		if (col.GetComponent<GenericEnemy>()){
			anim.SetBool("attack", true);
			fadeTimer = 0f;
			col.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
		}
	}
	*/
	
	public void ExitAttack(){
		anim.SetBool("attack", false);
	}
}
