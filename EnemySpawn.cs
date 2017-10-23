using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour {
	public bool spawnsOnDeath = false;
	public bool stayAlert = false;
	public float spawnTimer = 0f;
	public float maxSpawnTimer = 5f;
	// Number of enemies spawned
	int totalSpawned = 0;
	public int maxSpawnLimit = 4;

	public GameObject enemy;
	ArrayList enemies = new ArrayList();
	// Use this for initialization
	void Start () {
	
	}

	// Triggers an enemy to attack the target
	void ActivateEnemyAlert(){
		enemy.GetComponent<GenericEnemy>().Alert();
	}
	
	// Update is called once per frame
	void Update () {
		if (stayAlert){
			foreach (GameObject g in enemies){
				g.GetComponent<GenericEnemy>().Alert();
			}
		}
	}

	// Spawns enemies when in range
	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player"){
			if (spawnsOnDeath){
				// Clears out null enemies
				for (int i = (enemies.Count - 1); i >= 0; i--){
					if (enemies[i] == null){
						enemies.RemoveAt(i);
					}
				}
				// Adds a new enemy
				if (enemies.Count == 0){
					GameObject g = (GameObject)Instantiate(enemy);
					g.transform.position = transform.position;
					enemies.Add(g);

				}
			}
		}
	}
}
