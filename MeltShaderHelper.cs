using UnityEngine;
using System.Collections;

public class MeltShaderHelper : MonoBehaviour {
	Material mat;
	// Use this for initialization
	void Start () {
		mat = GetComponent<MeshRenderer>().material;
		for (int count = 0; count <= 256; count ++)
		{
			mat.SetVector("_Offset"  + count.ToString(), new Vector2 (Random.Range(1f, 1.25f), 0));
		}
	}
	
	// Update is called once per frame
	void Update () {
		mat.SetFloat("_Timer", mat.GetFloat("_Timer") + Time.deltaTime);
		print(mat.GetFloat("_Timer"));
	}
}
