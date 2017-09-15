using UnityEngine;
using System.Collections;

public class MovingWave : MonoBehaviour {
	Vector3 vel = new Vector3(0f, 0f, 0f);
	Vector3 accel = new Vector3(0f, -70f, 0f);
	Vector3 minVelocity = new Vector3(0f, -20f, 0f);
	float damageValue = 2f;
	bool goingDown = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (goingDown){
			vel.z = 0f;
			vel += accel * Time.deltaTime;
			vel = Vector3.ClampMagnitude(vel, 20f);
		}
		transform.position += vel * Time.deltaTime;
		if (goingDown && transform.position.z < -40f){
			Destroy(gameObject, 0f);
		}
	}

	public void StartUp(){
		goingDown = false;
		vel = new Vector3(0f, 8f, -23f);
	}

	public void StartDown(){
		goingDown = true;
		Camera.main.GetComponent<CameraTarget>().AddShake(1f);
	}

	// Damages only when going down
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && goingDown){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageValue);
		}
	}
}
