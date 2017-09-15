using UnityEngine;
using System.Collections;

public class LightningScript : MonoBehaviour {
	Color color = new Color(1f, 1f, 1f, 1f);
	Color maxColor = new Color(1f, 1f, 1f, 1f);
	Material mat;
	GameObject plane;
	GameObject backPlane;

	bool matEnabled = false;
	float reduction = .98f;
	public float countDown = 0f;
	float steadyReduction = .02f;
	// Use this for initialization
	void Start () {
		plane = transform.Find("LightningFill").gameObject;
		backPlane = transform.Find("LightningFillBack").gameObject;
		mat = plane.GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		//color.a -= color.a * ((Time.deltaTime * 60f * (1f-reduction)));
		//color.a *= reduction;
		//print (Time.deltaTime + " , " + color.a);
		// Winds down the visibility of the lightning before disabling it
		if (matEnabled){
			countDown -= Time.deltaTime;
			if (countDown <= 0f){
				color.a -= steadyReduction * Time.deltaTime * 60f;
				mat.color = color;
				if (color.a <= .02f){
					plane.SetActive(false);
					backPlane.SetActive(false);
				}
			}
		}
	}
	/// <summary>
	/// Starts a flash of lightning
	/// </summary>
	public void EnableFlash(float waitT = 0f){
		countDown = waitT;
		color = maxColor;
		mat.color = color;
		plane.SetActive(true);
		//backPlane.SetActive(true);
		matEnabled = true;
	}
}
