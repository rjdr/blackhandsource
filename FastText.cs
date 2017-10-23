using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FastText : MonoBehaviour {
	float time = 0f;
	public float startMaxTime = .5f;
	public float lowMaxTime = .3f;
	public float maxTime = .3f;		// Time between the text transitions
	public float finalMaxTime = 1f; 	// If it's the last text, do a fade before deleting it
	ArrayList texts = new ArrayList();
	Text text;
	public bool cascading = false;
	public bool increaseSize = false;
	public bool fastText = true;
	// Use this for initialization
	void Start () {
		text = transform.Find("Text").GetComponent<Text>();
		maxTime = startMaxTime;
	}

	// Takes a string as input and auto splits by spaces
	public void SetText(string s){
		// Split up fast text word-by-word
		if (fastText){
			text = transform.Find("Text").GetComponent<Text>();
			string[] subs = s.Split(' ');
			for (int i = 0; i < subs.Length; i++){
				texts.Add(subs[i]);
			}
			text.text = (string)texts[0];
			texts.RemoveAt(0);
			if (texts.Count < 1){
				maxTime *= 3f;
				finalMaxTime += maxTime;
			}
		}
		// Put on the slow text as-is
		else {
			text = transform.Find("Text").GetComponent<Text>();
			text.text = s;
		}
	}
	/// <summary>
	/// Position ranges from X(-640, 640) to Y(-400, 400) 
	/// </summary>
	public void SetPosition(float x, float y){
		text = transform.Find("Text").GetComponent<Text>();
		float scale = transform.GetComponent<RectTransform>().localScale.x;
		text.rectTransform.position = new Vector3(x * scale + 640f * scale, y * scale + 400f * scale, 0f);
	}

	// Sets the font size
	public void SetFontSize(int f){
		text.fontSize = f;
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= maxTime){
			// Deactivates if there is no more text
			if (texts.Count <= 0){
				// Destroy it once we become transparent
				if (time >= finalMaxTime){
					Destroy(gameObject, 0f);
				// Do a fade out of the text
				} else {
					Color c = text.color;
					float f = (finalMaxTime - maxTime);
					c.a = (f -(time - maxTime)) / f;
					text.color = c;
				}
			// Go to the next text if there are more
			} else {
				maxTime = lowMaxTime;
				time = 0f;
				text.text = (string)texts[0];
				texts.RemoveAt(0);
				if (cascading){
					float scale = transform.GetComponent<RectTransform>().localScale.x;
					text.rectTransform.position += new Vector3(0f, -70f * scale, 0f);
				}
				if (increaseSize){
					text.fontSize += 50;
				}
			}
		}
	}
}
