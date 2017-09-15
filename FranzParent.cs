using UnityEngine;
using System.Collections;

public class FranzParent : MonoBehaviour {
	Transform child;
	GameObject light;
	bool addedDialogue = false;
	Color startColor;
	Color destColor = new Color(.1f, .05f, .25f, 1f);
	Color startLightColor;
	Color destLightColor = new Color(.12f, .05f, .33f, 1f);
	Color startFogColor;
	Color destFogColor = new Color(.1f, .2f, .45f, 1f);
	float startLightIntensity;
	float time = 0f;
	float maxTimer = 3f;

	public GameObject blob;
	ArrayList blobs = new ArrayList();
	Vector3 blobVelocity = new Vector3(0f, -17f, 0f);

	float camMoveTimer = 0f;
	float exitLevelTime = .5f;
	bool faded = false;
	float exitDelayTimer = 0f;

	GameObject basic1;
	GameObject basic2;

	// Use this for initialization
	void Start () {
		light = GameObject.Find("Directional light");
		startLightColor = light.GetComponent<Light>().color;
		startColor = RenderSettings.ambientLight;
		startLightIntensity = light.GetComponent<Light>().intensity;
		startFogColor = RenderSettings.fogColor;
		child = transform.Find("Franz");
		basic1 = transform.Find("Basic1").gameObject;
		basic2 = transform.Find("Basic2").gameObject;

		for (int i = 0; i < 88; i++){
			GameObject b = (GameObject)Instantiate(blob);
			b.transform.parent = transform;
			b.transform.localPosition = new Vector3(Random.Range(-12f, 20f), Random.Range(14f, 32f), Random.Range(-8f, 12f));
			float f = Random.Range(.2f, 1f);
			b.transform.localScale = new Vector3(f, f, f);
			b.SetActive(false);
			blobs.Add(b);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (child == null && addedDialogue == false){
			addedDialogue = true;
			Camera.main.GetComponent<CameraTarget>().AddDialogue(DialogueTable.GetChatChain("franzdead"));
			foreach (GameObject b in blobs){
				b.SetActive(true);
			}
		} else if (addedDialogue == true){
			time += Time.deltaTime;
			RenderSettings.ambientLight = Vector4.Lerp(startColor, destColor, time / maxTimer);
			RenderSettings.fogColor = Vector4.Lerp(startFogColor, destFogColor, time / maxTimer);
			light.GetComponent<Light>().color = Vector4.Lerp(startLightColor, destLightColor, time / maxTimer);
			light.GetComponent<Light>().intensity = Mathf.Lerp(startLightIntensity, 3f, time/maxTimer);
			basic1.GetComponent<RippleController>().SetRippleVisibility(time/maxTimer);
			basic2.GetComponent<RippleController>().SetRippleVisibility(time/maxTimer);
			Vector3 vel = blobVelocity * Time.deltaTime;
			foreach (GameObject b in blobs){
				b.transform.localPosition += vel;
				if (b.transform.localPosition.y <= -4f){
					Vector3 temp = b.transform.localPosition;
					temp.y = 14f;
					b.transform.localPosition = temp;
				}
			}
			if (Camera.main.GetComponent<CameraTarget>().ActiveDialogue() == false){
				// Fades to black to exit the level
				camMoveTimer += Time.deltaTime;
				if (camMoveTimer > exitLevelTime){
					if (!faded){
						Camera.main.GetComponent<FadeToBlack>().FadeOut();
						faded = true;
						// Loads next level once fade is complete
					} else {
						if (Camera.main.GetComponent<FadeToBlack>().GetAlpha() >= .99f){
							exitDelayTimer += Time.fixedDeltaTime;
							if (exitDelayTimer >= .5f){
								Application.LoadLevel("MapScene");
							}
						}
					}
				}
			}
		}
	}
}
