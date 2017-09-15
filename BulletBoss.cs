using UnityEngine;
using System.Collections;

public class BulletBoss : GenericEnemy {
	// Hands
	BulletBossHandManager frontHand;
	BulletBossHandManager backHand;

	// The hand that guides events
	BulletBossHandManager dominantHand;

	// Grabs tanks in the background to control when they fire
	Tank tank1;
	Tank tank2;

	BulletBossFog bbf;

	ArrayList woodCovers = new ArrayList();
	Transform woodCoverCenter;
	bool woodCoverTouched = false;
	float pushAwayWoodTimer = 1.2f;
	GameObject brokenWoodBehind;

	bool tanksActive = false;

	// Level to scale the Hand to when activating the eaten state
	Vector3 maxPlayerScale = new Vector3(3f, 3f, 3f);
	Vector3 basePlayerScale;
	float scaleUpPlayerTimer = 0f;
	float maxScaleUpPlayerTimer = .5f;
	public bool startDying = false;
	Transform hand;

	// Dialogue with boss announcing how it'll lose
	public bool bossWeaknessDialogue = false;
	bool calledDeath = false;

	Transform eatPoint;
	float eatPointTimer = 0f;
	float maxEatPointTimer = 1f;
	float eatPointSpeed = 1f;
	Vector3 eatPointPos;
	Vector3 eatPointMaxPos;

	float lastLife;

	// Use this for initialization
	void Start () {
		tank1 = transform.Find("Tank1").GetComponent<Tank>();
		tank2 = transform.Find("Tank2").GetComponent<Tank>();

		frontHand = transform.Find("FrontHand").GetComponent<BulletBossHandManager>();
		backHand = transform.Find("BackHand").GetComponent<BulletBossHandManager>();

		if (backHand.inControl){
			dominantHand = backHand;
		} else {
			dominantHand = frontHand;
		}

		GameObject.Find("BossSkybox").GetComponent<MeshRenderer>().material.color = new Color(38f/255f, 5f/255f, 191f/255f, 255f/255f);
		bbf = transform.Find("FogRadius").GetComponent<BulletBossFog>();

		// Gets the covers used to obscure the boss
		foreach (Transform t in transform.Find("WoodCover")){
			if (t.name != "Center"){
				woodCovers.Add(t);
			} else {
				woodCoverCenter = t;
			}
		}

		tank1.gameObject.SetActive(false);
		tank2.gameObject.SetActive(false);

		brokenWoodBehind = GameObject.Find("BrokenWoodBehind");
		hand = GameObject.Find("TheHand").transform;
		basePlayerScale = hand.localScale;

		eatPoint = transform.Find("EatPoint");
		eatPoint.gameObject.SetActive(false);
		Color tempColor = eatPoint.GetComponent<SpriteRenderer>().color;
		tempColor.a = 0f;
		eatPoint.GetComponent<SpriteRenderer>().color = tempColor;
		eatPointPos = eatPoint.position;
		eatPointMaxPos = eatPointPos + new Vector3(0f, -1f, 0f);

		killable = false; 			// Prevent destruction of object

		lastLife = life;
	}

	public override void ChildTriggered(GameObject child, GameObject obj){
		if (obj.gameObject.tag == "Player" && child.name == "WoodCover"){
			frontHand.gameObject.GetComponent<BulletBossHandManager>().enabled = true;
			backHand.gameObject.GetComponent<BulletBossHandManager>().enabled = true;
			woodCoverTouched = true;
		}
		// Activates eat process of the boss
		if (child.name == "EatPoint" && obj.name == "TheHand"){
			if (!startDying){
				eatPoint.gameObject.SetActive(false);
				Time.timeScale = .8f;		// Slows down time
				startDying = true;
				hand.GetComponent<HandController>().RestoreLife();
				hand.GetComponent<HandController>().EatObject(transform, true);
				hand.GetComponent<HandController>().Freeze(10f);
			}
		}
	}

	// Pushes the wood covers away
	void PushAwayCovers(){
		pushAwayWoodTimer -= Time.deltaTime;
		if (pushAwayWoodTimer <= 0f){
			transform.Find("WoodCover").gameObject.SetActive(false);
			return;
		}
		foreach (Transform t in woodCovers){
			float speed = 12f;
			Vector3 velocity = (t.position - woodCoverCenter.position).normalized;
			t.position += velocity * Time.deltaTime * speed;
			t.eulerAngles += new Vector3(0f, 0f, 90f*Time.deltaTime);
		}
	}

	// Activates the tanks
	public void ActivateTanks(){
		if (!tanksActive){
			tanksActive = true;

			tank1.gameObject.SetActive(true);
			tank2.gameObject.SetActive(true);

			tank1.ActivateAim();
			tank2.ActivateAim();

			// Shatter backgrounds and displays tanks
			foreach (Transform t in brokenWoodBehind.transform){
				t.gameObject.GetComponent<Shatterable>().Shatter();
				t.gameObject.SetActive(false);
			}
		}
	}

	// Activate the bullet attack of the hands
	public void ActivateHandBullets(){
		dominantHand.StartRotating();
	}
	
	// Update is called once per frame
	void Update () {
		// Handles pain animations
		if (GetComponent<Animator>().GetBool("Pain")){
			GetComponent<Animator>().SetBool("Pain", false);
		}
		if (life < lastLife){
			lastLife = life;
			GetComponent<Animator>().SetBool("Pain", true);
			Camera.main.GetComponent<CameraTarget>().AddShake(.15f);
		}

		if (woodCoverTouched){
			PushAwayCovers();
		}
		// Scale up the hand so that it eats the boss
		if (startDying){
			GetComponent<Animator>().SetBool("Death", true);
			eatPoint.gameObject.SetActive(false);
			Time.timeScale = .8f;		// Slows down time
			scaleUpPlayerTimer += Time.deltaTime;
			hand.localScale = Vector3.Lerp(basePlayerScale, maxPlayerScale, scaleUpPlayerTimer / maxScaleUpPlayerTimer);
		} else {
			if (life <= 0f){
				Death();
			}
		}
		if (eatPoint.gameObject.activeSelf){
			Color tempColor = eatPoint.GetComponent<SpriteRenderer>().color;
			Color destColor = eatPoint.GetComponent<SpriteRenderer>().color;
			destColor.a = 1f;
			eatPoint.GetComponent<SpriteRenderer>().color = Vector4.MoveTowards(tempColor, destColor, Time.deltaTime);

			eatPoint.position = Vector3.Lerp(eatPointPos, eatPointMaxPos, eatPointTimer/maxEatPointTimer);
			eatPointTimer += Time.deltaTime*eatPointSpeed;
			if (eatPointTimer >= maxEatPointTimer){
				eatPointSpeed = -Mathf.Abs(eatPointSpeed);
			} else if (eatPointTimer <= 0f){
				eatPointSpeed = Mathf.Abs(eatPointSpeed);	
			}
		}
	}

	public void ActivateEatPoint(){
		eatPoint.gameObject.SetActive(true);
	}

	// Activates the monster being eaten
	public override void Death(){
		if (!calledDeath){
			calledDeath = true;
			Invoke("ActivateEatPoint", 2f);
			Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("bulletbosseat"));
		}
		/*
		Time.timeScale = .8f;		// Slows down time
		startDying = true;
		hand.GetComponent<HandController>().RestoreLife();
		hand.GetComponent<HandController>().EatObject(transform);
		*/
	}

	// Called after devouring process is completed
	public override void FinishBeingDevoured(GameObject eater){
		Time.timeScale = 1f;
		Camera.main.GetComponent<CameraTarget>().TriggerLightning();
		foreach (Transform t in frontHand.GetComponent<BulletBossHandManager>().flames){
			t.gameObject.SetActive(false);
		}
		Camera.main.GetComponent<BulletBossLevel>().ActivateLogo();
	}

}
