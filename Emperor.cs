using UnityEngine;
using System.Collections;

public class Emperor : GenericEnemy {
	Rigidbody2D rb;
	Animator anim;
	float speed = 2.2f;
	float deathWalkSpeed = 1.5f;
	GameObject theHand;
	bool bleeding = false;
	bool beganEnd = false;

	GameObject WhiteCover;
	GameObject DirectionalLight;
	GameObject RenderSprite;
	GameObject RenderCamera;
	GameObject BlurCamera;
	GameObject Swirl;
	GameObject SittingStatue;
	GameObject SittingStatueAlt;
	GameObject RainFront;
	Transform Foot;
	Transform LeftBody;
	Transform RightBody;
	Transform Eye;
	GameObject BloodTrail;
	GameObject SelfSwirl1;
	GameObject SelfSwirl2;
	GameObject IntroWhite;
	SpriteRenderer SittingStatueSprite;
	RippleController SittingStatueRipple;

	float renderCameraActiveTimer = .1f;
	Vector3 swirlStartPos;
	Vector3 swirlDestPos;
	float beginEndCountdown = 2.25f;	// How long after reaching the end until we fall to our knees
	float swirlTimer = 0f;
	float maxSwirlTimer = 5f;
	float waitAfterSwirlTimer = 9f;	// How long after the swirl until we start the blur effect

	float startRippleTimer = 0f;
	float maxStartRippleTimer = 3.5f;
	float startSpriteTimer = 0f;
	float maxStartSpriteTimer = 3.5f;

	Color skyColor;
	Color equatorColor;
	Color groundColor;
	float fadeColorTimer = 0f;
	float maxFadeColorTimer = 3f;
	float bleedTimer = 0f;
	float bleedCount = 1.5f;
	float splitBodySpeed = 5f;
	float maxSplitBodySpeed = 5f;
	float splitBodyTimer = 0f;
	float maxSplitBodyTimer = 1f;
	bool splittingBody = false;
	float swirlFadeInTimer = 0f;
	float maxSwirlFadeInTimer = 3.5f;

	// The white at the beginning
	float introWhiteTimer = 0f;
	const float startIntroWhite = .75f;
	const float endIntroWhite = startIntroWhite + .5f;

	// After we split apart, turn the screen white, then go black, then go to the Dark World
	float startWhiteTimer = 0f;
	const float maxStartWhiteTimer = 8f;
	float startFadeToBlackTimer = 1f;
	float maxStartFadeToBlackTimer = 2f;
	const float startBlackFade2 = maxStartWhiteTimer + 1f;
	const float maxBlackFade2 = startBlackFade2 + 1f;
	const float maxStartShowRevengeText = maxBlackFade2 + 3f;
	const float maxShowChanceTimer = maxStartShowRevengeText + 3f;
	const float startNextLevelTimer = maxShowChanceTimer + 3f;
	bool showedRevengeText = false;
	bool showedOneChanceText = false;
	bool startNextLevel = false;

	bool shownAre = false;
	bool shownYou = false;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		theHand = GameObject.Find("TheHand");

		GameObject.Find("CameraCircle").SetActive(false);
		GameObject.Find("Crosshair").SetActive(false);

		DirectionalLight = GameObject.Find("DirectionalLight");
		RainFront = GameObject.Find("RainFront");
		RainFront.SetActive(false);
		LeftBody = transform.Find("Left");
		RightBody = transform.Find("Right");
		Eye = transform.Find("Eye");

		// Disable these at the beginning, then reenable later for special effects when dying
		RenderSprite = GameObject.Find("RenderSprite");
		RenderCamera = GameObject.Find("RenderCamera");
		BlurCamera = GameObject.Find("BlurCamera");
		RenderSprite.SetActive(false);
		RenderCamera.SetActive(false);
		BlurCamera.SetActive(false);
		Swirl = GameObject.Find("Swirl");
		Swirl.gameObject.SetActive(false);
		Foot = transform.Find("Foot");
		BloodTrail = (GameObject)Resources.Load("BloodTrail");
		swirlStartPos = Swirl.transform.position;
		swirlDestPos = swirlStartPos + new Vector3(0f, 240f, 0f);

		SelfSwirl1 = transform.Find("Swirl1").gameObject;
		SelfSwirl2 = transform.Find("Swirl2").gameObject;
		SelfSwirl1.SetActive(false);
		SelfSwirl2.SetActive(false);
		IntroWhite = transform.Find("IntroWhite").gameObject;

		SittingStatue = GameObject.Find("SittingStatue");
		SittingStatueAlt = GameObject.Find("SittingStatueAlt");
		SittingStatueSprite = SittingStatueAlt.transform.Find("SittingGod").GetComponent<SpriteRenderer>();
		SittingStatueSprite.color = new Color(1f, 1f, 1f, 0f);
		SittingStatueRipple = SittingStatueAlt.transform.Find("SittingGod").GetComponent<RippleController>();
		//SittingStatueRipple.SetRippleOnly(0f);
		SittingStatueAlt.SetActive(false);

		skyColor = RenderSettings.ambientSkyColor;
		equatorColor = RenderSettings.ambientEquatorColor;
		groundColor = RenderSettings.ambientGroundColor;
		WhiteCover = GameObject.Find("WhiteCover");
	}

	void FixedUpdate(){
		SplitBody();
	}

	// Update is called once per frame
	void Update () {
		theHand.SetActive(false);
		introWhiteTimer += Time.deltaTime;
		if (introWhiteTimer > startIntroWhite){
			if (introWhiteTimer < endIntroWhite){
				Color destColor = new Color(1f, 1f, 1f, 0f);
				IntroWhite.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, destColor, (introWhiteTimer - startIntroWhite) / (endIntroWhite - startIntroWhite));
			}
			else if (introWhiteTimer >= endIntroWhite && IntroWhite.activeSelf){
				IntroWhite.SetActive(false);
				FastText fa = AddFastText(DialogueTable.GetChat("thisempire").text, 200, 200, true);
			}
		}
		//if (Input.GetMouseButtonDown(0)){
		//	AddFastText("THIS IS SOME TEXT", -200f, 100f, true);
		//}
		if (!bleeding){
			float xMov = Input.GetAxisRaw("Horizontal");
			Vector2 vel = rb.velocity;
			if (xMov > .1f){
				vel.x = speed;
				anim.SetBool("Walk", true);
			} else if (xMov < -.1f){
				//vel.x = -speed;
				//anim.SetBool("Walk", true);
			} else {
				vel.x = 0f;
				anim.SetBool("Walk", false);
			}
			rb.velocity = vel;
		// Starts the end sequence
		} else {
			if (bleedTimer >= bleedCount && !beganEnd){
				GameObject b = (GameObject)Instantiate(BloodTrail);
				b.transform.position = Foot.transform.position;
				b.transform.eulerAngles = new Vector3(-90f, Random.Range(0f, 360f), 0f);
				b.transform.position += new Vector3(Random.Range(-.05f, .05f), 0f, Random.Range(-.1f, .1f));
				bleedCount += .5f;
			}
			bleedTimer += Time.deltaTime;
			SittingStatueSprite.color = new Color(1f, 1f, 1f, 0f);
			if (beganEnd){
				DirectionalLight.GetComponent<Light>().intensity -= Time.deltaTime;
				RenderSettings.ambientSkyColor = Vector4.Lerp(skyColor, new Vector4(.2f, .0f, .2f, 1f), fadeColorTimer / maxFadeColorTimer);

				fadeColorTimer += Time.deltaTime;
				if (beginEndCountdown >= 0f){
					anim.SetBool("BleedStand", true);
					beginEndCountdown -= Time.deltaTime;
					if (beginEndCountdown <= 0f){
						gameObject.layer = LayerMask.NameToLayer("Shifter");
						anim.SetBool("Death", true);
						BlurCamera.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>().enabled = true;
						Camera.main.GetComponent<CameraTarget>().TriggerSilentLightning(.25f);
						SittingStatue.SetActive(false);
						SittingStatueAlt.SetActive(true);
						Swirl.SetActive(true);
						Swirl.transform.position = swirlDestPos;
					}
				}
				else {
					if (swirlTimer > 1f && !shownYou){
						shownYou = true;
						FastText fa = AddFastText(DialogueTable.GetChat("YOU").text, -300, 100);
						fa.SetFontSize(400);
					}
					Swirl.transform.position = Vector3.Lerp(swirlDestPos, swirlStartPos, swirlTimer / maxSwirlTimer);
					swirlTimer += Time.deltaTime;
					// Handles appearance of the sprite
					if (swirlTimer >= maxSwirlTimer){
						// Ripple ramps up
						startRippleTimer += Time.deltaTime;
						SittingStatueRipple.SetRippleOnly(startRippleTimer / maxStartRippleTimer);
						// Ripples goes away as sprite fades in
						if (startRippleTimer >= maxStartRippleTimer){
							if (!shownAre){
								shownAre = true;
								//Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("ARE"));
								FastText fa = AddFastText(DialogueTable.GetChat("ARE").text, -200, 100);
								fa.SetFontSize(400);
							}
							startSpriteTimer += Time.deltaTime;
							float f = maxStartSpriteTimer - startSpriteTimer;
							SittingStatueSprite.color = new Color(1f, 1f, 1f, startSpriteTimer / maxStartSpriteTimer);
							SittingStatueRipple.SetRippleOnly(f / maxStartRippleTimer);
						}
					}
					// Starts the blur
					if (swirlTimer >= maxSwirlTimer + waitAfterSwirlTimer){
						RainFront.layer = LayerMask.NameToLayer("Default");
						BlurCamera.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>().blurSize += Time.deltaTime * 10f;
						if (renderCameraActiveTimer > 0f){
							RenderCamera.SetActive(true);
							RenderSprite.SetActive(true);
							renderCameraActiveTimer -= Time.deltaTime;
							if (renderCameraActiveTimer <= 0f){
								if (LeftBody.gameObject.activeSelf == false){
									//Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("DEAD"));
									FastText fa = AddFastText(DialogueTable.GetChat("DEAD").text, -200, 100);
									fa.SetFontSize(500);
								}
								// Start the body split process
								LeftBody.gameObject.SetActive(true);
								RightBody.gameObject.SetActive(true);
								Eye.gameObject.SetActive(true);
								splittingBody = true;
								GetComponent<SpriteRenderer>().enabled = false;

								RainFront.SetActive(false);
								RenderCamera.SetActive(false);
								BlurCamera.SetActive(true);
							}
						}
					}
				}
			} else {
				Bleeding();
			}
		}
	}
	// Splits the body apart
	void SplitBody(){
		if (!splittingBody) return;
		if (splitBodySpeed > 3f) splitBodySpeed = 4f;
		LeftBody.transform.position -= new Vector3(splitBodySpeed * Time.fixedDeltaTime, 0f, 0f);
		RightBody.transform.position += new Vector3(splitBodySpeed * Time.fixedDeltaTime, 0f, 0f);
		splitBodyTimer += Time.fixedDeltaTime;
		splitBodySpeed = Mathf.Lerp(maxSplitBodySpeed, 0f, splitBodyTimer / maxSplitBodyTimer);

		swirlFadeInTimer += Time.fixedDeltaTime;
		SelfSwirl1.SetActive(true);
		SelfSwirl2.SetActive(true);
		Color col = new Color(1f, 1f, 1f, Mathf.Lerp(-.75f, 1f, swirlFadeInTimer / maxSwirlFadeInTimer));
		if (col.a < 0f){
			col.a = 0f;
		}
		SelfSwirl1.GetComponent<SpriteRenderer>().material.SetColor("_Color", col);
		SelfSwirl2.GetComponent<SpriteRenderer>().material.SetColor("_Color", col);

		// Transitions to white
		startWhiteTimer += Time.fixedDeltaTime;
		if (startWhiteTimer >= maxStartWhiteTimer){
			WhiteCover.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			// Transitions to black
			if (startWhiteTimer >= (startBlackFade2)){
				float t1 = startWhiteTimer - startBlackFade2;
				float t2 = maxBlackFade2 - startBlackFade2;
				float f = t1/t2;
				f = 1 - f;
				if (f < 0){
					f = 0;
				}
				// Wrap up; shows end text and loads scene
				if (startWhiteTimer >= (maxStartShowRevengeText)){
					if (!showedRevengeText){
						showedRevengeText = true;
						AddFastText(DialogueTable.GetChat("doyouwantrevenge").text, -200, 200, true, true);
					}
					if (startWhiteTimer > maxShowChanceTimer && !showedOneChanceText){
						showedOneChanceText = true;
						AddFastText(DialogueTable.GetChat("onemorechance").text, 200, 200, true, true);
					}
					if (startWhiteTimer > startNextLevelTimer && !startNextLevel){
						UnityEngine.SceneManagement.SceneManager.LoadScene("TrainScene");
					}
				}
				WhiteCover.GetComponent<SpriteRenderer>().color = new Color(f, f, f, 1f);
			}
		}
	}

	void Bleeding(){
		Vector2 vel = rb.velocity;
		vel.x = deathWalkSpeed;
		rb.velocity = vel;
	}
	void BeginEnd(){
		if (!beganEnd){
			//Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChat("YOU").text);
			//AddFastText(DialogueTable.GetChat("YOU").text, -300, 100);
			beganEnd = true;
		}
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.name == "TriggerPillar"){
			anim.SetBool("Bleed", true);
			bleeding = true;
		} if (col.name == "TriggerPillar2"){
			anim.SetBool("Bleed2", true);
			bleeding = true;
		} if (col.name == "TriggerPillar3"){
			anim.SetBool("Bleed3", true);
			RainFront.SetActive(true);
			bleeding = true;
		} if (col.name == "FinalPillar"){
			BeginEnd();
		}
	}
}
