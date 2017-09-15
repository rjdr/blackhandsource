using UnityEngine;
using System.Collections;

public class TripBomb : GenericEnemy {
	public bool isParent = true;
	float damageVal = 7f;
	Transform child;
	Transform trigger;
	Transform line;
	public GameObject explosion;
	// Use this for initialization
	void Start () {
		child = transform.Find("ChildBomb");
		if (isParent){
			trigger = transform.Find("Trigger");
			line = transform.Find("Line");

			// Extends a line from one end to the other
			trigger.GetComponent<EdgeCollider2D>().points = new Vector2[]{new Vector2(0f, 0f), new Vector2(child.localPosition.x, child.localPosition.y)};
			line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(child.localPosition.x, child.localPosition.y, 0f));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// When touching the line in the middle
	void OnTriggerEnter2D(Collider2D col){
		// Explodes when hit
		if (!col.isTrigger){
			GameObject e = (GameObject)Instantiate(explosion);
			e.transform.position = transform.position;
			e.transform.localScale *= .5f;
			GameObject e2 = (GameObject)Instantiate(explosion);
			e2.transform.position = child.position;
			e2.transform.localScale *= .5f;
			GameObject e3 = (GameObject)Instantiate(explosion);
			e3.transform.position = col.gameObject.transform.position;
			AddSoundRing(e3.transform.position, 8f);

			if (col.gameObject.GetComponent<GenericEnemy>()){
				col.gameObject.GetComponent<GenericEnemy>().InstantDamage(damageVal);
			}

			Destroy(gameObject, 0f);
		}
	}
}
