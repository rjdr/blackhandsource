using UnityEngine;
using System.Collections;

// Ladder that can be climbed up/down when in Human form
public class ClimbableLadder : MonoBehaviour {
	Transform TopPoint;
	Transform BottomPoint;
	const int UP = 1;
	const int DOWN = -1;
	// Use this for initialization
	void Start () {
		TopPoint = transform.Find("TopPoint");
		BottomPoint = transform.Find("BottomPoint");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 StartClimb(GameObject col){
		// Determines which direction the object will move on the ladder
		// If closer to the bottom, move up; else, move down
		int direction = 0;
		Vector3 destination;
		if (Vector3.Distance(col.transform.position, BottomPoint.position) < Vector3.Distance(col.transform.position, TopPoint.position)){
			direction = UP;
			destination = TopPoint.position;
		} else {
			direction = DOWN;
			destination = BottomPoint.position;
		}
		return destination;
	}
	public int GetDirection(GameObject col){
		// Determines which direction the object will move on the ladder
		// If closer to the bottom, move up; else, move down
		int direction = 0;
		Vector3 destination;
		if (Vector3.Distance(col.transform.position, BottomPoint.position) < Vector3.Distance(col.transform.position, TopPoint.position)){
			direction = UP;
		} else {
			direction = DOWN;
		}
		return direction;
	}

	// Marks object touching ladders as having touched a ladder, letting them climb up
	void OnTriggerStay2D(Collider2D col){
		if (col.GetComponent<GenericEnemy>()){
			
			col.GetComponent<GenericEnemy>().SetTouchingLadder(true, gameObject);
		}
	}
}
