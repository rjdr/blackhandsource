using UnityEngine;
using System.Collections;

public class UnderwaterBoulder : MonoBehaviour {
	public GameObject explosion;
	float damageValue = 1.5f;
	float rotSpeed = 0f;
	float timer = 5f;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-20f, -14f), 0f);
		transform.eulerAngles = new Vector3(0f, 0f, Random.Range(-120f, 220f));
		float s = Random.Range(1.2f, 2.9f);
		transform.localScale = new Vector3(s, s, s);
		rotSpeed = Random.Range(-50f, 50f);
	}
	
	// Update is called once per frame
	void Update () {
		// Slowly rotates
		transform.eulerAngles += new Vector3(0f, 0f, rotSpeed * Time.deltaTime);
		timer -= Time.deltaTime;
		if (timer < 0f){
			Destroy(gameObject, 0f);
		}
	}

	// Damages things once it hits them
	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" || (col.GetComponent<Torpedo>() && !col.GetComponent<Torpedo>().reflected)){
			// Once it hits an enemy, explode and do damage
			if (col.gameObject.GetComponent<GenericEnemy>()){
				Camera.main.GetComponent<CameraTarget>().AddShake(.5f);
				col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageValue);
				GameObject g = Instantiate(explosion);
				g.transform.position = transform.position;
				Destroy(gameObject, 0f);
			} else {
				Camera.main.GetComponent<CameraTarget>().AddShake(.5f);
				GameObject g = Instantiate(explosion);
				g.transform.position = transform.position;
				Destroy(col.gameObject, 0f);
				Destroy(gameObject, 0f);
			}
		}
	}
}
