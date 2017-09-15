using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetWarning : MonoBehaviour {
	string name = "ARCHDUKE FERDINAND";
	float opacityDirection = -1f;
	int pulseCount = 5;
	Vector3 scale;
	Vector3 startScale;
	float timer = 0f;
	float exitTimer = 0f;
	// Use this for initialization
	void Start () {
		SetName(name);
		scale = transform.localScale;
		startScale = new Vector3(scale.x, 0f, scale.z);
		transform.localScale = startScale;
	}

	public void SetName(string n){
		name = n;
		transform.Find("Text1").GetComponent<Text>().text = name;
		transform.Find("Text2").GetComponent<Text>().text = name;
	}
	
	// Update is called once per frame
	void Update () {
		if (pulseCount > 0){
			timer += Time.deltaTime * 3f;
			transform.localScale = Vector3.Lerp(startScale, scale, timer);
		}

		Color tempColor = transform.Find("Image2").GetComponent<Image>().color;
		tempColor.a += Time.deltaTime * opacityDirection * 1f;
		if (tempColor.a <= 0f && opacityDirection < 0){
			pulseCount -= 1;
			opacityDirection = 1f;
		} else if (tempColor.a >= 1f && opacityDirection > 0) {
			pulseCount -= 1;
			opacityDirection = -1f;
		}
		transform.Find("Image2").GetComponent<Image>().color = tempColor;
		if (pulseCount <= 0){
			exitTimer += Time.deltaTime * 4f;
			transform.localScale = Vector3.Lerp(scale, startScale, exitTimer);
			if (exitTimer >= 1f){
				Destroy(transform.parent.gameObject, 0f);
			}
		}
	}
}
