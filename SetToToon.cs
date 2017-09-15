using UnityEngine;
using System.Collections;

public class SetToToon : MonoBehaviour {
	public bool setChildrenToToon = true;
	// Sets an object to use a toon shader without creating a new material
	void Start () {
		if (gameObject.GetComponent<MeshRenderer>()){
			gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Toon/Basic");
		}
		if (setChildrenToToon){
			RecursiveToon(transform);
		}
	}

	// Recursively sets each object to have a toon shader
	void RecursiveToon(Transform trans){
		foreach (Transform t in trans){
			if (t.GetComponent<MeshRenderer>()){
				t.GetComponent<MeshRenderer>().material.shader = Shader.Find("Toon/Basic");
			}
			RecursiveToon(t);
		}
	}
}
