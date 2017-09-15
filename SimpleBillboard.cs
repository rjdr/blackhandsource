using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleBillboard : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	void LateUpdate(){
		transform.LookAt(Camera.main.transform);
	}
}
