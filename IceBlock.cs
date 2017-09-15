using UnityEngine;
using System.Collections;

public class IceBlock : GenericEnemy {
	bool startedMelting = false;
	float meltTimer = 0f;
	float maxMeltTimer = 2f;
	float numWaterDrops = 0;
	float maxNumWaterDrops = 20f;
	Vector3 startScale;

	GameObject waterSpatter;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
		waterSpatter = (GameObject)Resources.Load("WaterSpatter");
		//StartMelting();
	}

	public override void Heat(){
		StartMelting();
	}

	// Update is called once per frame
	void Update () {
		if (startedMelting){
			Melt();
		}
	}

	public void StartMelting(){
		startedMelting = true;
	}

	// Melts the box (shrinks until it turns to nothing)
	void Melt(){
		meltTimer += Time.deltaTime;
		transform.localScale = Vector3.Lerp(startScale, new Vector3(0f, 0f, 0f), meltTimer/maxMeltTimer);
		if (meltTimer > (maxMeltTimer/maxNumWaterDrops * numWaterDrops)){
			numWaterDrops++;
			GameObject b = Instantiate(waterSpatter);
			b.GetComponent<BloodSpatter>().parent = gameObject;
			Vector3 tempPosition = transform.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += Random.Range(-1f, 1f);
			tempPosition.y += Random.Range(-1f, 1f);
			tempPosition.z += Random.Range(-1f, 1f);
			tempAngles.z = Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= Random.Range(.5f, 1f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 1f));
		}

		if (meltTimer >= maxMeltTimer){
			Destroy(gameObject, 0f);
		}
	}
}
