using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {
	public GameObject spawn;
	public string name;
	Vector3 normalScale;
	Vector3 hoveredScale;
	// Use this for initialization
	void Start () {
		normalScale = transform.localScale;
		hoveredScale = normalScale * 1.25f;
		// Sets icon name automatically
		if (gameObject.name.Contains("Noise")){
			name = "Noise";
		}

		// Automatically get the name if none is provided
		if (name.Length == 0){
			string tempName = gameObject.name;
			tempName.Substring(0, tempName.Length - "Icon".Length);
			name = tempName;
		}
	}
	void OnMouseEnter(){
		transform.localScale = hoveredScale;
	}
	void OnMouseExit(){
		transform.localScale = normalScale;
	}
	void OnMouseDown(){
		if (name == "Attack"){
			spawn.GetComponent<GenericEnemy>().ActivateAttack();
		} else if (name == "Transmit"){
			spawn.GetComponent<GenericEnemy>().ActivateTransmit();
		} else if (name == "Eat"){
			spawn.GetComponent<GenericEnemy>().ActivateEat();
		} else if (name == "Possess"){
			spawn.GetComponent<GenericEnemy>().ActivatePossess();
		} else if (name == "Noise"){
			spawn.GetComponent<GenericEnemy>().ActivateNoise();
		} else if (name == "Depossess"){
			spawn.GetComponent<PossessableScript>().Depossess();
		}
	}
}
