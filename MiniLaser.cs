using UnityEngine;
using System.Collections;

public class MiniLaser : MonoBehaviour {
	Vector3 endPoint;
	float laserLength = 0f;
	Transform laserTip;
	Transform destination;

	GameObject spawn;
	Transform laser;
	Transform laserGlow;
	public bool adjustScale = true;
	float timer = 4f;
	// Adds shatter particles
	float shatterParticleTimer = 0f;
	float nextShatterParticle = .025f;
	float currSpeed = 3f;
	float maxSpeed = -18f;
	Vector3 currScale = Vector3.zero;
	Vector3 endScale;

	Material m;
	// Use this for initialization
	void Start () {
		destination = transform.Find("Destination");
		laserTip = transform.Find("LaserEnd");
		endPoint = laserTip.localPosition;
		laserLength = Mathf.Abs(endPoint.x) * transform.localScale.x;

		laser = transform.Find("Laser");
		laserGlow = transform.Find("LaserGlow");

		// Speed
		GetComponent<Rigidbody2D>().velocity = new Vector2(currSpeed, 0f);

		// Scales up fast upon appearing
		endScale = transform.localScale;
		currScale.y = currScale.y;
		transform.localScale = currScale;

		m = laser.GetComponent<LineRenderer>().material;
	}

	public void SetSpawn(GameObject s){
		spawn = s;
	}

	void Update(){
		// Speeds up
		currSpeed = Mathf.MoveTowards(currSpeed, maxSpeed, 24f*Time.deltaTime);
		GetComponent<Rigidbody2D>().velocity = new Vector2(currSpeed, 0f);
		// Rapidly expands so as to not suddenly appear
		currScale = Vector3.MoveTowards(currScale, endScale, 10f*Time.deltaTime);
		transform.localScale = currScale;
		// Shows little shatter particles
		shatterParticleTimer += Time.deltaTime;
		if (shatterParticleTimer >= nextShatterParticle){
			SpawnParticle();
			nextShatterParticle += .1f;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f){
			Destroy(gameObject, 0f);
		}
		m.mainTextureOffset += new Vector2(Time.deltaTime * 6f, 0f);
	}

	void SpawnParticle(){
		GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSpriteAdditive"));
		Vector3 tempPos = transform.position + new Vector3(-1f, 0f, 0f);
		float f = Random.Range(0f, 1f);
		Color c;
		if (f < .25f){
			c = new Color(1f, .1f, .2f);
		} else if (f < .5f){
			c = new Color(.1f, 1f, .2f);
		} else if (f < .75f){
			c = new Color(.2f, .1f, 1f);
		} else {
			c = new Color(1f, .1f, 1f);
		}
		s.GetComponent<ShatterParticleScript>().Instantiate(tempPos, c);
		s.GetComponent<ShatterParticleScript>().dy *= Random.Range(-2f, 2f);
		s.transform.localScale *= Random.Range(1f, 3f);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player"){
			Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(1f);
			Destroy(gameObject, 0f);
		}
	}
}
