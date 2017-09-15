using UnityEngine;
using System.Collections;

public class WalkerBot : GenericEnemy {
	Animator anim;
	PossessableScript ps;
	float speed = 3.5f;
	Color c = new Color(1f, .2f, .6f);
	Color black = new Color(.1f, .1f, .1f);
	public GameObject body;
	MeshRenderer mr;
	LineRenderer lr;
	Transform laserGun;
	GameObject FastBullet;
	// Use this for initialization
	void Start () {
		anim = transform.Find("FinalWalkerBot").GetComponent<Animator>();
		ps = GetComponent<PossessableScript>();
		mr = body.GetComponent<MeshRenderer>();
		lr = transform.Find("LineRenderer").GetComponent<LineRenderer>();
		laserGun = transform.Find("lasergun");

		FastBullet = (GameObject)Resources.Load("FastBullet");
	}

	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			if (Input.GetKey("d")){
				anim.SetFloat("direction", 1f);
				anim.SetBool("walking", true);
				GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0f);
			} else if (Input.GetKey("a")){
				anim.SetFloat("direction", -1f);
				anim.SetBool("walking", true);
				GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0f);
			}
			else {
				anim.SetFloat("direction", 0f);
				anim.SetBool("walking", false);
				GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
			}
			// Instantiates a bullet
			if (Input.GetMouseButtonDown(0)){
				GameObject tempBullet = (GameObject)Instantiate(FastBullet);
				tempBullet.transform.position = laserGun.position + laserGun.up;
				tempBullet.GetComponent<FastBullet>().Fire(laserGun.up);
				tempBullet.GetComponent<FastBullet>().spawn = gameObject;
			}

			UpdateArm(GetCursorPoint());
		}
		else {
			Vector3 end = transform.position + new Vector3(5f, 2f, 0f);
			UpdateArm(end);
		}

	}

	void UpdateArm(Vector3 end){
		//GetComponent<Rigidbody2D>().velocity = new Vector2(-2f, 0f);
		Vector3 startPoint = transform.position;
		Vector3 endPoint = end;
		Vector3 midPoint = startPoint + (endPoint - startPoint)*.5f + new Vector3(0f, 2f, 6f);

		Vector3 lastPoint = new Vector3(0f, 0f, 0f);
		lr.SetPosition(0, startPoint);
		for (int i = 1; i < 7; i++){
			lastPoint = BezierPosition(endPoint, midPoint, startPoint, (float)i / 7);
			lr.SetPosition(i, lastPoint);
		}

		lr.SetPosition(7, endPoint);
		laserGun.transform.position = endPoint;
		laserGun.up = laserGun.transform.position - transform.position;
	}

	public override void SpecialHighlighting(){
		//print(transform.Find("group8"));
		//print(transform.Find("group8").Find("body").gameObject.GetComponent<MeshRenderer>());
		print("entered");
		mr.material.SetColor("_OutlineColor", c);
	}
	public void OnMouseExit(){
		mr.material.SetColor("_OutlineColor", black);
	}
}
