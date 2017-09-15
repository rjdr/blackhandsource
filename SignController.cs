using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SignController : MonoBehaviour {
	// Makes sure billboards face the right way, etc.
	GameObject sign;
	Text textObj;
	// Use this for initialization
	void Start () {
		sign = transform.Find("Sign").gameObject;
		textObj = sign.transform.Find("Text").gameObject.GetComponent<Text>();
		textObj.text = "Test dialogue here";
	}
	
	// Update is called once per frame
	void Update () {
		if ((transform.localScale.x < 0 && sign.transform.localScale.x > 0) || (transform.localScale.x > 0 && sign.transform.localScale.x < 0)){
			Vector3 tempScale = sign.transform.localScale;
			tempScale.x = -tempScale.x;
			sign.transform.localScale = tempScale;
		}
	}
}
