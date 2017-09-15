using UnityEngine;
using System.Collections;

public class LaserCollider : MonoBehaviour {
	float damageVal = 2.5f;
	float startTimer = .25f;
	// Use this for initialization
	void Start () {
		
	}
	void Update(){
		// Enforces a small delay before it can damage objects
		if (startTimer > 0f){
			startTimer -= Time.deltaTime;
		}
	}
	
	void OnTriggerStay2D(Collider2D col) {
		//if (col.gameObject.GetComponent<Life>() && startTimer <= 0f){
		//	col.gameObject.GetComponent<Life>().DelayedDamage(damageVal);
		//}
	}
}
