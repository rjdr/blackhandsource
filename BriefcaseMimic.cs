using UnityEngine;
using System.Collections;

public class BriefcaseMimic : GenericEnemy {
	Animator m_anim;
	public GameObject bloodSpatter;
	bool attacking = false;
	float lowDamageRangeTimer = .4f;
	float maxDamageRangeTimer = .65f;
	float attackTimer = 0f;
	float damageVal = 8f;
	bool jumped = false;
	// Use this for initialization
	void Start () {
		m_anim = GetComponent<Animator>();
	}

	public void EndAttack(){
		m_anim.SetBool("Attacking", false);
		GetComponent<CircleCollider2D>().enabled = false;
		attacking = false;
		attackTimer = 0f;
		jumped = false;
	}

	public void StartAttack(){
		m_anim.SetBool("Attacking", true);
		attacking = true;
	}

	public void StartDamaging(){
		GetComponent<CircleCollider2D>().enabled = true;
	}

	public void EndDamaging(){
		GetComponent<CircleCollider2D>().enabled = false;
	}

	// Splashes blood from a consumed object
	void SpillBlood(Transform target){
		float direction = transform.localScale.x;
		direction = (direction > 0) ? 1f : -1f;
		for (int i = 0; i < 5; i++){
			GameObject b = Instantiate(bloodSpatter);
			Vector3 tempPosition = target.transform.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += UnityEngine.Random.Range(-1f, 1f);
			tempPosition.z += UnityEngine.Random.Range(-1f, 1f);
			tempAngles.z = UnityEngine.Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= UnityEngine.Random.Range(.5f, 1f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(1f, 5f)*direction, UnityEngine.Random.Range(-3f, 1f));
		}
	}
	// Update is called once per frame
	void Update () {
		if (justDamaged){
			justDamaged = false;

		}
		if (attacking){
			attackTimer += Time.deltaTime;
			if (attackTimer >= lowDamageRangeTimer && !jumped){
				jumped = true;
				//GetComponent<Rigidbody2D>().velocity = new Vector2(4f, 2f);
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.GetComponent<GenericEnemy>()){
			StartAttack();
		}
	}
	void OnTriggerStay2D(Collider2D col){
		if (attackTimer >= lowDamageRangeTimer && attackTimer <= maxDamageRangeTimer){
			if (col.GetComponent<GenericEnemy>()){
				col.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			}
		}
	}
}
