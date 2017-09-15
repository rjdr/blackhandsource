using UnityEngine;
using System.Collections;

public class FadeToBlack : MonoBehaviour {
	Color color = new Color(0f, 0f, 0f, 1f);
	Color maxColor = new Color(0f, 0f, 0f, 0f);
	Material mat;
	GameObject plane;
	
	bool matEnabled = false;
	float reduction = .98f;
	float steadyReduction = .02f;
	// Use this for initialization
	void Start () {
		plane = transform.Find("BlackFill").gameObject;
		mat = plane.GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		//color.a -= color.a * ((Time.deltaTime * 60f * (1f-reduction)));
		//color.a *= reduction;
		//print (Time.deltaTime + " , " + color.a);
		// Winds down the visibility of the lightning before disabling it
		if (matEnabled){
			color.a += steadyReduction * Time.deltaTime * 60f;
			if (GetComponent<AudioSource>()){
				GetComponent<AudioSource>().volume = Mathf.Clamp(GetComponent<AudioSource>().volume - steadyReduction * Time.deltaTime * 60f, 0f, 1f);
			}
			if (color.a >= .98f){
				//plane.SetActive(false);
				//backPlane.SetActive(false);
				color.a = 1f;
			}
			mat.color = color;
		}
	}
	/// <summary>
	/// Starts a fade out
	/// </summary>
	public void FadeOut(){
		color = maxColor;
		mat.color = color;
		plane.SetActive(true);
		//backPlane.SetActive(true);
		matEnabled = true;
	}
	public float GetAlpha(){
		return color.a;
	}
}
