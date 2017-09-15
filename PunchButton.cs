using UnityEngine;
using System.Collections;

public class PunchButton : MonoBehaviour {
	Vector3 maxScale;

	float currFill = 0f;
	float maxFill = 1f;
	public float fillRate = 4/60f;
	public float dischargeRate = 0/320f;
	public bool discharging = false; 	// If it discharges, it eventually loses its fill

	float lastLife = 10f;
	float currLife = 10f;
	//Life life;
	Transform player;

	float waitHitTimer = 0f;
	float maxWaitHitTimer = .25f;

	public bool isSoundButton = false;

	public int id = 10;
	// Use this for initialization
	void Start () {
		//fillMeter = transform.Find("FillMeter");
		//maxScale = fillMeter.localScale;
		//life = GetComponent<Life>();
		//currLife = life.life;
		//lastLife = life.life;
		player = GameObject.Find("TheHand").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		// Change fill rate when damaged
		//currLife = life.life;
		/*
		if (currLife != lastLife){
			//if (currLife < lastLife){
			//	currFill = Mathf.Clamp(currFill + fillRate, 0f, 1f);
			//}
		} else if (discharging && currFill > 0f){
			currFill = Mathf.Clamp(currFill - dischargeRate, 0f, 1f);

		}
		*/
		waitHitTimer -= Time.fixedDeltaTime;
	}

	public float GetFill(){
		return currFill / maxFill;
	}
	public void SetFill(float f){
		currFill = f;
	}

	// If there's a horn attached, make a sound to distract enemies
	void MakeSound(){
		if (isSoundButton){
			Camera.main.GetComponent<CameraTarget>().AddSoundRing(transform.position, 10f, transform);
		}
	}

	// Activates the switch
	public void Activate(){
		if (waitHitTimer <= 0f){
			currLife -= 1f;
			currFill = maxFill;
			MakeSound();
			waitHitTimer = maxWaitHitTimer;
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		Activate();
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.name == "GrabPivot"){
			Activate();
		}
	}
}
