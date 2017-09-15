using UnityEngine;
using System.Collections;

public class RecordPlayer : GenericEnemy {
	PossessableScript ps;
	Animator m_Anim;
	bool destroyed = false;		// You can still warp through the phone once it's destroyed, but you can't attack or stay in it
	bool startedAttack = false;
	public int id;
	float killTimer = 0f;
	ArrayList collisions = new ArrayList();
	public int delayAttackTimer = 0;
	
	GameObject dustCloud;

	GameObject bloodSpatter;
	Vector3 baseScale;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		m_Anim = GetComponent<Animator>();

		bloodSpatter = (GameObject)Resources.Load("BloodSpatter");
		baseScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			//rigid.isKinematic = false;
			m_Anim.SetBool("Possessed", true);
			if (startedAttack){
				Camera.main.GetComponent<CameraTarget>().AddShake(.02f);
			}
			PlayerControls();
		} else {
			m_Anim.SetBool("Possessed", false);
			//PlayerControls();
		}
		// Counts down the window in which it can kill enemies
		//if (startedAttack && killTimer > 0f){
		//	killTimer -= Time.deltaTime;
		//}
	}

	// Splashes blood from a consumed object
	void SpillBlood(Transform target){
		float direction = target.transform.position.x;
		direction = (direction > transform.position.x) ? 1f : -1f;
		for (int i = 0; i < 25; i++){
			GameObject b = Instantiate(bloodSpatter);
			Vector3 tempPosition = target.transform.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += Random.Range(-1f, 1f);
			tempPosition.y += Random.Range(-3f, 2f);
			tempPosition.z += Random.Range(-1f, 1f);
			tempAngles.z = Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= Random.Range(.5f, 1f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(1f, 5f)*direction, Random.Range(-3f, 1f));
		}
	}
	
	// Activates the attack
	public override void ActivateAttack(){
		Time.timeScale = 1f;
		Vector3 tempScale = baseScale * 1f;
		tempScale.x *= 1.25f;
		transform.localScale = tempScale;
		if (!destroyed && !startedAttack && delayAttackTimer <= 0){
			ps.DisableDepossession();
			startedAttack = true;
			ps.depossessable = false;
			m_Anim.SetBool("Attacking", true);
		}
	}
	
	void Attack(){
		//m_Anim.SetBool("Destroyed", true);
	}
	
	public void FinishAttack(){
		//if (!destroyed){
		startedAttack = false;
		Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
		//destroyed = true;
		m_Anim.SetBool("Attacking", false);
		ps.EnableDepossession();
		foreach (Collider2D col in collisions){
			if (col && col.gameObject.GetComponent<GenericEnemy>()){
				col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(10f);
				if (col.gameObject.GetComponent<PossessableScript>() && col.gameObject.GetComponent<PossessableScript>().isHuman){
					SpillBlood(col.transform);
				}
			}
		}
		collisions = new ArrayList();
		transform.localScale = baseScale;
		//}
	}
	void PlayerControls(){
		if (Input.GetMouseButtonDown(0) && ps.attackMenu.activeSelf == false){
			ActivateAttack();
		}
		delayAttackTimer--;
	}
	void OnTriggerStay2D(Collider2D col){
		if (!collisions.Contains(col)){
			collisions.Add(col);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		collisions.Remove(col);
	}
}
