using UnityEngine;
using System.Collections;

public class LevelUpEffect : MonoBehaviour {
	ArrayList children = new ArrayList();
	ArrayList positions = new ArrayList();
	ArrayList colors = new ArrayList();

	Vector3 startVelocity = new Vector3(0f, -5f, 0f);
	Vector3 finalVelocity = new Vector3(0f, 8f, 0f);

	float startTimer = 0f;
	float maxTimer = 1.25f;
	// Use this for initialization
	void Start () {
		foreach (Transform child in transform){
			children.Add(child);
			positions.Add(child.position);
			colors.Add(child.GetComponent<SpriteRenderer>().color);
		}
	}

	// Restart the positions of the level up effect
	public void RestartLevelUp(){
		startTimer = 0f;
		for (int i = 0; i < children.Count; i++){
			((Transform)children[i]).position = (Vector3)positions[i];
			((Transform)children[i]).GetComponent<SpriteRenderer>().color = (Color)colors[i];
			((Transform)children[i]).GetComponent<LevelUpParticle>().Reset();
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition += Vector3.Lerp(startVelocity, finalVelocity, startTimer/maxTimer)*Time.deltaTime;
		startTimer += Time.deltaTime;

		// Disable the object once it's run its course
		if (startTimer >= maxTimer){
			RestartLevelUp();
			gameObject.SetActive(false);
		}
	}
}
