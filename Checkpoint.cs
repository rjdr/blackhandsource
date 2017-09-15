using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Checkpoint : MonoBehaviour {
	Transform spawnPoint;
	GameObject[] otherCheckpoints;
	bool lastCollidedWith = false;
	public int checkpointOrder = 0;
	bool alreadyWrote = false;
	GameObject hand;

	string checkpointLocation = "checkpoint.txt";
	// Use this for initialization
	void Start () {
		spawnPoint = transform.Find("checkpointspawn");
		otherCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		hand = GameObject.Find("TheHand");
		GetComponent<SpriteRenderer>().enabled = false;
		spawnPoint.GetComponent<SpriteRenderer>().enabled = false;
	}

	// Sets whether this checkpoint was the last one to be touched
	void SetLastCollided(bool b){
		lastCollidedWith = b;
	}
	
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.tag == "Player" && alreadyWrote == false){
			for (int i = 0; i < otherCheckpoints.Length; i++){
				if (otherCheckpoints[i] == gameObject){
					SetLastCollided(true);
					alreadyWrote = true;
					//WriteCheckpoint(hand.transform);
					CheckpointData.WriteCheckpoint(hand.transform, spawnPoint);
				} else {
					SetLastCollided(false);
				}
			}
		}
	}

	// Writes desired spawn position
	/*
	void WriteCheckpoint(Transform obj, Transform spawnPoint){
		StreamWriter sr;
		sr = File.CreateText(checkpointLocation);

		sr.WriteLine(""+spawnPoint.position.x);
		sr.WriteLine(""+spawnPoint.position.y);
		sr.WriteLine(""+obj.position.z); 	// We'll want to write the player's depth instead of the checkpoint's
		sr.Close();
	}
	*/
}
