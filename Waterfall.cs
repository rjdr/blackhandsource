using UnityEngine;
using System.Collections;

public class Waterfall : MonoBehaviour {
	Material m;
	public float speed = -1f;
	// Use this for initialization
	void Start () {
		if (GetComponent<LineRenderer>()){
			m = GetComponent<LineRenderer>().material;
		} else {
			m = GetComponent<MeshRenderer>().material;
		}
	}
	
	// Update is called once per frame
	void Update () {
		m.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
	}
}
