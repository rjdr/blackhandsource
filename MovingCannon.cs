using UnityEngine;
using System.Collections;

public class MovingCannon : GenericEnemy {
	int facing = 1;
	Rigidbody2D rb;
	PossessableScript ps;
	float moveSpeed = 3.5f;
	float rotateSpeed = -140f;
	float waitAttackTimer = 0f;
	bool attacking = false;
	Animator m_Anim;
	Transform wheelFront;
	Transform wheelBack;

	GameObject solidBullet;
	GameObject gunTip;
	GameObject dustCloud;
	float gunDamageVal = 5;
	float gunVolume = 19.25f;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		m_Anim = GetComponent<Animator>();
		wheelFront = transform.Find("WheelFront");
		wheelBack = transform.Find("WheelBack");
		ps = GetComponent<PossessableScript>();

		solidBullet = (GameObject)Resources.Load("FastBullet");
		dustCloud = (GameObject)Resources.Load("DustCloud");
		gunTip = transform.Find("GunTip").gameObject;
	}

	// Update is called once per frame
	void Update () {
		// Determines whether user has control or object is autonomous
		if (ps.possessed){
			//rigid.isKinematic = false;
			m_Anim.SetBool("Possessed", true);

			waitAttackTimer -= Time.deltaTime;
			PlayerControls();
		} else {
			m_Anim.SetBool("Possessed", false);
			//PlayerControls();
		}
		waitAttackTimer -= Time.deltaTime;
		if (attacking && waitAttackTimer <= 0f){
			Attack();
		}
	}

	void ActivateAttack(){
		if (waitAttackTimer <= 0f){
			Attack();
			Camera.main.GetComponent<CameraTarget>().AddShake(.3f);
			rb.velocity += (Vector2)transform.right * -facing * 9f;
		}
	}

	void Attack(){
		GetComponent<AudioSource>().Play();
		waitAttackTimer = 1f;
		GameObject b2 = (GameObject)Instantiate(solidBullet);
		b2.transform.localScale *= 2f;
		b2.transform.position = gunTip.transform.position;
		b2.GetComponent<FastBullet>().spawn = gameObject;
		b2.GetComponent<FastBullet>().Fire(gunTip.transform.right * facing);

		GameObject p = (GameObject)Instantiate(dustCloud);
		p.transform.position = gunTip.transform.position;
		p.transform.localScale *= 1.2f;
		p.GetComponent<Dustcloud>().velocity = gunTip.transform.forward * Time.fixedDeltaTime;
		p.GetComponent<Dustcloud>().velocity.x *= transform.localScale.x;
		// Add a sound ring
		AddSoundRing(gunTip.transform.position, gunVolume);
	}

	// Move and flip the cannon
	void PlayerControls(){
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
			if (facing < 0){
				Flip ();
			}
			rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
			wheelFront.localEulerAngles += new Vector3(0f, 0f, rotateSpeed*Time.deltaTime);
			wheelBack.localEulerAngles += new Vector3(0f, 0f, rotateSpeed*Time.deltaTime);
			// Moving toilets are incredibly suspicious
			ownSuspicionLevel = 100;
		} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
			if (facing > 0){
				Flip ();
			}
			rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
			wheelFront.localEulerAngles += new Vector3(0f, 0f, rotateSpeed*Time.deltaTime);
			wheelBack.localEulerAngles += new Vector3(0f, 0f, rotateSpeed*Time.deltaTime);
			// Moving toilets are incredibly suspicious
			ownSuspicionLevel = 100;
		} else {
			//rb.velocity = new Vector2(0f, rb.velocity.y);
			//transform.eulerAngles = new Vector3(0f, 0f, 0f);
			ownSuspicionLevel = 0;
		}

		if (Input.GetMouseButtonDown(0) && !attacking){
			ActivateAttack();
		}
	}

	private void Flip(){
		// Switch the way the player is labelled as facing.
		if (facing < 0) {
			facing = 1;
		} else if (facing > 0){
			facing = -1;
		}

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnCollisionStay2D(Collision2D col){
		//transform.up = Vector3.MoveTowards((Vector3)transform.up, (Vector3)((Vector3)transform.position - (Vector3)col.contacts[0].point).normalized, 1f*Time.deltaTime);
		//print(transform.up);
	}
}