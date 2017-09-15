using UnityEngine;
using System.Collections;

public class IllumHead : MonoBehaviour {
	// The model which we can rotate separately
	Transform Model;

	public Vector3 revolveSpeed = new Vector3(0, 120f, 0f);
	Vector3 lastPosition;
	Vector3 lastAngle;				// Angles we last had before starting the attack
	Vector3 lastModelAngle;

	Vector3 destAngle;

	Vector3 finishedAngle;			// Angle we have once attack ends and we'll be returning from
	Vector3 finishedModelAngle;
	bool rotating = true;

	// Moving to/away from the attack point
	bool movingAway = true;
	float moveToDestTimer = 0f;
	const float maxMoveToDestTimer = .5f;
	public Vector3 awayDest;

	// For controlling the laser's functions
	bool startedAttack = false;
	float attackTimer = 0f;
	const float prepareAttackTimer = 1f;
	const float startAttackTimer = 1f + prepareAttackTimer;
	const float startTurnLaserTimer = startAttackTimer + 1f;
	const float maxAttackTimer = 8f;
	GameObject LaserObj;
	GameObject laserChild;

	// Where we move to while attacking
	public Vector3 nextPoint;
	public Vector3 finalAttackDirection;

	// Move back to the WaterSpirit's body
	bool movingBack = false;
	float moveBackTimer = 0f;
	const float maxMoveBackTimer = 1f;

	float particleInterval = 0f;
	float nextParticleInterval = .05f;
	// Use this for initialization
	void Start () {
		LaserObj = (GameObject)Resources.Load("Laser");

		Model = transform.Find("Model");
	}

	public void StartMoveAway(){
		rotating = false;
		movingAway = true;
	}

	public void EndAttack(){
		particleInterval = 0f;
		startedAttack = false;
		attackTimer = 0;
		rotating = true;
		movingBack = false;
		moveBackTimer = 0f;
		moveToDestTimer = 0f;
		movingAway = true;

		if (laserChild != null){
			Destroy(laserChild, 0f);
			laserChild = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Rotates around the WaterSpirit
		if (rotating){
			transform.RotateAround(transform.parent.position, Vector3.up, revolveSpeed.y * Time.deltaTime);
			lastPosition = transform.position;
			lastAngle = transform.eulerAngles;
			lastModelAngle = Model.localEulerAngles;
			// Angle we want the heads to move to
			destAngle = lastAngle;
			if (transform.name.Contains("2")){
				destAngle.y = -180f;
			} else {
				destAngle.y = 180f;
			}
		} else {
			// Moves away from the spirit to start the attack
			if (movingAway){
				moveToDestTimer += Time.deltaTime;
				transform.position = Vector3.Lerp(lastPosition, awayDest, moveToDestTimer / maxMoveToDestTimer);
				transform.eulerAngles = Vector3.Lerp(lastAngle, destAngle, moveToDestTimer / maxMoveToDestTimer);
				if (moveToDestTimer > maxMoveToDestTimer){
					//moveToDestTimer = 0f;
					movingAway = false;
				}
			}
			attackTimer += Time.deltaTime;
			// Moves the heads to face down
			if (attackTimer < prepareAttackTimer){
				transform.up = Vector3.RotateTowards(transform.up, Vector3.up, 3f*Time.deltaTime, 1f);
			}
			// Spins up and spawns particles
			if (attackTimer < startAttackTimer){
				Model.localEulerAngles += new Vector3(0f, 720f*Time.deltaTime, 0f);
				if (attackTimer > particleInterval){
					particleInterval += nextParticleInterval;
					SpawnParticle(0f);
				}
				Camera.main.GetComponent<CameraTarget>().AddShake(.01f);
			}
			// Starts attack once in position
			if (attackTimer > startAttackTimer && !movingBack){
				// Release laser
				if (attackTimer > startAttackTimer && !startedAttack){
					startedAttack = true;
					laserChild = (GameObject)Instantiate(LaserObj, transform);
					laserChild.transform.localPosition = new Vector3(0f, 0f, 0f);
					laserChild.transform.right = -transform.up;
				}
				Camera.main.GetComponent<CameraTarget>().AddShake(.05f);
				// Quickly rotates
				Model.localEulerAngles += new Vector3(0f, 1460f*Time.deltaTime, 0f);
				if (attackTimer > maxAttackTimer){
					awayDest = transform.position;
					finishedAngle = transform.eulerAngles;
					finishedModelAngle = Model.localEulerAngles;
					movingBack = true;
					Camera.main.GetComponent<CameraTarget>().AddShake(.15f);
				}
				// Turns the Pyramids to face each other
				transform.up = Vector3.RotateTowards(transform.up, finalAttackDirection, .35f*Time.deltaTime, 1f);
				// Move toward the center of the screen while attacking
				transform.position = Vector3.Lerp(awayDest, nextPoint, (attackTimer - startAttackTimer) / (maxAttackTimer - startAttackTimer));
			}
			// Moves towards the spirit to finish the attack
			// Reset all major values
			if (movingBack){
				// Destroys the lasers
				if (laserChild != null){
					Destroy(laserChild, 0f);
					laserChild = null;
				}
				moveBackTimer += Time.deltaTime;
				transform.position = Vector3.Lerp(awayDest, lastPosition, moveBackTimer / maxMoveBackTimer);
				transform.eulerAngles = Vector3.Lerp(finishedAngle, lastAngle, moveBackTimer / maxMoveBackTimer);
				Model.localEulerAngles = Vector3.Lerp(finishedAngle, lastModelAngle, moveBackTimer / maxMoveBackTimer);
				if (moveBackTimer > maxMoveBackTimer){
					//moveToDestTimer = 0f;
					rotating = true;
					movingBack = false;
					moveBackTimer = 0f;
					attackTimer = 0f;
					moveToDestTimer = 0f;
					movingAway = true;
					EndAttack();
					// Marks the parent for attack completion
					transform.parent.GetComponent<WaterSpirit>().pyramidBeamFinished = true;
				}
			}
		}
	}

	void SpawnParticle(float dist){
		GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSpriteAdditive"));
		Vector3 tempPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0f, 0f);
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
