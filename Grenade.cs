using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	float timer = 3f;
	float turnRed = .75f;
	float damageVal = 4f;
	public GameObject explosion;
	SpriteRenderer sp;
	ArrayList targets = new ArrayList();
	// Use this for initialization
	void Start () {
		sp = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		// Turns to red before exploding
		if (timer < turnRed){
			sp.color = Vector4.Lerp(new Vector4(1f, 0f, 0f, 1f), new Vector4(1f, 1f, 1f, 1f), timer / turnRed);
			if (timer < 0f){
				GameObject g = Instantiate(explosion);
				g.transform.position = transform.position;
				g.transform.localScale *= 2f;
				Camera.main.GetComponent<CameraTarget>().AddSoundRing(transform.position, 15f, g.transform);
				// Damages objects when it's destroyed
				for (int i = 0; i < targets.Count; i++){
					if (targets[i] != null){
						((GameObject)targets[i]).GetComponent<GenericEnemy>().DelayedDamage(damageVal);
					}
				}
				Destroy(gameObject, 0f);
			}
		}
	}

	// For when objects enter the field of damage, add them to a list
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && !targets.Contains(col.gameObject)){
			targets.Add(col.gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>() && targets.Contains(col.gameObject)){
			targets.Remove(col.gameObject);
		}
	}
}
