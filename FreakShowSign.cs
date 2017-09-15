using UnityEngine;
using System.Collections;

public class FreakShowSign : GenericEnemy {
	bool addText = false;
	float timer = 0f;
	const float showTextTimer = 4f;
	const float startFadeTimer = showTextTimer + 1.5f;
	const float endFadeTimer = startFadeTimer + .5f;
	Color startColor;
	Color noColor = new Color(0f, 0f, 0f, 0f);
	Transform cover;
	// Use this for initialization
	void Start () {
		cover = transform.Find("Cover");
		startColor = cover.GetComponent<SpriteRenderer>().color;
		GameObject g = GameObject.Find("TheHand");
		// If we start to the left of the hand, that means he's spawning from a checkpoint and we're not needed
		if (g.transform.position.x > transform.position.x){
			gameObject.SetActive(false);
		// Disable the audio source so music doesn't play at the start
		} else {
			Camera.main.GetComponent<AudioSource>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		// Shows wake up text
		if (!addText && timer > showTextTimer){
			addText = true;
			AddFastText(DialogueTable.GetChat("wakeup").text, 200, 100, true, true);
			// Enable music
			Camera.main.GetComponent<AudioSource>().enabled = true;
		}
		// Fades out the screen cover
		if (timer > startFadeTimer){
			cover.GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, noColor, (timer - startFadeTimer) / (endFadeTimer - startFadeTimer));
			if (timer > endFadeTimer){
				this.enabled = false;
			}
		}
	}
}
