using UnityEngine;
using System.Collections;

public class BloodTrail : MonoBehaviour {
	float expandTimer = 0f;
	float maxTimer = 1f;
	Vector3 startScale = new Vector3(0f, 0f, 0f);
	public Vector3 maxScale;
	// Use this for initialization
	void Start () {
		maxScale = transform.localScale * Random.Range(.1f, 1f);
		startScale = maxScale / 2f;
		transform.localScale = startScale;
	}
	
	// Update is called once per frame
	void Update () {
		expandTimer += Time.deltaTime;
		transform.localScale = Vector3.Lerp(startScale, maxScale, expandTimer / maxTimer);
	}
}
