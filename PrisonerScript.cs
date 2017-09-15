using UnityEngine;
using System.Collections;

public class PrisonerScript : GenericEnemy {
	bool talked = false;
	bool blobsStarted = false;
	float blobsEnabledTimer = 10f;

	public bool preBattleSequence = false;
	bool preBattleBlobsActive = false;
	float preBattleBlobsSpeed = 6f;
	float preBattleBlobTimer = 1f;
	Vector3 blobDest;

	struct blob {
		public Vector3 pos;
		public GameObject obj;
	};

	ArrayList blobs = new ArrayList();
	bool addedBlobs = false;
	Transform leftCrate;
	Transform rightCrate;
	Vector3 leftCratePos;
	Vector3 rightCratePos;
	Vector3 leftCrateStartPos;
	Vector3 rightCrateStartPos;
	float crateMoveTimer = 0f;
	float maxCrateMoveTimer = .25f;
	bool moveCratesBack = false;

	GameObject cameraFit;
	GameObject bulletBoss;
	// Use this for initialization
	void Start () {
		// Creates structs for each blob to manage them later
		if (preBattleSequence){
			foreach (Transform t in transform){
				if (t.name.Contains("blob")){
					blob b;
					b.pos = t.transform.position;
					b.obj = t.gameObject;
					blobs.Add(b);
					
					b.obj.transform.position = transform.position;
					b.obj.SetActive(false);
				}
			}
			blobDest = transform.Find("Destination").position;
			leftCrate = transform.Find("CrateL");
			rightCrate = transform.Find("CrateR");
			leftCratePos = leftCrate.position;
			rightCratePos = rightCrate.position;

			leftCrate.position += new Vector3(-4f, 0f, 0f);
			rightCrate.position += new Vector3(4f, 0f, 0f);

			leftCrateStartPos = leftCrate.position;
			rightCrateStartPos = rightCrate.position;

			cameraFit = GameObject.Find("AutofitCameraObject");
			cameraFit.SetActive(false);
			bulletBoss = GameObject.Find("BulletBoss");
			bulletBoss.GetComponent<SpriteRenderer>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Move crates to keep player from exiting
		if (moveCratesBack){
			crateMoveTimer += Time.deltaTime;
			leftCrate.position = Vector3.Lerp(leftCrateStartPos, leftCratePos, crateMoveTimer/maxCrateMoveTimer);
			rightCrate.position = Vector3.Lerp(rightCrateStartPos, rightCratePos, crateMoveTimer/maxCrateMoveTimer);
		}
		if (blobsStarted){
			blobsEnabledTimer -= Time.deltaTime;
			if (blobsEnabledTimer <= 0f){
				gameObject.SetActive(false);
			}
		}
		if (preBattleBlobsActive){
			if (preBattleBlobTimer > 0f){
				foreach (blob b in blobs){
					b.obj.transform.position += preBattleBlobsSpeed * Time.deltaTime * (b.pos - b.obj.transform.position);
				}
				preBattleBlobTimer -= Time.deltaTime;
			} else {
				foreach (blob b in blobs){
					if (b.obj == null){
						continue;
					}
					b.obj.transform.position += preBattleBlobsSpeed * 6f * b.obj.transform.localScale.x * Time.deltaTime * (blobDest - b.obj.transform.position).normalized;
				}
			}
		}
		// Activates blobs once dialogue is over
		if (talked && preBattleSequence && !Camera.main.GetComponent<CameraTarget>().ActiveDialogue()){
			AddPreBattleBlobs();
		}
	}

	public override void ChildTriggered(GameObject child, GameObject obj){
		if (child.name == "BelowCrate" && obj.tag == "Player"){
			moveCratesBack = true;
			obj.gameObject.GetComponent<HandController>().freezeTimer = 0f;
		}
		else if (obj.tag == "Player" && !blobsStarted){
			GetComponent<SpriteRenderer>().enabled = false;
			blobsStarted = true;
			foreach (Transform t in transform){
				if (t.GetComponent<Rigidbody2D>()){
					t.gameObject.SetActive(true);
					t.GetComponent<Rigidbody2D>().velocity = new Vector2(26f + Random.Range(.1f, 5f), 0f);
				}
			}
		}
	}

	// Activates blobs which then begin the battle phase
	void AddPreBattleBlobs(){
		if (!addedBlobs){
			cameraFit.SetActive(true);
			bulletBoss.GetComponent<SpriteRenderer>().enabled = true;
			transform.Find("Floor").gameObject.SetActive(false);
			GetComponent<SpriteRenderer>().enabled = false;
			addedBlobs = true;
			GetComponent<Shatterable>().Shatter();
			preBattleBlobsActive = true;
			foreach (blob b in blobs){
				b.obj.SetActive(true);
			}

		}
	}

	// Marks a dialogue as having happened
	void OnTriggerEnter2D(Collider2D col){
		if (!talked && col.tag == "Player"){
			talked = true;
			if (!preBattleSequence){
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetText("prisonerexposition"));
			} else {
				col.gameObject.GetComponent<HandController>().Freeze(1000f);
				/*
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("firstbossexposition0"));
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("firstbossexposition1"));
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("firstbossexposition2"));
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("firstbossexposition3"));
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("firstbossexposition4"));
				*/
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("firstbossexposition"));
			}
		}
	}
}
