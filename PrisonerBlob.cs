using UnityEngine;
using System.Collections;

public class PrisonerBlob : DamagingObject {
	float maxActiveTimer = 3f;	
	// Update is called once per frame
	void Update () {
		maxActiveTimer -= Time.deltaTime;
		if (maxActiveTimer <= 0f){
			Destroy(gameObject, 0f);
		}
	}
}
