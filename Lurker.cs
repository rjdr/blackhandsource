using UnityEngine;
using System.Collections;

public class Lurker : MonoBehaviour {
	Animator anim;
	CameraTarget ct;
	public float minXDist = 4f;
	Vector3 scale;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		ct = Camera.main.GetComponent<CameraTarget>();
		scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		Transform t = ct.GetCurrentPlayer();
		if (Mathf.Abs(t.position.x - transform.position.x) < minXDist){
			anim.SetBool("hiding", true);
		} else {
			anim.SetBool("hiding", false);
		}
		if (t.position.x < transform.position.x){
			scale.x = -Mathf.Abs(scale.x);
			transform.localScale = scale;
		} else {
			scale.x = Mathf.Abs(scale.x);
			transform.localScale = scale;
		}
	}
}
