using UnityEngine;
using System.Collections;

public class FlipPlatform : MonoBehaviour {
	Animation anim;
	bool down = false;
	float comeUpTimer = 0f;
	float maxComeUpTimer = 2f;
	float minFallSpeed = -6f;
	AudioSource ass;
	AudioClip metalBend;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation>();
		anim["Drop"].speed = 1.5f;
		anim["Raise"].speed = 1.5f;
		anim["Unpressure"].speed = 1.25f;
		ass = gameObject.AddComponent<AudioSource>() as AudioSource;
		ass.volume = .3f;
		metalBend = Resources.Load("Audio/metalbend") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
		if (down){
			comeUpTimer += Time.deltaTime;
			if (comeUpTimer >= maxComeUpTimer){
				down = false;
				anim.Play("Raise");
				GetComponent<BoxCollider2D>().enabled = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (!down && (col.gameObject.tag == "Player" || col.gameObject.tag == "Possessable")){
			print(col.gameObject.GetComponent<Rigidbody2D>().velocity);
			if (col.gameObject.GetComponent<Rigidbody2D>() && col.gameObject.GetComponent<Rigidbody2D>().velocity.y <= minFallSpeed){
				comeUpTimer = 0f;
				Vector3 tempVelocity = col.gameObject.GetComponent<Rigidbody2D>().velocity;
				tempVelocity.y = 0f;
				col.gameObject.GetComponent<Rigidbody2D>().velocity = tempVelocity;
				down = true;
				anim.Play("Drop");
				GetComponent<AudioSource>().PlayOneShot(metalBend);
				Camera.main.GetComponent<CameraTarget>().AddSoundRing(transform.position, 3f, transform);
				GetComponent<BoxCollider2D>().enabled = false;
			} else {
				anim["Pressure"].speed = 1f;
				anim.Play("Pressure");
			}
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if (!down && (col.gameObject.tag == "Player" || col.gameObject.tag == "Possessable")){
			anim.Play("Unpressure");
		}
	}
}
