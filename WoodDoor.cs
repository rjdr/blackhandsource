using UnityEngine;
using System.Collections;

public class WoodDoor : MonoBehaviour {
	bool opened = false;
	float openTimer = 0f;
	float maxOpenTimer = .25f;
	float beginCloseTimer = 0f;
	float maxBeginCloseTimer = 3.5f;
	float direction = 1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (opened){
			beginCloseTimer += Time.deltaTime;
			if (beginCloseTimer > maxBeginCloseTimer){
				transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, 80f * direction, 0f), new Vector3(0f, 0f, 0f), (beginCloseTimer - maxBeginCloseTimer)/maxOpenTimer);
				if (beginCloseTimer > (maxBeginCloseTimer + maxOpenTimer)){
					beginCloseTimer = 0f;
					opened = false;
					GetComponent<Collider2D>().isTrigger = false;
				}
			} else {
				openTimer += Time.deltaTime;
				transform.localEulerAngles = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(0f, 80f * direction, 0f), openTimer/maxOpenTimer);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (!opened){
			GetComponent<Collider2D>().isTrigger = true;
			opened = true;
			beginCloseTimer = 0f;
			openTimer = 0f;
			if (col.transform.position.x < transform.position.x){
				direction = -1f;
			} else {
				direction = 1f;
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (!opened && !col.isTrigger){
			GetComponent<Collider2D>().isTrigger = true;
			opened = true;
			beginCloseTimer = 0f;
			openTimer = 0f;
			if (col.transform.position.x < transform.position.x){
				direction = -1f;
			} else {
				direction = 1f;
			}
		}
	}
}
