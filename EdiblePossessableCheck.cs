using UnityEngine;
using System.Collections;

public class EdiblePossessableCheck : MonoBehaviour {
	ArrayList collisions = new ArrayList();
	HandController hc;
	// Use this for initialization
	void Start () {
		hc = transform.parent.gameObject.GetComponent<HandController>();
	}
	
	// If an object that's possessable or edible is near the player, set a flag to enable a visual cue to do so
	void OnTriggerStay2D (Collider2D col) {
		if (col.gameObject.GetComponent<PossessableScript>()){
			if (col.gameObject.GetComponent<PossessableScript>().edible || col.gameObject.GetComponent<PossessableScript>().possessable){
				hc.SetNearbyEdiblePossessibleEnemy();
			}
		}
	}
}
