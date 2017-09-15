using UnityEngine;
using System.Collections;

public class PullHook : MonoBehaviour {
	float velocity = 0f;
	GameObject player;
	MeshRenderer mr;
	public GameObject connector;
	Color c = new Color(1f, .2f, .6f);
	Color black = new Color(.1f, .1f, .1f);
	// Use this for initialization
	void Start () {
		mr = transform.Find("pullhandle").gameObject.GetComponent<MeshRenderer>();
		mr.material.SetColor("_OutlineColor", black);
		player = GameObject.Find("TheHand");
	}

	// When grabbed by The Hand, add a force in the opposite of the right vector direction to the parent object
	public void Pull(){
		Vector2 force = (Vector2)(transform.right * -20);
		Vector2 playerPull = 2000f * (Vector2)(transform.position - player.transform.position).normalized;
		player.GetComponent<Rigidbody2D>().AddForce(playerPull);
		connector.GetComponent<Rigidbody2D>().velocity = force;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		if (player.activeSelf){
			if (player.GetComponent<HandController>().AttackMenu.activeSelf == false || player.GetComponent<HandController>().grabbedObject == null){
				// Checks if player can make contact with the center or edges of the beam
				if (player.GetComponent<HandController>().ClearPathToTarget(transform.position)){
					//Pull();
					Invoke("Pull", .25f);
				}
			}
		}
	}

	public void OnMouseEnter(){
		mr.material.SetColor("_OutlineColor", c);
	}

	public void OnMouseExit(){
		mr.material.SetColor("_OutlineColor", black);
	}
}
