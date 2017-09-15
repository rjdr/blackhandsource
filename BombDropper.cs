using UnityEngine;
using System.Collections;

public class BombDropper : GenericEnemy {
	Transform spawnPoint;
	GameObject bomb;
	float waitAttackTimer = 0f;
	float maxWaitAttackTimer = 2f;
	bool attacking = false;
	// Use this for initialization
	void Start () {
		spawnPoint = transform.Find("Spawn");
		bomb = (GameObject)Resources.Load("GooBomb");
	}

	void StartAttack(){
		waitAttackTimer = maxWaitAttackTimer;
		GetComponent<Animator>().SetBool("Attack", true);
	}

	// Drops a bomb
	void DropBomb(){
		GameObject newBomb = (GameObject)Instantiate(bomb);
		bomb.transform.position = spawnPoint.position;

		GetComponent<Animator>().SetBool("Attack", false);
	}

	// Update is called once per frame
	void Update () {
		waitAttackTimer -= Time.deltaTime;
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && waitAttackTimer <= 0f){
			StartAttack();
		}
	}
}
