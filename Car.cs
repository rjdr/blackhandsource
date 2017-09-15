using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	public bool IsBrown;
	public Texture2D brownTexture;
	public Texture2D brownWheelTexture;
	// Use this for initialization
	void Start () {
		if (IsBrown){
			transform.Find("pCube1").GetComponent<MeshRenderer>().material.mainTexture = brownTexture;
			transform.Find("pCube2").GetComponent<MeshRenderer>().material.mainTexture = brownTexture;
			transform.Find("pCube4").GetComponent<MeshRenderer>().material.mainTexture = brownTexture;

			transform.Find("pCylinder1").GetComponent<MeshRenderer>().material.mainTexture = brownWheelTexture;
			transform.Find("pCylinder2").GetComponent<MeshRenderer>().material.mainTexture = brownWheelTexture;
			transform.Find("pCylinder5").GetComponent<MeshRenderer>().material.mainTexture = brownWheelTexture;
			transform.Find("pCylinder6").GetComponent<MeshRenderer>().material.mainTexture = brownWheelTexture;
			transform.Find("pCylinder7").GetComponent<MeshRenderer>().material.mainTexture = brownWheelTexture;
		}
	}

}
