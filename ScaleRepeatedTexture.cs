using UnityEngine;
using System.Collections;

public class ScaleRepeatedTexture : MonoBehaviour {
	public float repeatLevel = 1f;
	// Sets the texture to smoothly repeat regardless of scale
	void Start () {
		MeshRenderer mr = GetComponent<MeshRenderer>();
		float x = transform.lossyScale.x;
		float z = transform.lossyScale.z;
		mr.material.mainTextureScale = new Vector2(repeatLevel*x, repeatLevel*z);
	}

}
