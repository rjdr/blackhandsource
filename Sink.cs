using UnityEngine;
using System.Collections;

public class Sink : GenericEnemy {
	ArrayList collisions = new ArrayList();
	PossessableScript ps;
	Animator m_anim;
	bool destroyed = false;
	bool startedAttack = false;
	GameObject smoke;
	float waitAttackTimer = .2f;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		m_anim = GetComponent<Animator>();
		smoke = transform.Find("Smoke").gameObject;
		smoke.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			PlayerControls();
		}
	}
	void PlayerControls(){
		waitAttackTimer -= Time.deltaTime;
		if (Input.GetMouseButtonDown(0) && waitAttackTimer < 0f && ps.attackMenu.activeSelf == false){
			ActivateAttack();
		}
	}
	public override void ActivateAttack(){
		Time.timeScale = 1f;
		if (!destroyed && !startedAttack){
			AddSoundRing(transform.position, 3f);
			ps.DisableDepossession();
			startedAttack = true;
			ps.depossessable = false;
			m_anim.SetBool("Attacking", true);
			Camera.main.GetComponent<CameraTarget>().AddShake(.15f);
			//GetComponent<AudioSource>().Play();
			smoke.SetActive(true);
		}
	}
	// Enable the box collider and damage colliding enemies
	public void SprayWater(){
		Camera.main.GetComponent<CameraTarget>().AddShake(.1f);
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<CircleCollider2D>().enabled = false;

		foreach (Collider2D col in collisions){
			if (col == null){
				continue;
			}
			print(col.gameObject);
			if (col.gameObject.GetComponent<GenericEnemy>()){
				col.gameObject.GetComponent<GenericEnemy>().Heat();
				col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(5f);
			} else if (col.transform.parent && col.transform.parent.GetComponent<GenericEnemy>()){
				col.transform.parent.GetComponent<GenericEnemy>().Heat();
				col.transform.parent.GetComponent<GenericEnemy>().DelayedDamage(5f);
			}else if (col.gameObject.GetComponent<PossessableScript>()){
				col.gameObject.GetComponent<PossessableScript>().DelayedDamage(5);
			}
		}
	}
	public override void Death(){
		ps.EnableDepossession();
		ps.Depossess();
		m_anim.SetBool("Destroyed", true);
		destroyed = true;
		smoke.SetActive(false);
	}
	public void Shatter(){
		ps.EnableDepossession();
		if (ps.possessed){
			ps.Depossess();
		}
		Camera.main.GetComponent<CameraTarget>().AddShake(.2f);
		AddSoundRing(transform.position, 5f);
		m_anim.SetBool("Destroyed", true);
		ps.possessable = false;
		smoke.gameObject.SetActive(false);
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
