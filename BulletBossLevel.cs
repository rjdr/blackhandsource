using UnityEngine;
using System.Collections;

public class BulletBossLevel : MonoBehaviour {
	GameObject logo;
	bool faded = false;
	bool activated = false;

	Vector3 logoScale;
	Vector3 minScale = new Vector3(0f, 0f, 0f);
	float scaleTimer = -.5f;
	float maxScaleTimer = .25f;

	float exitDelayTimer = 0f;
	// Use this for initialization
	void Start () {
		logo = GameObject.Find("BlackHandLogo");
		logoScale = logo.transform.localScale;
		logo.SetActive(false);
	}

	// Enables logo and turns screen white
	public void ActivateLogo(){
		logo.SetActive(true);
		logo.GetComponent<BlackHandLogo>().enabled = false;			// Disables the spinning of the logo
		Camera.main.GetComponent<LightningScript>().EnableFlash();
		Camera.main.GetComponent<LightningScript>().enabled = false;
		logo.transform.localScale = minScale;
		activated = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (activated){
			// Enable text spinning
			if (scaleTimer >= 0f){
				logo.GetComponent<BlackHandLogo>().enabled = true;
			}
			logo.transform.localScale = Vector3.Lerp(minScale, logoScale, scaleTimer/maxScaleTimer);
			if (!faded && logo.GetComponent<BlackHandLogo>().TextStopped()){
				Invoke("Fade", 3f);
				faded = true;
				// Loads next level once fade is complete
			} else if (faded) {
				if (Camera.main.GetComponent<FadeToBlack>().GetAlpha() >= .99f){
					exitDelayTimer += Time.fixedDeltaTime;
					if (exitDelayTimer >= 2f){
						Application.LoadLevel("MapScene");
					}
				}
			}
			scaleTimer += Time.deltaTime;
		}
	}

	void Fade(){
		Camera.main.GetComponent<FadeToBlack>().FadeOut();
	}

}
