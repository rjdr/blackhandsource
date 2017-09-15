using UnityEngine;
using System.Collections;

public class Franz : GenericEnemy {
	bool gaveIntro = false;
	GameObject prisoner;
	GameObject player;
	bool possessedPrisoner = false;
	GameObject bloodSpatter;
	bool prisonerActive = true;
	bool activated = false;
	float x;

	GameObject TargetWarning;
	GameObject targetLoad;
	bool loadedWarning = false;

	GameObject autoFitCameraObject;
	Vector3 autoFitCameraObjectScale;

	// Use this for initialization
	void Start () {
		prisoner = GameObject.Find("Danilo");
		player = GameObject.Find("TheHand");
		life = 10000000;
		maxLife = 100000000f;

		bloodSpatter = (GameObject)Resources.Load("BloodSpatter");

		targetLoad = (GameObject)Resources.Load("TargetWarning");
		autoFitCameraObject = GameObject.Find("AutofitCameraObject");
		autoFitCameraObjectScale = autoFitCameraObject.transform.localScale;
		autoFitCameraObject.transform.localScale *= .9f;
	}
	
	// Update is called once per frame
	void Update () {
		if (prisonerActive){
			prisonerActive = false;
			prisoner.SetActive(false);
		}
		if (gaveIntro && !Camera.main.GetComponent<CameraTarget>().ActiveDialogue() && !possessedPrisoner){
			possessedPrisoner = true;
			prisoner.SetActive(true);
			GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
			if (tempPlayer.name != "TheHand" && tempPlayer.GetComponent<GenericEnemy>()){
				tempPlayer.GetComponent<GenericEnemy>().Depossess();
			}
			player.GetComponent<HandController>().Possess(prisoner);
			player.GetComponent<HandController>().TriggerLightning();
			life = 1f;
		}
		if (loadedWarning){
			Vector3 temp = player.transform.position;
			temp.x = x;
			player.transform.position = temp;
		}
	}
	public override void Death(){
		float direction = transform.localScale.x;
		for (int i = 0; i < 24; i++){
			GameObject b = Instantiate(bloodSpatter);
			Vector3 tempPosition = transform.position;
			Vector3 tempAngles = b.transform.eulerAngles;
			tempPosition.x += Random.Range(-1f, 1f);
			tempPosition.y += Random.Range(-1f, 1f);
			tempPosition.z += Random.Range(-1f, 1f);
			tempAngles.z = Random.Range(0, 360f);
			b.transform.position = tempPosition;
			b.transform.eulerAngles = tempAngles;
			b.transform.localScale *= Random.Range(.5f, 2f);
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-12, 12f), Random.Range(-6f, 1f));
		}
	}

	void OnTriggerStay2D(Collider2D col){		
		if (col.tag == "Player" && !gaveIntro){
			if (loadedWarning == false){
				x = col.transform.position.x;
				TargetWarning = (GameObject)Instantiate(targetLoad);
				TargetWarning.transform.parent = Camera.main.transform.Find("ColorBlindCanvasCam");
				loadedWarning = true;
				TargetWarning.transform.Find("TargetWarning").GetComponent<TargetWarning>().SetName("ARCHDUKE FERDINAND");
			} else if (loadedWarning && TargetWarning == null){
				autoFitCameraObject.transform.localScale = autoFitCameraObjectScale;
				x = col.transform.position.x;
				activated = true;
				gaveIntro = true;
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("franzfirst"));
			}

		}
	}
}
