using UnityEngine;
using System.Collections;

public class NextSectionPoint : MonoBehaviour {
	public string levelName;					// Name of the scene to load
	public bool loadAdditive = false;			// Loads the level additively
	public bool disablePreviousArea = false;	// If this is true, we're simply managing separate game objects. This overrides loadAdditive
	public GameObject previousArea;				// Area we're coming from
	public GameObject nextArea; 				// Area we're going to
	Transform pointOfNoReturn;					// Beyond this point, items will be wiped (if loading a new scene)

	// Use this for initialization
	void Start () {
		pointOfNoReturn = transform.Find("PointOfNoReturn");
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerChild"){
			// If additive, wipe out all objects beyond the point of no return
			if (loadAdditive && !disablePreviousArea){
				GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
				print(allObjects.Length);
				foreach(GameObject g in allObjects){
					if (g.transform.position.x < pointOfNoReturn.position.x && g != pointOfNoReturn.gameObject){
						Destroy(g, .1f);
					}
				}
			} else if (disablePreviousArea){
				nextArea.SetActive(true);
				previousArea.SetActive(false);
				print("Moved to: " + nextArea);
			}
			gameObject.SetActive(false);
		}
	}
}
