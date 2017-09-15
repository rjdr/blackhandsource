using UnityEngine;
using System.Collections;

public class HotCoals : MonoBehaviour {
	void Start(){
		if (transform.Find("Flame")){
			Transform f = transform.Find("Flame");
			f.gameObject.GetComponent<Animator>().speed *= Random.Range(.9f, 1.1f);
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent<GenericEnemy>()){
			print (col.gameObject);
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(10);
		}
	}
}
