using UnityEngine;
using System.Collections;

public class Bat : GenericEnemy {
	public Transform target;
	GameObject theHand;
	float dist = 100000f;
	Rigidbody2D rb;
	float facing = -1f;
	float damageVal = 2f;
	// Use this for initialization
	void Start () {
		theHand = GameObject.Find("TheHand");
		rb = GetComponent<Rigidbody2D>();
		soundSensitivity *= .25f;
	}
	
	// Update is called once per frame
	void Update () {
		Flip();
		ManageCuriousity();
		damageTimer -= Time.deltaTime;

		if (life <= 0f){
			GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		}
	}

	// Alerts the bat && finds the closest enemy
	public override void Alert(){
		if (alerted){
			return;
		}
		alerted = true;
		WakeUp();
		PlayExclamationSound();
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Possessable");
		for (int i = 0; i < enemies.Length; i++){
			if (Vector3.Distance(enemies[i].transform.position, transform.position) < dist){
				dist = Vector3.Distance(enemies[i].transform.position, transform.position);
				target = enemies[i].transform;
			}
		}
		if (Vector3.Distance(theHand.GetComponent<HandController>().currentObjectForm.position, transform.position) < dist){
			target = theHand.GetComponent<HandController>().currentObjectForm;
		}
		dist = 100000f;
	}

	// Makes an enemy curious
	public override void TriggerCuriousity(Transform trans, float vol){
		if (vol >= lastVolume){
			curiousityTimer = maxCuriousityTimer;
			curiousityPoint = trans.position;
			if (!trans.GetComponent<SoundRing>().AlreadyHeardBy(gameObject) && trans.GetComponent<SoundRing>().spawn != gameObject){
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

	/// <summary>
	/// Manages the curiousity of an object (its target and remaining time)
	/// </summary>
	public override void ManageCuriousity(){
		// Sets the object's direction of movement to be towards the point of curiousity
		bool wasCurious = curious;
		curious = false;
		if (curiousityTimer > 0f && !alerted){
			curiousityTimer -= Time.fixedDeltaTime;
			//chaseDest = curiousityPoint;
			curious = true;
			// prevents QuestionSound from playing endlessly
			if (!wasCurious){
			//	PlayQuestionSound();
			}
		}
		// Interest in last sound fades and new loud noises take priority (although quiet ones don't)
		lastVolume -= Time.deltaTime;
		// Object becomes less paranoid as time goes on
		paranoiaLevel -= Time.deltaTime*paranoiaFadeRate;
	}

	// Wakes up the bat
	void WakeUp(){
		GetComponent<Animator>().SetBool("awake", true);
		GetComponent<Rigidbody2D>().gravityScale = 1f;
	}

	private void Flip()
	{
		float lastFacing = facing;
		// Switch the way the player is labelled as facing.
		if (rb.velocity.x > 0) {
			facing = 1;
		} else {
			facing = -1;
		}
		if (lastFacing != facing){
			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}


	// Flaps wings
	public void Flap(){
		Vector2 speed = new Vector2(0f, 0f);
		if (target.position.x > transform.position.x){
			speed.x = 2f;
		} else {
			speed.x = -2f;
		}
		if (target.position.y > transform.position.y){
			speed.y = 7f;
		} else {
			speed.y = 2f;
		}
		GetComponent<Rigidbody2D>().velocity += speed;
		Vector2.ClampMagnitude(rb.velocity, 10f);
	}

	public void OnTriggerEnter2D(Collider2D col){
		if (col.GetComponent<GenericEnemy>() && !col.GetComponent<Bat>()){
			col.GetComponent<GenericEnemy>().DelayedDamage(damageVal, gameObject);
		}
		Alert();
	}
	public void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && !col.gameObject.GetComponent<Bat>()){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal, gameObject);
		}
		Alert();
	}
}
