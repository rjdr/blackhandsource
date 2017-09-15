using UnityEngine;
using System.Collections;

public class BloodSpatter : MonoBehaviour {
	public GameObject parent;
	// Use this for initialization
	void Start () {
	}

	// When the blood hits a solid object/non-spatter, make a stain
	void OnCollisionEnter2D(Collision2D collisionInfo)
	{
		if (!collisionInfo.collider.isTrigger && collisionInfo.gameObject.GetComponent<BloodSpatter>() == null && collisionInfo.gameObject != parent){
			Vector3 collisionNormal = collisionInfo.contacts[0].normal;
			float zRot = transform.eulerAngles.z;
			transform.forward = -collisionNormal;
			Vector3 tempRotation = transform.eulerAngles;
			tempRotation.z = zRot;
			transform.eulerAngles = tempRotation;
			transform.localScale *= 4f;
			GetComponent<Rigidbody2D>().isKinematic = true;
			GetComponent<BoxCollider2D>().enabled = false;

			Destroy(gameObject, 3f);
		}
	}
}
