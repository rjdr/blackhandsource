using UnityEngine;
using System.Collections;

public class ShatterParticleCube : MonoBehaviour {
	Texture2D tex;
	Color color;
	float opacity = 1f;
	float dOpacity = .01f;
	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3(.5f, .5f, .04f);
		// Creates a texture
		tex = new Texture2D(1,1);
		//color = new Color(1f, 0f, 1f, .5f);
		//tex.SetPixel(0, 0, pixels[i+j*w]);
		tex.SetPixel(0, 0, color);
		tex.Apply();
		GetComponent<MeshRenderer>().material.shader = Shader.Find("Particles/Additive");
		GetComponent<MeshRenderer>().material.mainTexture = tex;
		GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, .1f);
	}
	
	// Update is called once per frame
	void Update () {
		//print (timer--);
		opacity = opacity - dOpacity*(Time.deltaTime*60);
		print (opacity);
		if (opacity <= 0f){
			opacity = 0f;
			Destroy(gameObject, 0f);
		} else {
			//Color updatedColor = GetComponent<MeshRenderer>().material.color;
			//updatedColor.a = opacity;
			//GetComponent<MeshRenderer>().material.color = updatedColor;
		}
	}
	public void Instantiate(Vector3 pos, Color c){	
		transform.position = pos;
		color = c;
	}
}