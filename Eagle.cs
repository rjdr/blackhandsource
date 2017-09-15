using UnityEngine;
using System.Collections;

public class Eagle : MonoBehaviour {
	Vector3 startPos = new Vector3(7.5f*.8f, 3f*.8f, 3f);
	Vector3 endPos = new Vector3(-5.8f, -3f, 2f);
	Vector3 startSpeed = new Vector3(-12.5f*1.5f, -6f*1.5f, 0f);
	Vector3 endSpeed = new Vector3(6.25f, 3f, 0f);
	float startMoveTimer = .5f;
	float endMoveTimer = 2f;
	// Use this for initialization
	void Start () {
		transform.localPosition = startPos;
		transform.Find("AmericanAppearanceFlag").gameObject.SetActive(false);
		// Flips to face left
		Vector3 scale = transform.localScale;
		scale.x *= -1f;
		transform.localScale = scale;
		GetComponent<SpriteRenderer>().color = new Color(.1f, .1f, .1f, 1f);
		transform.Find("AmericanAppearanceFlag").GetComponent<SpriteRenderer>().material.SetVector("_Color", new Vector4(1f, 1f, 1f, 1f));
		transform.Find("AmericanAppearanceFlag").GetComponent<SpriteRenderer>().material.SetFloat("_BumpAmt", 4.1f);
	}
	
	// Update is called once per frame
	void Update () {
		// Flies downward, no flag
		if (startMoveTimer > 0f){
			transform.position += startSpeed * Time.deltaTime;
			startMoveTimer -= Time.deltaTime;
			if (startMoveTimer <= 0f){
				transform.localPosition = endPos;
				transform.Find("AmericanAppearanceFlag").gameObject.SetActive(true);
				// Flips once more to face right
				Vector3 scale = transform.localScale;
				scale.x *= -1f;
				transform.localScale = scale;
				GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			}
		// Flies up with the flag trailing behind it
		} else {
			transform.position += endSpeed*Time.deltaTime;
			endMoveTimer -= Time.deltaTime;
			if (endMoveTimer <= 0f){
				Destroy(gameObject, 0f);
			}
		}
	}
}
