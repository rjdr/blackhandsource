using UnityEngine;
using System.Collections;

public class HeavyItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Destroys an item that it touches
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && GetComponent<Rigidbody2D>().velocity.magnitude > 2f){
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(30f);
		}
	}
}
