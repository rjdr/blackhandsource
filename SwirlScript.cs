using UnityEngine;
using System.Collections;

public class SwirlScript : MonoBehaviour {
	Vector3 endPosition;
	Vector3 startPosition;
	float speed = 480f;
	float time = 0f;
	public bool active = false;
	public bool changePosition = true;
	// Use this for initialization
	void Start () {
		if (changePosition){
			endPosition = transform.position;
			startPosition = transform.position + new Vector3(0f, 40f, 0f);
			transform.position = startPosition;
		}
	}
	public void SetActive(bool b){
		active = b;
	}
	// Update is called once per frame
	void Update() {
		if (active){
			transform.Rotate(0f, speed*Time.deltaTime, 0f);
			time += Time.deltaTime*.2f;
			if (changePosition){
				transform.position = Vector3.Lerp(startPosition, endPosition, time);
			}
		}
	}
}
