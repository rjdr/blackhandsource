using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {
	bool activeAnimation = false;
	public bool enabled = false;
	Transform lever;
	Vector3 defaultRot;
	Vector3 onRot;
	float rotSpeed = 4f;
	// Use this for initialization
	void Start () {
		lever = transform.Find("Lever");
		defaultRot = transform.eulerAngles;
		onRot = defaultRot;
		onRot.z += 45f;

		SwitchOn();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (activeAnimation){
			// Switches lever into position
			if (enabled){
				Vector3 tempPos = Vector3.MoveTowards(lever.transform.eulerAngles, onRot, rotSpeed);
				if (Mathf.Abs(tempPos.z - onRot.z) < 1f){
					activeAnimation = false;
				}
				lever.transform.eulerAngles = tempPos;
			} else {
				Vector3 tempPos = Vector3.MoveTowards(lever.transform.eulerAngles, defaultRot, rotSpeed);
				if (Mathf.Abs(tempPos.z - defaultRot.z) < 1f){
					activeAnimation = false;
				}
				lever.transform.eulerAngles = tempPos;
			}
		}
	}

	// Activates switch
	public void SwitchOn(){
		enabled = true;
		activeAnimation = true;
	}

	// Deactivates switch
	public void SwitchOff(){
		enabled = false;
		activeAnimation = true;
	}
}
