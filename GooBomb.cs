using UnityEngine;
using System.Collections;

public class GooBomb : MonoBehaviour {
	float timer = 6f;
	GameObject dust;
	// Use this for initialization
	void Start () {
		dust = (GameObject)Resources.Load("Dustcloud");
	}
	// Destroys the bomb and injures objects near it
	void Explode(){
		dust = (GameObject)Instantiate(dust);
		dust.transform.position = transform.position;
		Destroy(gameObject, 0f);
	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f){
			Explode();
		}
	}
	// Adds an explosion once it touches an object
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.GetComponent<GenericEnemy>()){
			Explode();
		}
	}
}
