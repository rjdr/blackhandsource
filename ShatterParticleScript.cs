using UnityEngine;
using System.Collections;

public class ShatterParticleScript : MonoBehaviour {
	Texture2D tex;
	Color color;
	// Opacity of the particle and how quickly it fades
	float opacity = 3f;
	public float dOpacity = .011f;
	// Velocity of the particle and how quickly it accelerates up
	public float dy = -6f;
	float ddy = .2f * 60;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//print (timer--);
		opacity = opacity - dOpacity*(Time.deltaTime*60);
		//print (opacity);
		if (opacity <= 0f){
			opacity = 0f;
			Destroy(gameObject, 0f);
		} else {
			float localOpacity = opacity;
			if (opacity > 1f) opacity = 1f;
			Color updatedColor = GetComponent<SpriteRenderer>().color;
			updatedColor.a = localOpacity;
			GetComponent<SpriteRenderer>().color = updatedColor;

			Vector3 velocity = new Vector3(0f, dy, 0f) * Time.deltaTime;
			transform.Translate(velocity);
			dy += ddy * Time.deltaTime;
			if (dy > 12f){
				dy = 12f;
			}
		}
	}
	// Use this to declare initial values
	public void Instantiate(Vector3 pos, Color c){
		float scale = Random.value + .5f;
		transform.localScale = new Vector3(20f*scale, 20f*scale, .04f);
		ddy += Random.value * 10f;

		pos.z -= Random.value  - .5f;
		transform.position = pos;

		color = c;
		tex = new Texture2D(1,1);
		tex.SetPixel(0, 0, color);
		tex.Apply();
		GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(.5f, .5f));
		//GetComponent<SpriteRenderer>().material.shader = Shader.Find("Particles/Additive");
		// Put on the same layer as the player so that the player can move through a cloud of particles that surround him
		gameObject.layer = LayerMask.NameToLayer("Shifter");
		GetComponent<SpriteRenderer>().sortingOrder = 2;

		if (color.a == 0f){
			opacity = 0f;
		}
	}
}