using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {
	Rigidbody2D rb;
	float timer = 0f;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector3(0f, Random.Range(3f, 8f));
		// How long the bubble exists for
		timer = Random.Range(1f, 3f);

		transform.localScale *= Random.Range(.25f, 2f);
		Color c = GetComponent<SpriteRenderer>().color;
		c.a *= Random.Range(.25f, 1f);
		GetComponent<SpriteRenderer>().color = c;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f){
			Destroy(gameObject, 0f);
		}
	}
}
