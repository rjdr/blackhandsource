using UnityEngine;
using System.Collections;

public class DeathBoundary : MonoBehaviour {
	// Object that instantly kills upon contact
	// Use this for initialization
	void Start () {
		// Disable the sprite renderer so that it's not visible during gameplay
		GetComponent<SpriteRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
