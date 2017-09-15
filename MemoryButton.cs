using UnityEngine;
using System.Collections;

// Chooses which enemy to transform into
public class MemoryButton : MonoBehaviour {
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
	void Update(){
		hc.SlowTime();
	}
	void OnMouseEnter(){
		transform.localScale = hoveredScale;
	}
	void OnMouseExit(){
		transform.localScale = normalScale;
	}
	void OnMouseDown(){
		GameObject p = null;
		if (name == "Cat"){
			p = (GameObject)Resources.Load("Cat");
		} else if (name.Contains("BasicColonel")){
			p = (GameObject)Resources.Load("BasicColonel");
		} else if (name.Contains("American")){
			p = (GameObject)Resources.Load("American");
		} else if (name.Contains("BasicSoldier")){
			p = (GameObject)Resources.Load("BasicSoldier");
		} else if (name == "Sink"){
			p = (GameObject)Resources.Load("Sink");
		} else if (name == "Bird"){
			p = (GameObject)Resources.Load("Bird");
		} else if (name == "Toilet"){
			p = (GameObject)Resources.Load("Toilet");
		// The turret's direction is based on the direction the player is facing
		} else if (name == "Turret" || name == "TurretFlipped"){
			if (spawn.transform.localScale.x >= 0f){
				p = (GameObject)Resources.Load("Turret");
			} else {
				p = (GameObject)Resources.Load("TurretFlipped");
			}
		} else if (name == "RecordPlayer"){
			p = (GameObject)Resources.Load("RecordPlayer");
		} else if (name == "Turkey"){
			p = (GameObject)Resources.Load("Turkey");
		}
		if (p != null){
			p = (GameObject)Instantiate(p);
			p.GetComponent<PossessableScript>().SetAsMemory(true);
			// Set the timer for limited possession
			p.GetComponent<PossessableScript>().memoryTimer = hc.memoryTimer;
			p.transform.position = hc.transform.position;

			// Toilet has a small issue with the spawn point. Move it up a bit to avoid falling through floors
			if (name == "Toilet"){
				p.transform.position += new Vector3(0f, 1f, 0f);
			}
			if (name == "Sink"){
				p.transform.position += new Vector3(0f, .5f, 0f);
			}

			// Escaping from an object requires a reference to The Hand, so set it here
			p.GetComponent<PossessableScript>().SetHand(hc.gameObject);
			p.GetComponent<GenericEnemy>().Init();
			// We want to always have life maximized while possessing
			p.GetComponent<GenericEnemy>().life = p.GetComponent<GenericEnemy>().maxLife;
			hc.PossessMemory(p);
		}
		hc.DeactivateMemoryMenu();
	}
}
