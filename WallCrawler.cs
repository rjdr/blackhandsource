using UnityEngine;
using System.Collections;

public class WallCrawler : GenericEnemy {
	RippleController rc;
	Animator anim;
	float fadeTimer = 0f;
	float maxFadeTimer = 5f;
	float damageVal = 5f;
	float delayAttackTimer = 0f;
	GameObject collided;
	// Use this for initialization
	void Start () {
		rc = GetComponent<RippleController>();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		//anim.SetBool("attack", false);
		fadeTimer += Time.deltaTime;
		rc.SetRippleVisibility(fadeTimer / maxFadeTimer);
		if (delayAttackTimer > 0f){
			delayAttackTimer -= Time.deltaTime;
			if (delayAttackTimer <= 0f){
				fadeTimer = 0f;
				if (transform.Find("Ripple").GetComponent<BoxCollider2D>().IsTouching(collided.GetComponent<Collider2D>())){
					collided.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
				}
			}
		}
	}

	public override void ChildTriggered(GameObject child, GameObject col){
		if (col.GetComponent<GenericEnemy>()){
			collided = col;
			delayAttackTimer = .1f;
			anim.SetBool("attack", true);
		}
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
