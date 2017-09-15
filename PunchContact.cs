using UnityEngine;
using System.Collections;

public class PunchContact : MonoBehaviour {
	float timer = .25f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f){
			Destroy(gameObject);
		}
	}
}
