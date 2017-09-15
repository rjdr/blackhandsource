using UnityEngine;
using System.Collections;

public class IllumHeadSub : MonoBehaviour {
	float radius = 0f;
	public float angle = 0f;
	float speed = 115f;
	float shootTimer = 0f;
	const float baseShootTimer = 5.75f;
	float maxShootTimer = baseShootTimer;
	float shootInterval = .75f;
	float endShootTimer = 6.5f;

	public GameObject miniLaser;
	public GameObject punchContact;
	// Use this for initialization
	void Start () {
		radius = Mathf.Abs(transform.localPosition.x);
	}
	
	// Update is called once per frame
	void Update () {
		angle += Time.deltaTime * speed;
		transform.localPosition = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

		// Fires a small series of shots
		shootTimer += Time.deltaTime;
		if (shootTimer >= maxShootTimer){
			maxShootTimer += shootInterval;
			if (shootTimer >= endShootTimer){
				shootTimer = 0f;
				maxShootTimer = baseShootTimer;
			}
			//shootTimer = 0f;
			GameObject g = (GameObject)Instantiate(miniLaser);
			g.transform.position = transform.position;
		}
	}

	// Reflects back torpedoes
	void OnTriggerEnter2D(Collider2D col){
		if (col.GetComponent<Torpedo>()){
			col.GetComponent<Torpedo>().Reflect();
			//Camera.main.GetComponent<CameraTarget>().AddShake(.25f);
			GameObject g = (GameObject)Instantiate(punchContact);
			g.transform.position = transform.position;
		}
	}
}
