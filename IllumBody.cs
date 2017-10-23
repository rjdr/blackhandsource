using UnityEngine;
using System.Collections;

public class IllumBody : MonoBehaviour {
	public GameObject spawn;
	SpriteRenderer sp;
	Vector3 startPos;
	float initialAppearTimer = 0f;
	float maxInitialAppearTimer = .6f;
	float deathTimer = 0f;
	float maxDeathTimer = 5f;
	public float spawnGravity = 0f;
	// Use this for initialization
	void Start () {
		sp = GetComponent<SpriteRenderer>();
		startPos = transform.position;
		spawn = (GameObject)Instantiate(spawn);
		spawn.transform.position = transform.position;
		spawn.SetActive(false);
		spawn.GetComponent<Rigidbody2D>().gravityScale = spawnGravity;
	}
	
	// Update is called once per frame
	void Update () {
		sp.color = Vector4.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, initialAppearTimer / maxInitialAppearTimer);
		transform.position = startPos + new Vector3(0f, -2f*Mathf.Sin(initialAppearTimer/maxInitialAppearTimer * 180f*Mathf.Deg2Rad), 0f);
		if (initialAppearTimer < maxInitialAppearTimer){
			initialAppearTimer += Time.deltaTime;
			if (initialAppearTimer > maxInitialAppearTimer){
				initialAppearTimer = maxInitialAppearTimer;
			}
		}
		// Dies
		deathTimer += Time.deltaTime;
		if (deathTimer > maxDeathTimer){
			GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		}
	}

	// Possesses the spawn object
	/*
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent<HandController>()){
			spawn.GetComponent<PossessableScript>().possessable = true;
			spawn.SetActive(true);
			Camera.main.GetComponent<CameraTarget>().TriggerLightning();
			col.gameObject.GetComponent<HandController>().Possess(spawn);
			Destroy(gameObject, 0f);
		}
	}
	*/

	/// <summary>
	/// When clicked, the player moves to the the beam
	/// 			NOTE:
	/// If there's another collider (including triggers) in front of the beam, the beam will be unclickable
	/// Make sure the grab beam is always closer to the camera than other potential colliders
	/// </summary>
	void OnMouseDown(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		GameObject player = Camera.main.GetComponent<CameraTarget>().GetHandReference();
		if (player.activeSelf){
			// Escape if the player and beam are on different layers
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.layer == bgLayer && (gameObject.layer == fgLayer || gameObject.layer == defaultLayer)){
				return;
			}
			if (player.layer == defaultLayer && gameObject.layer == bgLayer){
				return;
			}
			if (player.GetComponent<HandController>().AttackMenu.activeSelf == false || player.GetComponent<HandController>().grabbedObject == null){
				// Checks if player can make contact with the center or edges of the beam
				if (player.GetComponent<HandController>().ClearPathToTarget(transform.position)){
					// Disables attack menu to pull towards a rod
					if (player.GetComponent<HandController>().AttackMenu.activeSelf){
						player.GetComponent<HandController>().SetAttackMenuStatus(false);
						player.GetComponent<HandController>().NormalTime();
					}

					spawn.GetComponent<PossessableScript>().possessable = true;
					spawn.SetActive(true);
					Camera.main.GetComponent<CameraTarget>().TriggerLightning();
					player.GetComponent<HandController>().Possess(spawn);
					Destroy(gameObject, 0f);

					/*
					player.GetComponent<HandController>().TimedLockControls(maxTimer);
					player.transform.position = transform.position;
					timer = maxTimer;
					startPosition = player.transform.position;
					velocity = Vector3.Distance(player.transform.position, transform.position) / (maxTimer/(Time.fixedDeltaTime));
					*/
				}
			}
		}
	}
}
