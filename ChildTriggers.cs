using UnityEngine;
using System.Collections;

public class ChildTriggers: MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col){
		transform.parent.GetComponent<GenericEnemy>().ChildTriggered(gameObject, col.gameObject);
	}
	void OnTriggerStay2D(Collider2D col){
		transform.parent.GetComponent<GenericEnemy>().ChildTriggerStayed(gameObject, col.gameObject);
	}
}
