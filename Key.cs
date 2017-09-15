using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {
	GameObject hand;
	public GameObject holder = null;		// The object that is holding a key to be dropped
	public bool isHeld = false;
	public int number = 0;
	// Use this for initialization
	void Start () {
		hand = GameObject.Find("TheHand");
		if (holder != null){
			isHeld = true;
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<CircleCollider2D>().enabled = false;
		}
	}
	void Update(){
		// Reenables key once the holder is destroyed
		if (isHeld && (holder == null || holder.activeSelf == false)){
			GetComponent<SpriteRenderer>().enabled = true;
			GetComponent<CircleCollider2D>().enabled = true;
		}
	}
	// Sets the number of the key
	public void SetNumber(int n){
		number = n;
	}
	// Key gets picked up
	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject == hand || (col.transform.parent != null && col.transform.parent.gameObject == hand)){
			hand.GetComponent<HandController>().PickUpKey(number);

			// Popup text for finding a key
			GameObject keyText = (GameObject)Resources.Load("FoundKeyText");
			GameObject g = (GameObject)Instantiate(keyText);
			g.transform.position = transform.position + new Vector3(0f, 1f, 0f);
			Destroy(g, 2f);

			GetComponent<Shatterable>().Shatter();
			Destroy(gameObject, 0f);
		}
	}
}
