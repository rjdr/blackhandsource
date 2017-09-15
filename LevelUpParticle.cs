using UnityEngine;
using System.Collections;

public class LevelUpParticle : MonoBehaviour {
	float angle = 0f;
	float distance = 0f;
	public float angleSpeed = 180f;
	float opacity;
	float opacityReduction = .9f;
	SpriteRenderer sr;
	// Use this for initialization
	void Start () {
		distance = Mathf.Sqrt(Mathf.Pow(transform.localPosition.x, 2f) + Mathf.Pow(transform.localPosition.z, 2));
		float angle = Mathf.Atan2(transform.localPosition.z, transform.localPosition.x);

		sr = GetComponent<SpriteRenderer>();
		opacity = sr.color.a;
	}

	public void Reset(){
		angle = 0f;
		Color tempColor = sr.color;
		tempColor.a = opacity;
		sr.color = tempColor;
	}
	
	// Update is called once per frame
	void Update () {
		angle += angleSpeed * Time.deltaTime;
		if (angle >= 360f) angle -= 360f;
		if (angle >= 90f && angle <= 270f){
			sr.sortingOrder = 1;
		} else {
			sr.sortingOrder = 20;
		}
		transform.localPosition = new Vector3(distance * Mathf.Cos(angle * Mathf.Deg2Rad), transform.localPosition.y, distance * Mathf.Sin(angle * Mathf.Deg2Rad));

		float tempOpacity = sr.color.a;
		Color tempColor = sr.color;
		tempOpacity -= opacityReduction * Time.deltaTime;
		if (tempOpacity <= 0f){
			tempOpacity = 0f;
		}
		tempColor.a = tempOpacity;
		sr.color = tempColor;
	}
}
