using UnityEngine;
using System.Collections;

public class GrabPivotHandler : MonoBehaviour {
	public Transform parent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((parent.localScale.x < 0 && transform.localScale.x > 0) || (transform.localScale.x < 0 && parent.localScale.x > 0)){
			print ("flip");
			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}
}
