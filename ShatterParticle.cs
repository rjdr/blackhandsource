using UnityEngine;
using System.Collections;

public class ShatterParticle : MonoBehaviour {
	Texture2D tex;
	Color color;
	GameObject rect;
	Vector3 pos;
	float timer = 1.25f;

	// Use this for initialization
	//void Start () {
	public ShatterParticle(Vector3 pos){
		rect = GameObject.CreatePrimitive(PrimitiveType.Cube);
		rect.transform.localScale = new Vector3(.5f, .5f, .04f);
		rect.transform.position = pos;
		// Creates a texture
		tex = new Texture2D(1,1);
		color = new Color(1f, 0f, 1f, .5f);
		//tex.SetPixel(0, 0, pixels[i+j*w]);
		tex.SetPixel(0, 0, color);
		tex.Apply();
		rect.GetComponent<MeshRenderer>().material.shader = Shader.Find("Particles/Additive");
		rect.GetComponent<MeshRenderer>().material.mainTexture = tex;
		rect.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
	}
	public void Instantiate(Vector3 pos){
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f){
			Destroy(gameObject, 0f);
		}
	}
}
