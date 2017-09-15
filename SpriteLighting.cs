using UnityEngine;
using System.Collections;

public class SpriteLighting : MonoBehaviour {
	Color spriteSkyColor = new Color(1f, 1f, 1f, 1f);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Color previousSkyColor = RenderSettings.ambientSkyColor;
		RenderSettings.ambientSkyColor = spriteSkyColor;
	}
}
