using UnityEngine;
using System.Collections;

public class Torpedo : MonoBehaviour {
	public GameObject bubble;
	public GameObject explosion;
	float damageValue = 1.25f;
	float deathTimer = 0f;
	float maxDeathTimer = 4f;
	float bubbleInterval = .1f;
	float incrBubbleInterval = .125f;
	bool movingDown = true;
	public bool reflected = false;
	float maxSpeed = 20f;
	float accel = 1f;
	Rigidbody2D rb;
	public Sprite torpedoReflected;
	// Use this for initialization
	void Start () {
		//bubble = (GameObject)Resources.Load("Bubble");
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2(0f, -8f);
	}
	
	// Update is called once per frame
	void Update () {
		// Moves down, slows to 0 velocity and once it finishes, starts moving forward
		if (movingDown){
			rb.velocity += new Vector2(0f, 40f) * Time.deltaTime;
			if (rb.velocity.y > 0f){
				movingDown = false;
			}
		} else {
			Vector2 tempVel = rb.velocity;
			tempVel += new Vector2(44f * Time.deltaTime * accel, 0f);
			if (tempVel.x >= maxSpeed){
				tempVel.x = maxSpeed;
			}
			// Slight up-down wave motion
			tempVel.y = Mathf.Cos(Time.time*5f) * .8f;
			rb.velocity = tempVel;
		}
		deathTimer += Time.deltaTime;
		// Adds a bubble
		if (deathTimer >= bubbleInterval){
			bubbleInterval += incrBubbleInterval * Random.Range(.5f, 1f);
			GameObject b = (GameObject)Instantiate(bubble);
			b.transform.position = transform.position + new Vector3(-.5f, 0f, 0f);
		}
		// Destroys the torpedo
		if (deathTimer >= maxDeathTimer){
			Destroy(gameObject, 0f);
		}
	}

	// Damages things once it hits them
	void OnTriggerEnter2D(Collider2D col){
		if ((col.tag != "Player" && !reflected) || (col.tag == "Player" && reflected)){
			// Once it hits an enemy, explode and do damage
			if (col.gameObject.GetComponent<GenericEnemy>()){
				Camera.main.GetComponent<CameraTarget>().AddShake(.3f);
				col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageValue);
				GameObject g = Instantiate(explosion);
				g.transform.position = transform.position;
				Destroy(gameObject, 0f);
			}
		}
	}

	// Reflects back
	public void Reflect(){
		if (!reflected){
			reflected = true;
			rb.velocity *= .1f;
			accel = -1f;
			GetComponent<SpriteRenderer>().sprite = torpedoReflected;
			GetComponent<SpriteRenderer>().flipX = true;
		}
	}
}
