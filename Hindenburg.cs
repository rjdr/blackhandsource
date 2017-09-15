using UnityEngine;
using System.Collections;

public class Hindenburg : MonoBehaviour {
	Transform Leg, BackArm, BackLeg, Arm, Body;
	// Use this for initialization
	void Start () {
		Arm = transform.Find("Arm");
		Body = transform.Find("Body");
		Leg = transform.Find("Leg");
		BackArm = transform.Find("BackArm");
		BackLeg = transform.Find("BackLeg");
	}
	
	// Update is called once per frame
	void Update () {
		Body.localEulerAngles = new Vector3(0f, 0f, Mathf.Cos(Time.time*1f)*2f);
		Arm.localEulerAngles = new Vector3(0f, 0f, -Mathf.Sin(Time.time*1f)*4f);
		BackArm.localEulerAngles = new Vector3(0f, 0f, Mathf.Sin(Time.time*1f)*4f);
	}
}
