using UnityEngine;
using System.Collections;

public class Briefcase : GenericEnemy {
	// Use this for initialization
	void Start () {
		// Sets all the child objects to be disabled
		foreach (Transform t in transform){
			t.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Destroys the object and makes its child object appear, if it exists
	void Death(){
		// Reenables all child objects
		foreach (Transform t in transform){
			t.gameObject.SetActive(true);
			t.parent = null;
		}

		GetComponent<Shatterable>().Shatter();
		Destroy(gameObject, 0f);
	}

	// Damages everything it touches with a small explosion
	void OnTriggerEnter2D(Collider2D col){
		// Dies from being slapped with the Hand's grab
		if (col.gameObject.name == "GrabPivot"){
			Death();
		}
	}
}
