using UnityEngine;
using System.Collections;

public class BounceScale : MonoBehaviour {
	public float[] times;
	public float[] scales;
	Vector3 startScale;
	float timer = 0f;
	bool started = false;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (started){
			timer += Time.unscaledDeltaTime;
			for (int i = 0; i < times.Length; i++){
				if (timer <= times[i]){
					float f = scales[i];
					Vector3 tempScale = new Vector3(f, f, f);
					if (i == 0){
						transform.localScale = Vector3.Lerp(startScale, tempScale, timer / times[i]);
					} else {
						float g = scales[i-1];
						Vector3 tempScaleStart = new Vector3(g, g, g);
						transform.localScale = Vector3.Lerp(tempScaleStart, tempScale, timer / times[i]);
					}
					break;
				}
			}
		} 
		started = true;
		/*
		if (timer <= times[0]){
			float f = scales[0];
			Vector3 tempScale = new Vector3(f, f, f);
			transform.localScale = Vector3.Lerp(startScale, tempScale, timer / times[0]);
		} else if (timer <= times[1]){
			float f = scales[1];
			Vector3 tempScale = new Vector3(f, f, f);
			transform.localScale = Vector3.Lerp(times[0], tempScale, timer / times[0]);
		}
		*/
	}
}
