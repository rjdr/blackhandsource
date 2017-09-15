using UnityEngine;
using System.Collections;

public class TankBullet : MonoBehaviour {
	float damageVal = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Destroys bullet if it goes behind the camera and is thus just traveling forever
	void Update () {
		if (transform.position.z < Camera.main.transform.position.z){
			Destroy(gameObject, 0f);
		}
	}

	// Destroys 3D objects that it collides with and destroys itself
	void OnCollisionEnter(Collision col){
		Camera.main.GetComponent<CameraTarget>().AddShake(.3f);
		print(col.gameObject);
		if (col.gameObject.GetComponent<GenericEnemy>()){
			col.gameObject.GetComponent<GenericEnemy>().InstantDamage(damageVal);
		}
		if (col.gameObject.tag != "Boss"){
			Destroy(col.gameObject, 0f);
		}

		Destroy(gameObject, .1f);
	}
}
