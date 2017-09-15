using UnityEngine;
using System.Collections;

public class DamagingObject : GenericEnemy {
	public float damageVal = 1f;

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>()){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
		}
	}
}
