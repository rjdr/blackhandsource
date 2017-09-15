using UnityEngine;
using System.Collections;

public class Shatterable : MonoBehaviour {
	public bool shattered = false;
	public Vector3 pivot = new Vector3(0f, 0f, 0f);
	// Use this for initialization
	void Start () {
	
	}
	public void Shatter(int spacing = 25){
		shattered = true;
		if (!GetComponent<SpriteRenderer>()){
			print("Error: shattered object has no sprite renderer");
			if (GetComponent<GenericEnemy>()){
				GetComponent<GenericEnemy>().DustCloud();
			}
			return;
		}
		Texture2D originSprite = GetComponent<SpriteRenderer>().sprite.texture;
		Rect bounds = GetComponent<SpriteRenderer>().sprite.textureRect;
		int w = (int)bounds.width;
		int h = (int)bounds.height;
		int startX = (int)bounds.x;
		int startY = (int)bounds.y;
		int n = 0;
		float ppu = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		
		Color[] pixels = originSprite.GetPixels(startX, startY, w, h);
		// Goes through the array of pixels, generating blocks
		for (int i = 0; i < w; i += spacing){
			for (int j = 0; j < h; j+= spacing){
				Color c = pixels[i+j*w];
				if (c.a > 0f){
					GameObject s = (GameObject)Instantiate(Resources.Load("ShatterParticleSprite"));
					s.GetComponent<ShatterParticleScript>().Instantiate(transform.position + new Vector3(((i)/ppu - .1f)*transform.localScale.x,
						(-(j)/ppu)*transform.localScale.y, 0f) + pivot, pixels[i+j*w]);
					n++;
				}
			}
		}
	}
}
