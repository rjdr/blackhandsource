using UnityEngine;
using System.Collections;

public class LookScript : MonoBehaviour {

	// Checks for a clear path to an object
	public bool ClearPathToTarget(Transform p, Transform self, Vector3 targetPoint, float maxDist){
		// If the object isn't active, it shouldn't be visible at all
		if (!p.gameObject.activeSelf){
			return false;
		}

		Vector3 point = targetPoint;
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - self.position;
		float distance = rayDirection.magnitude;
		if (distance > maxDist){
			return false;
		}
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		//float closestCollisionDistance = distance;
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(self.position, rayDirection, distance);
		foreach (RaycastHit2D r in hits){
			if (!r.collider.isTrigger){
				if (r.transform != p && r.transform != self){
					// See what wall we hit, and reset the grab reach based on that
					float dist = Vector3.Distance(r.point, self.position);
					if (dist < closestCollisionDistance && !r.collider.isTrigger){
						closestCollisionDistance = dist;
						closestCollision = r.point;
						clearPath = false;
					}

				} 
			}
		}
		return clearPath;
	}

	// Checks for a clear path to an object
	public bool ClearPathToTarget(Transform p, Transform self, float maxDist){
		// If the object isn't active, it shouldn't be visible at all
		if (!p.gameObject.activeSelf){
			return false;
		}

		Vector3 point = p.position;
		bool clearPath = true;
		RaycastHit2D[] hits;
		Vector3 rayDirection = point - self.position;
		float distance = rayDirection.magnitude;
		if (distance > maxDist){
			return false;
		}
		Vector3 closestCollision = new Vector3(0f, 0f, 0f);
		//float closestCollisionDistance = distance;
		float closestCollisionDistance = 100000f;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(self.position, rayDirection, distance);
		foreach (RaycastHit2D r in hits){
			if (!r.collider.isTrigger){
				if (r.transform != p && r.transform != self){
					// See what wall we hit, and reset the grab reach based on that
					float dist = Vector3.Distance(r.point, self.position);
					if (dist < closestCollisionDistance && !r.collider.isTrigger){
						closestCollisionDistance = dist;
						closestCollision = r.point;
						clearPath = false;
					}
					
				} 
			}
		}
		return clearPath;
	}

	// Gets the proper angle given the face direction
	public float GetAngleToObj(Vector3 fromPos, Vector3 toPos){
		// Sets player to face the cursor's position]
		float ang;
		if (transform.localScale.x < 0){
			ang = -Mathf.Atan2(toPos.y-fromPos.y, toPos.x-fromPos.x);
		} else {
			ang = Mathf.Atan2(toPos.y-fromPos.y, toPos.x-fromPos.x);
		}
		ang *= Mathf.Rad2Deg;

		if (transform.localScale.x < 0){
			ang += 180f;
		}
		
		return ang;
	}
}
