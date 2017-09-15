using UnityEngine;
using System.Collections;

public class WatchingSpirit : MonoBehaviour {
	Transform eye;
	GameObject player;
	float maxUpdatePlayerTimer = 1f;
	float updatePlayerTimer = 0f;
	bool addedDialogue = false;
	bool active = false;
	bool addedSecondDialogue;
	Transform swirl;
	GameObject YBorder;
	Transform flowerOfLife;
	Transform flowerOfLifeColor;
	Transform reach;
	Transform head;
	Transform playerPoint;
	Transform wildMoon;
	Transform otherMoon;
	float f = 0;

	bool smashed = false;
	float postSmashWait = 2f;

	bool activatedWildMoon = false;
	// Use this for initialization
	void Start() {
		eye = transform.Find("watchingspirit_eye");
		swirl = transform.Find("Swirl");
		swirl.GetComponent<SwirlScript>().SetActive(false);
		swirl.gameObject.SetActive(false);
		YBorder = GameObject.Find("YBorder");
		flowerOfLife = transform.Find("FlowerOfLifeFilter");
		flowerOfLifeColor = flowerOfLife.transform.Find("FlowerOfLifeColor");
		flowerOfLife.gameObject.SetActive(false);

		head = transform.Find("WatchingSpiritHead");
		reach = transform.Find("WatchingSpiritReach");
		head.gameObject.SetActive(false);
		reach.gameObject.SetActive(false);
		playerPoint = transform.Find("PlayerPoint");
		playerPoint.gameObject.SetActive(false);
		wildMoon = transform.Find("WildMoon");
		wildMoon.SetParent(null);
		wildMoon.gameObject.SetActive(false);
		otherMoon = transform.Find("OtherMoon");
		otherMoon.SetParent(null);
		otherMoon.gameObject.SetActive(false);
	}

	void UpdateFlower(){
		f += Time.deltaTime;
		flowerOfLife.GetComponent<MeshRenderer>().material.SetFloat("_BumpAmt", Mathf.Clamp(.6f*f, 0f, 60f));
		Color c = flowerOfLifeColor.GetComponent<MeshRenderer>().material.GetColor("_TintColor");
		c.a = Mathf.Clamp(.05f*f, 0f, 1f);
		flowerOfLifeColor.GetComponent<MeshRenderer>().material.SetColor("_TintColor", c);
	}
	
	// Update is called once per frame
	void Update() {
		updatePlayerTimer -= Time.deltaTime;
		if (updatePlayerTimer <= 0f){
			updatePlayerTimer = maxUpdatePlayerTimer;
			player = GameObject.FindGameObjectWithTag("Player");
		}
		// Checks if the player is right of the Spirit and activates the moon if so
		if (!activatedWildMoon && player.transform.position.x > transform.position.x){
			activatedWildMoon = true;
			wildMoon.gameObject.SetActive(true);
			otherMoon.gameObject.SetActive(true);
		}
		eye.transform.up = player.transform.position - transform.position;
		if (active){
			// Keep player locked in place 
			player.transform.position = playerPoint.transform.position;
			UpdateFlower();
			if (!addedDialogue){
				addedDialogue = true;
				Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("firstwatchingspirit"));
			} else {
				// Activate the player crush phase
				if (!Camera.main.GetComponent<CameraTarget>().ActiveDialogue()){
					smashed = true;
					GetComponent<SpriteRenderer>().enabled = false;
					head.gameObject.SetActive(true);
					reach.gameObject.SetActive(true);

					// Activates the object with the smoosh/smash animation
					playerPoint.gameObject.SetActive(true);
					// disables player rendering
					player.GetComponent<SpriteRenderer>().enabled = false;
					player.transform.Find("YellowLayer").GetComponent<SpriteRenderer>().enabled = false;

					player.GetComponent<HandController>().Freeze(1f);
				}
			}
			// Countdown until the Spirit disappears
			// Activates a lightning flash, shows the new moon, then goes away
			if (smashed){
				postSmashWait -= Time.deltaTime;
				if (postSmashWait <= 0f){
					if (!addedSecondDialogue){
						addedSecondDialogue = true;
						Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("firstwatchingspirit2"));
					} else if (!Camera.main.GetComponent<CameraTarget>().ActiveDialogue()) {
						Camera.main.GetComponent<CameraTarget>().TriggerLightning();
						wildMoon.gameObject.SetActive(true);
						otherMoon.gameObject.SetActive(true);

						player.GetComponent<SpriteRenderer>().enabled = true;
						player.transform.Find("YellowLayer").GetComponent<SpriteRenderer>().enabled = true;
						gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Activates the scene once the player is near
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.tag == "Player" && !active){
			if (col.gameObject.transform.position.x > playerPoint.position.x){
				player = col.gameObject;
				//GetComponent<SpriteRenderer>().enabled = false;
				//head.gameObject.SetActive(true);
				//reach.gameObject.SetActive(true);
				flowerOfLife.gameObject.SetActive(true);
				swirl.gameObject.SetActive(true);
				swirl.GetComponent<SwirlScript>().SetActive(true);
				active = true;
				//YBorder.transform.position = transform.position + new Vector3(0f, 7f, 0f);
			}
		}
	}
}
