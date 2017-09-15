using UnityEngine;
using System.Collections;

public class SimpleWave : MonoBehaviour {
	Material m;
	float c = 0f;
	float speed = 1f;
	// Use this for initialization
	void Start () {
		m = GetComponent<LineRenderer>().material;
		m.SetFloat("Counter", 10);
	}
	
	// Update is called once per frame
	void Update () {
		c += Time.deltaTime;
		m.SetColor("Color", new Color(Random.Range(0f, 1f), 0f, 0f, 1f));
		//m.SetFloat("Counter", c*speed);
	}
}
