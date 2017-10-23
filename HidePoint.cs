using UnityEngine;
using System.Collections;

public class HidePoint : MonoBehaviour {
	// Position relative to the barrier we're hiding behind
	public int hidingDirection = 1; 	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player"){
			if (col.GetComponent<GenericEnemy>().canHide == true && ( Input.GetAxis("Vertical") > .1f)){
				print("entered hole");
				col.GetComponent<GenericEnemy>().Hide(hidingDirection);
			}
		}
	}
}
