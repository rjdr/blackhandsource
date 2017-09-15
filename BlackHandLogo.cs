using UnityEngine;
using System.Collections;

public class BlackHandLogo : MonoBehaviour {
	public Transform text;
	float maxRotateSpeed = -387.3f;
	float rotateSpeed = -360f;
	float slowDownTime = 0f;
	float maxSlowDownTime = 5.5f;
	// Use this for initialization
	void Start () {
	
	}

	// whether the text spin is stopped
	public bool TextStopped(){
		return (slowDownTime >= maxSlowDownTime);
	}

	// Update is called once per frame
	void Update () {
		Vector3 tempPosition = transform.position;
		tempPosition.x = Camera.main.transform.position.x;
		tempPosition.y = Camera.main.transform.position.y;
		transform.position = tempPosition;

		Vector3 tempAngles = text.transform.eulerAngles;
		tempAngles.z += Time.deltaTime * rotateSpeed;
		text.transform.eulerAngles = tempAngles;

		rotateSpeed = Mathf.Lerp(maxRotateSpeed, 0f, slowDownTime/maxSlowDownTime);
		if (slowDownTime >= maxSlowDownTime){
			//text.transform.eulerAngles = Vector3.RotateTowards(text.transform.eulerAngles, new Vector3(0f, 0f, 0f), 1f*Time.deltaTime, 1f);
		}
		slowDownTime += Time.deltaTime;
	}
}
