using UnityEngine;
using System.Collections;

public class MouthBullet : MonoBehaviour {
	Vector3 velocity;
	float timer = .6f;
	Rigidbody2D rb;
	SpriteRenderer sr;
	public GameObject spawn;
	float damageVal = 2f;
	float speed = 3.5f;
	float moveAngle = 0f;
	int runs = 0;

	GameObject player;
	HandController hc;
	ArrayList pastHits = new ArrayList();

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		rb = GetComponent<Rigidbody2D>();
		rb = GetComponent<Rigidbody2D>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		player = GameObject.Find("TheHand");
		
		//CreateParticle(180f);
		//CreateParticle(0f);

		hc = GameObject.Find("TheHand").GetComponent<HandController>();

		//SetAngle();
	}
	// Creates a particle that circles around the punch object
	void CreateParticle(float angle){
	}
	// Sets the spawn. The velocity is derived from the object's RB
	public void SetSpawn(GameObject s){
		spawn = s;
		//velocity = spawn.GetComponent<Rigidbody2D>().velocity;
		//rb.velocity = velocity;
		//SetAngle();
		//rb.velocity = new Vector2(speed * Mathf.Cos(moveAngle), speed * Mathf.Sin(moveAngle));
	}
	// Sets the angle to face the direction of movement
	public void SetAngle(float z){
		moveAngle = z*Mathf.Deg2Rad;
		//transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(rb.velocity.y, rb.velocity.x)*Mathf.Rad2Deg + -90f);
		Vector3 tempAngle = transform.eulerAngles;
		tempAngle.z = moveAngle*Mathf.Rad2Deg;
		transform.eulerAngles = tempAngle;
		//GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(moveAngle), speed * Mathf.Sin(moveAngle));
	}
	// Update is called once per frame
	void FixedUpdate(){
		rb.velocity = new Vector2(speed * Mathf.Cos(moveAngle), speed * Mathf.Sin(moveAngle));
		timer -= Time.fixedDeltaTime;
		//transform.localScale *= 1.05f;
		runs ++;
		if (timer <= .225f){
			//rb.velocity *= .9f;
			Color tempColor = sr.color;
			//tempColor.a *= .91f;
			//sr.color = tempColor;
		}
		if (timer <= 0f){
			//Destroy(gameObject, 0f);
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (!col.collider.isTrigger && col.collider.gameObject.tag != "Player"){
			GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		} else if (col.collider.gameObject.tag == "Player"){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player"){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			//GetComponent<Shatterable>().Shatter();
			//Destroy(gameObject, 0f);
		}
		if (col.gameObject != spawn){
			// Destroy bullets it collides with
			if (col.gameObject.tag == "Bullet"){
				//Destroy(col.gameObject, 0f);
			}
			// Damage enemy && destroy self
			else if (!col.isTrigger){

			}
		}
	}
}
