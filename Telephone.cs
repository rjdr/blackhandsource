using UnityEngine;
using System.Collections;

public class Telephone : GenericEnemy {
	PossessableScript ps;
	Animator m_Anim;
	bool destroyed = false;		// You can still warp through the phone once it's destroyed, but you can't attack or stay in it
	bool startedAttack = false;
	public int id;
	GameObject[] phones;
	float killTimer = 0f;
	ArrayList collisions = new ArrayList();
	public int delayAttackTimer = 0;

	float attackRadius = 1.5f;

	GameObject dustCloud;

	GameObject connection;
	bool showConnection = true;
	float waitAttackTimer = .2f;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		m_Anim = GetComponent<Animator>();

		dustCloud = (GameObject)Resources.Load("Dustcloud");
		connection = transform.Find("ConnectsIcon").gameObject;
		// Gets all phones with the same ID to warp to
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Possessable");
		for (int i = 0; i < targets.Length; i++){
			if (targets[i].GetComponent<Telephone>() && targets[i] != gameObject && targets[i].GetComponent<Telephone>().id == id){
				transform.Find("ConnectsIcon").eulerAngles = new Vector3(0f, 0f,
				 Mathf.Atan2(-transform.position.y + targets[i].transform.position.y, -transform.position.x + targets[i].transform.position.x) * Mathf.Rad2Deg);
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			//rigid.isKinematic = false;
			m_Anim.SetBool("Possessed", true);
			PlayerControls();
			waitAttackTimer -= Time.deltaTime;
			if (!showConnection){
				showConnection = true;
				connection.SetActive(true);
			}
		} else {
			m_Anim.SetBool("Possessed", false);
			if (showConnection){
				showConnection = false;
				connection.SetActive(false);
			}
			//PlayerControls();
		}
		// Destroyed through being damaged
		if (life <= 0 && !destroyed){
			FinishAttack();
			m_Anim.SetBool("Destroyed", true);
		}
		// Counts down the window in which it can kill enemies
		//if (startedAttack && killTimer > 0f){
		//	killTimer -= Time.deltaTime;
		//}
	}

	// Transmits to whatever phone it's paired with
	public override void ActivateTransmit(){
		Time.timeScale = 1f;
		GameObject hand = ps.hand;
		GameObject attackMenu = ps.attackMenu;

		GameObject[] targets = GameObject.FindGameObjectsWithTag("Possessable");
		for (int i = 0; i < targets.Length; i++){
			if (targets[i].GetComponent<Telephone>() && targets[i] != gameObject && targets[i].GetComponent<Telephone>().id == id){
				targets[i].GetComponent<Telephone>().delayAttackTimer = 1;
				ps.Depossess();
				hand.transform.position = targets[i].transform.position;
				hand.GetComponent<HandController>().Possess(targets[i]);
				break;
			}
		}
	}

	// Activates the attack
	public override void ActivateAttack(){
		Time.timeScale = 1f;
		if (!destroyed && !startedAttack && delayAttackTimer <= 0 && waitAttackTimer <= 0f){
			ps.DisableDepossession();
			startedAttack = true;
			//ps.depossessable = false;
			m_Anim.SetBool("Attacking", true);
			GetComponent<AudioSource>().Play();
		}
	}

	// Makes a noise
	public override void ActivateNoise(){
		Time.timeScale = 1f;
		AddSoundRing(transform.position, 10f);
	}

	void Attack(){
		m_Anim.SetBool("Destroyed", true);
	}

	public void FinishAttack(){
		if (!destroyed){
			// Damage anything near the phone
			GenericEnemy[] g = GameObject.FindObjectsOfType<GenericEnemy>();
			for (int i = g.Length-1; i >= 0; i--){
				GameObject g2 = g[i].gameObject;
				if (g2 != gameObject){
					if (Vector2.Distance((Vector2)transform.position, (Vector2)g2.transform.position) < attackRadius){
						print(g2);
						g2.GetComponent<GenericEnemy>().DelayedDamage(10f);
					}
				}
			}

			AddSoundRing(transform.position, 6.5f);
			Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
			destroyed = true;
			m_Anim.SetBool("Attacking", false);
			GameObject d = (GameObject)Instantiate(dustCloud);
			d.transform.position = transform.position;
			ps.EnableDepossession();
			foreach (Collider2D col in collisions){
				if (col.gameObject.GetComponent<GenericEnemy>()){
					col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(10f);
				} else if (col.gameObject.GetComponent<PossessableScript>()){
					col.gameObject.GetComponent<PossessableScript>().DelayedDamage(10);
				}
			}
		}
	}
	void PlayerControls(){
		if (Input.GetMouseButtonDown(0) && ps.attackMenu.activeSelf == false){
			ActivateAttack();
		}
		delayAttackTimer--;
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag != "SoundRing"){
			collisions.Add(col);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		collisions.Remove(col);
	}
	// Doesn't fucking work with the toilet for whatever fucking reason
	// Detects trains forever, for some fucking retarded ass goddamn reason
	/*
	void OnTriggerStay2D(Collider2D col){
		if (killTimer > 0f){
			if (col.gameObject.GetComponent<PossessableScript>()){
				col.gameObject.GetComponent<PossessableScript>().DelayedDamage(10);
			}
		}
	}
	*/
}
