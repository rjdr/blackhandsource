using UnityEngine;
using System.Collections;

public class LevelEndTrigger : GenericEnemy {
	bool touched = false;
	bool endedScene = false;
	int currLevel = 1;
	GameObject FillColor;
	GameObject AutofitCameraObject;
	Vector4 startColor = new Vector4(0f, 0f, 0f, 0f);
	Vector4 endColor = new Vector4(0f, 0f, 0f, 1f);
	float timer = 0f;

	const float startFade = 8f;
	const float endFade = 13f;

	const float textInterval = 5f;
	const float startText = 4f;
	bool started1 = false;
	const float text2 = startText + textInterval;
	bool started2 = false;
	const float text3 = text2 + textInterval;
	bool started3 = false;
	const float endScene = text3 + textInterval * 1.5f;
	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().enabled = false;
		FillColor = transform.Find("FillColor").gameObject;
		FillColor.transform.parent = Camera.main.transform;
		FillColor.transform.localPosition = new Vector3(0f, 0f, 1f);
		FillColor.SetActive(false);

		// For dramatic camera panning after finishing a level
		AutofitCameraObject = transform.Find("AutofitCameraObject").gameObject;
		AutofitCameraObject.SetActive(false);

		// Enables a block that prevents the player from moving back to where they came from
		transform.Find("NoReturn").gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (touched){
			if (currLevel == 1){
				OfficeCityEnd();
			}
		}
	}
	// Ends the office zone. Ends a SlowText object
	void OfficeCityEnd(){
		FillColor.GetComponent<SpriteRenderer>().color = Vector4.Lerp(startColor, endColor, (timer - startFade) / (endFade - startFade));
		if (!started1 && timer >= startText){
			started1 = true;
			AddSlowText("The future will wonder whether this destruction was too much...", 0f, 0f);
		}
		if (!started2 && timer >= text2){
			started2 = true;
			AddSlowText("And I will say...", 0f, 0f);
		}
		if (!started3 && timer >= text3){
			started3 = true;
			AddSlowText("It was not enough", 0f, 0f);
		}
		// Ends the level
		if (timer >= endScene && !endedScene){
			endedScene = true;
			Application.LoadLevel("MapScene");
		}
		AutofitCameraObject.transform.position += new Vector3(-5f * Time.deltaTime, 0f, 0f);

		timer += Time.unscaledDeltaTime;
	}
	// Ends the level once touched
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player"){
			touched = true;
			FillColor.SetActive(true);
			AutofitCameraObject.SetActive(true);
			if (col.GetComponent<GenericEnemy>()){
				col.GetComponent<GenericEnemy>().enabled = false;
			}

			// Enables a block that prevents the player from moving back to where they came from
			transform.Find("NoReturn").gameObject.SetActive(true);
		}
	}
}
