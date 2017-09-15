using UnityEngine;
using System.Collections;

public class WindowShatter : MonoBehaviour {
	bool shattered = false;
	void OnTriggerEnter2D(Collider2D col){
		if (!shattered && !col.isTrigger){
			shattered = true;
			Transform g = transform.Find("Glass");
			g.GetComponent<Shatterable>().Shatter(110);
			g.gameObject.SetActive(false);
		}
	}
}
