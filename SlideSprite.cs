using UnityEngine;
using System.Collections;

public class SlideSprite : MonoBehaviour {
	public Vector2 speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().material.mainTextureOffset += speed * Time.deltaTime;
	}
}
