using UnityEngine;
using System.Collections;

public class FolderObj : GenericEnemy {
	public GameObject explosion;
	public Transform spawnObject;
	public Transform[] destroyObjects;
	// Use this for initialization
	void Start () {
		if (spawnObject != null){
			spawnObject.gameObject.SetActive(false);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Player"){
			if (spawnObject != null){
				spawnObject.gameObject.SetActive(true);
			}

			// Adds explosions for the objects that are destroyed
			for (int i = 0; i < destroyObjects.Length; i++){
				GameObject e = (GameObject)Instantiate(explosion);
				e.transform.position = destroyObjects[i].position;
				Destroy(destroyObjects[i].gameObject, 0f);
			}

			AddFastText(DialogueTable.GetChat("gotfirstfolder").text, 0f, 200f, true, true);
			GetComponent<Shatterable>().Shatter();
			AddSoundRing(transform.position, 15f);
			AddSoundRing(transform.position, 15f);
			AddSoundRing(transform.position, 15f);
			Destroy(gameObject, 0f);
		}
	}
}
