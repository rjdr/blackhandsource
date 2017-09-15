using UnityEngine;
using System.Collections;

public class WallCheck : MonoBehaviour {
	int madeContact = 0;
	// Use this for initialization
	void Start () {
	
	}

	// If we've gone more than 2 frames without a collision, report there being no collision with a wall
	public bool IsColliding(){
		return madeContact >= 0;
	}

	// Update is called once per frame
	void Update () {
		madeContact--;
	}
	void OnTriggerStay2D(Collider2D col){
		if (!col.isTrigger){
			madeContact = 2;
		}
	}
}
