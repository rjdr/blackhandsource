using UnityEngine;
using System.Collections;

public class BulletBossFog : MonoBehaviour {
	// We'll want some brief fog upon approaching the bullet boss, then fade it out once we get near him
	bool fogOn = true;
	float maxFogIntensity = .045f;
	float fogIntensity = .00f;
	float fogIncrRate = .01f;

	float lastTouchedTimer = 0f;
	float maxLastTouchedTimer = .1f;

	public bool touched = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lastTouchedTimer -= Time.deltaTime;
		if (lastTouchedTimer > 0f){
			FadeFog();
		} else if (!touched) {
			ThickenFog();
		}
	}

	void FadeFog(){
		RenderSettings.fog = fogOn;
		if (fogIntensity > 0){
			fogIntensity -= fogIncrRate*Time.deltaTime;
			if (fogIntensity <= 0f){
				fogIntensity = 0f;
			}
			RenderSettings.fogDensity = fogIntensity;
		}
	}
	void ThickenFog(){
		RenderSettings.fog = fogOn;
		if (fogIntensity < maxFogIntensity){
			
			fogIntensity += fogIncrRate*Time.deltaTime;
			RenderSettings.fogDensity = fogIntensity;
		}
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player"){
			touched = true;
			lastTouchedTimer = maxLastTouchedTimer;
		}
	}
}
