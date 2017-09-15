using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
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
	// Use this for initialization
	void Start () {
		destination = transform.Find("Destination");
		laserTip = transform.Find("LaserEnd");
		endPoint = laserTip.localPosition;
		laserLength = Mathf.Abs(endPoint.x);

		laser = transform.Find("Laser");
		laserGlow = transform.Find("LaserGlow");
	}
	
	// Update is called once per frame
	void Update () {
		GetLength();
	}

	public void SetSpawn(GameObject s){
		spawn = s;
	}

	public void GetLength(){
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = destination.position - transform.position;
		float distance = laserLength;
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		GameObject closestCollisionObject = null;
		float closestCollisionDistance = laserLength;
		rayDirection.Normalize();
		// Find the closest hit
		hits = Physics2D.RaycastAll(transform.position, rayDirection, distance);
		for (int i = 0; i < hits.Length; i++){
			RaycastHit2D r = hits[i];
			if (!r.collider.isTrigger && r.collider.gameObject != spawn && r.collider.gameObject != gameObject && r.collider.gameObject != laser.gameObject){
				// Ignore one way platforms
				if (r.transform.GetComponent<PlatformEffector2D>() && r.transform.GetComponent<PlatformEffector2D>().useOneWay){
					continue;
				}
				// See what wall we hit, and reset the grab reach based on that
				float dist = Vector3.Distance(r.point, transform.position);
				if (dist < closestCollisionDistance){
					closestCollisionObject = r.collider.gameObject;
					closestCollisionDistance = dist;
					closestCollision = r.point;
				}
				if (dist < distance){
					clearPath = false;
					//return;
				}
			}
		}

		// Damages player if it hits it
		if (closestCollisionObject.tag == "Player"){
			if (closestCollisionObject.GetComponent<GenericEnemy>()){
				closestCollisionObject.GetComponent<GenericEnemy>().DelayedDamage(2f);
			}
		}

		Vector3 tempPosition = laserTip.localPosition;

		// Sets scaled based on currLength / maxLength
		Vector3 tempLaserScale = laser.localScale;
		tempLaserScale.x = closestCollisionDistance/laserLength;
		if (adjustScale){
			laserTip.localPosition = tempPosition;
			laser.localScale = tempLaserScale;
			laserGlow.localScale = tempLaserScale;
		} else {
			// Temp lasers will go away
			timer -= Time.deltaTime;
			if (timer < 0f){
				Destroy(gameObject, 0f);
			}
		}
		// Adds shatter particles
		shatterParticleTimer += Time.deltaTime;
		// Spawns particles at random points along the laser's length
		if (shatterParticleTimer >= nextShatterParticle){
			nextShatterParticle += .05f;
			SpawnParticle(closestCollisionDistance);
			SpawnParticle(closestCollisionDistance);
		}
	}

	void SpawnParticle(float dist){
		GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSpriteAdditive"));
		Vector3 tempPos = transform.position;
		tempPos += (laserGlow.transform.position - transform.position) * Random.Range(0f, dist/laserLength);
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

}
