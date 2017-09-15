using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
	public float rotSpeed = 5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0f, rotSpeed * Time.deltaTime, 0f));
	}
}
