using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class CheckpointData : MonoBehaviour {
	static string checkpointLocation = "checkpoint.txt";
	// Writes desired spawn position
	public static void WriteCheckpoint(Transform obj, Transform spawnPoint){
		StreamWriter sr;
		sr = File.CreateText(checkpointLocation);
		sr.WriteLine(""+SceneManager.GetActiveScene().name);
		sr.WriteLine(""+spawnPoint.position.x);
		sr.WriteLine(""+spawnPoint.position.y);
		sr.WriteLine(""+obj.position.z); 	// We'll want to write the player's depth instead of the checkpoint's
		// Writes the possession history
		for (int i = 0; i < obj.GetComponent<HandController>().hasPossessedBefore.Count; i++){
			sr.WriteLine(""+(string)obj.GetComponent<HandController>().hasPossessedBefore[i]);
		}
		sr.Close();
	}
	// Deletes checkpoint file
	public static void DeleteCheckpointData(){
		File.Delete(checkpointLocation);
	}

	// Loads the checkpoint data
	public static Vector3 LoadCheckpoint(){
		if (!File.Exists("checkpoint.txt")){
			return new Vector3(0f, 0f, 0f);
		}
		StreamReader sr = new StreamReader("checkpoint.txt");
		Vector3 pos = new Vector3(0f, 0f, 0f);
		string sceneName = sr.ReadLine();
		if (SceneManager.GetActiveScene().name != sceneName){
			Debug.Log("Didn't load checkpoint data. Scene has changed.");
		}
		pos.x = (float)Convert.ToDouble(sr.ReadLine());
		pos.y = (float)Convert.ToDouble(sr.ReadLine());
		pos.z = (float)Convert.ToDouble(sr.ReadLine());
		sr.Close();

		return pos;
	}
}
