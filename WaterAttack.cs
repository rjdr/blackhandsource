using UnityEngine;
using System.Collections;

public class WaterAttack : MonoBehaviour {
	float timer = 0;
	float radius = 2f;
	public float xMultiple = 1f;
	public float yMultiple = 1f;
	public float zMultiple = 1f;
	public float xRadius, yRadius, zRadius = 2f;
	Vector3 startPos;
	// Use this for initialization
	void Start () {
		timer = Random.Range(0f, 20f);
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime*4f;
		//transform.position += new Vector3(1f, 0f, 0f)*Time.deltaTime;
		Vector3 tempPos = startPos + new Vector3(xRadius * Mathf.Cos(timer*xMultiple), yRadius * Mathf.Sin(timer*yMultiple), zRadius * Mathf.Sin(timer*zMultiple));
		transform.position = tempPos;
	}
}
