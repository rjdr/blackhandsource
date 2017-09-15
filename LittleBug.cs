using UnityEngine;
using System.Collections;

public class LittleBug : GenericEnemy {
	float damageVal = 3f;
	public GameObject explosion;
	Rigidbody2D rb;
	float speed = .5f;
	float direction = -1f;
	float minDist = 30f;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (life <= 0f){
			Die();
		}
		Transform p = Camera.main.GetComponent<CameraTarget>().GetTargetReference();
		// Moves towards player when relatively close
		if (Vector3.Distance(p.position, transform.position) < minDist){
			if (p.position.x > transform.position.x){
				rb.velocity = new Vector2(speed, rb.velocity.y);
			} else {
				rb.velocity = new Vector2(-speed, rb.velocity.y);
			}
			direction = (rb.velocity.x < 0f) ? -1 : 1;
		} else {
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}
		transform.localScale = new Vector3(direction, 1f, 1f);
	}
	void Die(){
		GetComponent<Shatterable>().Shatter();
		GameObject e = (GameObject)Instantiate(explosion);
		e.transform.position = transform.position;
		e.transform.localScale *= .5f;
		AddSoundRing(transform.position, 7f);

		Destroy(gameObject, 0f);
	}
	// Damages everything it touches with a small explosion
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && !col.gameObject.GetComponent<LittleBug>() && !col.gameObject.GetComponent<Turret>() && !col.gameObject.GetComponent<RotateGun>()){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			Die();
		}
		// Dies from being slapped with the Hand's grab
		if (col.gameObject.name == "GrabPivot"){
			Die();
		}
	}
}
