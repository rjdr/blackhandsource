using UnityEngine;
using System.Collections;

public class PhantomScript : MonoBehaviour {
	Transform spawn;
	SpriteRenderer sr;
	//Queue positions = new Queue();
	Vector3 lastPos;
	//Queue sprites = new Queue();
	Sprite lastSprite;
	float time = 0f;
	float maxTime = .1f;
	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
		spawn = transform.parent;
		transform.parent = null;
		//positions.Enqueue(transform.position);
		lastPos = spawn.transform.position;
		//sprites.Enqueue(spawn.GetComponent<SpriteRenderer>().sprite);
		lastSprite = spawn.GetComponent<SpriteRenderer>().sprite;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= maxTime){
			// Disables self if the parent is gone
			if (spawn == null || sr == null){
				gameObject.SetActive(false);
				return;
			}
			time = 0f;
			//transform.position = (Vector3)positions.Dequeue();
			//sr.sprite = (Sprite)sprites.Dequeue();
			transform.position = lastPos;
			sr.sprite = lastSprite;
			lastPos = spawn.position;
			lastSprite = spawn.GetComponent<SpriteRenderer>().sprite;
			//positions.Enqueue(spawn.position);
			//sprites.Enqueue(spawn.GetComponent<SpriteRenderer>().sprite);
		}
	}
}
