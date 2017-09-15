using UnityEngine;
using System.Collections;

public class BuildingEater : MonoBehaviour {
	// Use this for initialization
	Vector3 destPoint;
	Vector3 startPoint;
	float timer = 0f;
	public float maxTimer = 10f;
	float delayFallTimer = 2f;
	float buildingFallTimer = 0f;
	public float maxBuildingFallTimer = 8f;
	public float baseShiftDist = 105f;	// How far down we move the base point
	public float shakeAmount = .1f;
	Transform building;
	Vector3 buildingStartPoint;
	Vector3 buildingDestPoint;

	bool active = false;

	void Start () {
		GetComponent<SpriteRenderer>().material.SetFloat("_BumpAmt", 9.1f);
		transform.Find("Body1").GetComponent<SpriteRenderer>().material.SetFloat("_BumpAmt", 15.1f);
		transform.Find("Body2").GetComponent<SpriteRenderer>().material.SetFloat("_BumpAmt", 15.1f);
		GetComponent<SpriteRenderer>().material.SetVector("_Color", new Vector4(1f, 1f, 1f, 1f));
		float dist = 5f / transform.position.z;
		transform.Find("Body1").GetComponent<SpriteRenderer>().material.SetVector("_Color", new Vector4(1f, 1f, 1f, 1f));
		transform.Find("Body2").GetComponent<SpriteRenderer>().material.SetVector("_Color", new Vector4(1f, 1f, 1f, 1f));

		destPoint = transform.position;
		if (GetComponent<BoxCollider2D>()){
			GetComponent<BoxCollider2D>().size += new Vector2(0f, 100f);
		}
		if (transform.Find("ColliderObject")){
			if (transform.Find("ColliderObject").GetComponent<BoxCollider2D>()){
				transform.Find("ColliderObject").GetComponent<BoxCollider2D>().size += new Vector2(0f, 100f);
			}
		}
		startPoint = destPoint - transform.up * baseShiftDist;
		building = transform.Find("Building");
		buildingStartPoint = building.position;
		buildingDestPoint = buildingStartPoint + Vector3.up * -50f;
		building.transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(startPoint, destPoint, timer/maxTimer);
		// Only executes once active
		if (!active){
			return;
		}
		timer += Time.deltaTime;

		// Building begins to drop once reaching max position
		if (timer >= maxTimer && buildingFallTimer <= maxBuildingFallTimer){
			delayFallTimer -= Time.deltaTime;
			if (delayFallTimer <= 0f){
				buildingFallTimer += Time.deltaTime;
				building.localPosition = Vector3.Lerp(buildingStartPoint, buildingDestPoint, buildingFallTimer / maxBuildingFallTimer);
				// Adds some shake to the building
				building.position += new Vector3(Mathf.Cos(Time.time*25f)*shakeAmount, 0f, Mathf.Cos(Time.time*25f)*shakeAmount);
			}
		}
	}

	// Activates the object when touched
	void OnTriggerStay2D(Collider2D col){
		if (!active && col.gameObject.tag == "Player"){
			active = true;
		}
	}
}
