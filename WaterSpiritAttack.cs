using UnityEngine;
using System.Collections;

public class WaterSpiritAttack : GenericEnemy {
	Vector3 startVelocity = new Vector3(-3f, 2.5f, 0f);
	Vector3 finalVelocity;
	float speed = 20f;
	Vector3 pivot;
	float initialMoveTimer = .6f; 		// How long the object will move before adopting its new pattern
	float radius = 1.5f;
	float angleSpeed = -600f;
	float angle = -90f;
	bool gotTargetPosition = false; 	// whether we locked onto the target
	bool setVelocity = false;
	Vector3 targetPosition;
	float setTargetAngle = -600f;		// Angle at which we set the target position
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Moves slightly upwards at the start
		if (initialMoveTimer > 0f){
			transform.position += startVelocity * Time.deltaTime;
			initialMoveTimer -= Time.deltaTime;
			// Gets the pivot to rotate around (directly above the current point);
			if (initialMoveTimer <= 0f){
				pivot = transform.position + new Vector3(0f, radius, 0f);
			}
		} else {
			// Gets the position of the player and then aims
			if (!gotTargetPosition && angle <= setTargetAngle){
				gotTargetPosition = true;
				targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
			}
			// Do a quick spin
			if (angle > (-90f - 360f*3)){
				transform.position = pivot + new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angle), radius * Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
				angle += angleSpeed * Time.deltaTime;
				// Slightly grows in size
				float incr = .2f * Time.deltaTime;
				transform.localScale += new Vector3(incr, incr, incr);
				transform.localScale = Vector3.ClampMagnitude(transform.localScale, 2.5f);
			}
			else {
				if (!setVelocity){
					setVelocity = true;
					finalVelocity = speed * (targetPosition - transform.position).normalized;
				}
				transform.position += finalVelocity * Time.deltaTime;
			}

			// Then launch towards the player
		}
	}
}
