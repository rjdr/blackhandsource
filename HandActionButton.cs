using UnityEngine;
using System.Collections;

public class HandActionButton : MonoBehaviour {
	GameObject spawn;
	HandController hc;
	public string name;
	Vector3 normalScale;
	Vector3 hoveredScale;
	// Use this for initialization
	void Start () {
		spawn = GameObject.Find("TheHand");
		hc = spawn.GetComponent<HandController>();
		normalScale = transform.localScale;
		hoveredScale = normalScale * 1.25f;
		// Automatically get the name if none is provided
		if (name.Length == 0){
			string tempName = gameObject.name;
			tempName.Substring(0, tempName.Length - "Icon".Length);
			name = tempName;
		}
	}
	void OnMouseEnter(){
		print("ENTERED");
		transform.localScale = hoveredScale;
	}
	void OnMouseExit(){
		transform.localScale = normalScale;
	}
	void OnMouseDown(){
		if (name == "Attack"){
			hc.ActivateAttack();
			hc.JustSelectedMenuAction();
		} else if (name == "Transmit"){
			hc.ActivateTransmit();
			hc.JustSelectedMenuAction();
		} else if (name == "Eat"){
			hc.EatObject();
			hc.JustSelectedMenuAction();
		} else if (name == "Possess"){
			hc.PossessHuman();
			hc.JustSelectedMenuAction();
		} else if (name == "Depossess"){
			hc.Depossess();
			hc.JustSelectedMenuAction();
		}
		else if (name == "Memory"){
			hc.ActivateMemoryMenu(transform.position);
			hc.JustSelectedMenuAction();
		}
	}
}
