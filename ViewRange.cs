using UnityEngine;
using System.Collections;

public class ViewRange : MonoBehaviour {
	ArrayList children = new ArrayList();
	ArrayList positions = new ArrayList();
	float distance = 1f;
	// Updates every N frames
	int maxUpdateCount = 3;
	int updateCount = 3;
	// Use this for initialization
	void Start () {
		// Gets the children transforms
		foreach (Transform t in transform){
			Transform lowerMostChild = t.Find("p");
			children.Add(lowerMostChild);
			distance = (lowerMostChild.transform.position - lowerMostChild.parent.position).magnitude;
		}
	}
	
	// Update is called once per frame
	void Update () {
		updateCount++;
		if (updateCount >= maxUpdateCount){
			updateCount = 0;
			UpdatePositions();
		}
	}

	void UpdatePositions(){
		// Direction of the view area is important for determining the right vector
		Transform topParent = transform;
		if (transform.parent){
			topParent = transform.parent;
		}
		float direction = (topParent.localScale.x > 0) ? 1 : -1;
		for (int i = 0; i < children.Count; i++){
			Transform lowerMostChild = (Transform)children[i];
			Vector3 tempRight = lowerMostChild.right;
			tempRight.x *= direction;
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, tempRight, distance);
			float minDistance = distance;
			foreach (RaycastHit2D hit in hits){
				if (!hit.collider.isTrigger){
					if (hit.distance < minDistance)
						minDistance = hit.distance;
				}
			}

			//lowerMostChild.position = transform.position + lowerMostChild.right * minDistance;
			Vector3 tempScale = lowerMostChild.parent.localScale;
			tempScale.x = minDistance / distance;
			lowerMostChild.parent.localScale = tempScale;
		}
	}
}
