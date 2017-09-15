using UnityEngine;
using System.Collections;

public class BreakableWood: GenericEnemy {
	Object dustCloud;

	// Use this for initialization
	void Start () {
		life = 1f;
		dustCloud = Resources.Load("Dustcloud");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Spawns a few dust clouds when destroyed
	public override void DustCloud(){
		for (int i = 0; i < 3; i++){
			GameObject p = (GameObject)Instantiate(dustCloud);
			p.transform.position = transform.position;
			p.transform.localScale *= .7f;
			p.GetComponent<Dustcloud>().velocity = new Vector3(Random.Range(-1f, 1f)*Time.fixedDeltaTime, Random.Range(-1f, 1f)*Time.fixedDeltaTime,
				Random.Range(-.25f, .25f)*Time.fixedDeltaTime);
		}
		Destroy(gameObject, 0f);
	}
}
