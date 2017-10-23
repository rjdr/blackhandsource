using UnityEngine;
using System.Collections;

public class BackgroundEntrance : GenericEnemy {
	Transform triangle;
	public float transitionDistance = 1f; 			// How far we move into the background when entering the tunnel
	float cycleTimer = 0f;
	float maxCycleTimer = .25f;
	float multiplier = 1f;
	Vector3 minSize;
	Vector3 maxSize;
	// Use this for initialization
	void Start () {
		triangle = transform.Find("trianglearrow");
		maxSize = triangle.localScale;
		minSize = maxSize * .5f;
	}
	
	// Update is called once per frame
	void Update () {
		cycleTimer += Time.deltaTime*multiplier;
		if (cycleTimer >= maxCycleTimer){
			multiplier = -1f;
		} else if (cycleTimer <= 0f){
			multiplier = 1f;
		}
		triangle.localScale = Vector3.Lerp(minSize, maxSize, cycleTimer);
	}

	// Swap between the background and foreground
	public override void ChildTriggerStayed(GameObject child, GameObject hit){
		if (Input.GetKeyDown(KeyBindings.w) && hit.tag == "Player"){
			hit.GetComponent<HandController>().SwapPlanes(transitionDistance);
		}
	}
}
