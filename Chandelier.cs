using UnityEngine;
using System.Collections;

public class Chandelier : GenericEnemy {
	PossessableScript ps;
	Animator m_Anim;
	bool destroyed = false;	
	bool startedAttack = false;
	public int id;
	GameObject[] phones;
	ArrayList collisions = new ArrayList();
	public int delayAttackTimer = 0;

	float damageVal = 12f;

	GameObject dustCloud;

	float waitToDestroyTimer = .05f;

	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		m_Anim = GetComponent<Animator>();
		
		dustCloud = (GameObject)Resources.Load("Dustcloud");

		// Make all chain units independent of the chandelier so they don't fall with it
		ArrayList chains = new ArrayList();
		foreach (Transform t in transform){
			chains.Add(t);
		}
		foreach(Transform t in chains){
			t.SetParent(null);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			if (startedAttack){
				Camera.main.GetComponent<CameraTarget>().SlowZoom(Camera.main.GetComponent<CameraTarget>().GetOriginalZoom() + 1f, 1f*Time.deltaTime);
				Camera.main.GetComponent<CameraTarget>().SetUnzoomDelay(.1f);
			}
			//rigid.isKinematic = false;
			m_Anim.SetBool("Possessed", true);
			PlayerControls();
		} else {
			m_Anim.SetBool("Possessed", false);
			//PlayerControls();
		}
		// Destroyed through being damaged
		if (life <= 0 && !destroyed){
			//FinishAttack();
			//m_Anim.SetBool("Destroyed", true);
		}
		// Counts down the window in which it can kill enemies
		//if (startedAttack && killTimer > 0f){
		//	killTimer -= Time.deltaTime;
		//}
	}

	// Activates the attack
	public override void ActivateAttack(){
		Time.timeScale = 1f;
		if (!destroyed && !startedAttack && delayAttackTimer <= 0){
			ps.DisableDepossession();
			startedAttack = true;
			//ps.depossessable = false;
			GetComponent<Rigidbody2D>().gravityScale = 2f;
			//m_Anim.SetBool("Attacking", true);
			//GetComponent<AudioSource>().Play();
		}
	}
	
	void Attack(){
		//m_Anim.SetBool("Destroyed", true);
	}
	
	public void FinishAttack(){
		if (waitToDestroyTimer <= 0f){
			Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
			m_Anim.SetBool("Attacking", false);
			GameObject d = (GameObject)Instantiate(dustCloud);
			d.transform.position = transform.position;
			ps.EnableDepossession();
			ps.Depossess();
			GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		}
		if (destroyed){
			waitToDestroyTimer -= Time.deltaTime;
		}
	}
	void PlayerControls(){
		if (Input.GetMouseButtonDown(0) && ps.attackMenu.activeSelf == false){
			ActivateAttack();
		}
		delayAttackTimer--;
		FinishAttack();
	}
	// Destroys chandelier briefly after making contact
	void OnTriggerEnter2D(Collider2D col){
		if (startedAttack){
			if (col.gameObject.GetComponent<GenericEnemy>()){
				col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			} else if (col.gameObject.GetComponent<PossessableScript>()){
				col.gameObject.GetComponent<PossessableScript>().DelayedDamage(((int) damageVal));
			}
			destroyed = true;
		}
	}
}
