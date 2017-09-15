using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {
	GameObject bullet;
	Transform firePoint;
	Transform targetReticule;
	ArrayList targetChildren = new ArrayList();

	// Initial pre-aim waiting period
	public float initialWaitPeriod = 2f;
	bool passedInitialWait = false;

	// Timer for aiming the target reticule
	float aimTimer = 0f;
	float maxAimTimer = 5f;
	float holdAimTimer = 2f;
	// Speed of the target reticule's movement
	float aimSpeed = 4f;

	bool aiming = false;

	// Hand to target
	Transform hand;

	float bulletVelocity = 24f;

	// Use this for initialization
	void Start () {
		firePoint = transform.Find("FirePoint");
		targetReticule = transform.Find("TargetReticule");
		// Decouples the target reticule to more easily move it around absolutely
		//targetReticule.parent = null;
		targetReticule.eulerAngles = new Vector3(0f, 0f, 0f);

		bullet = (GameObject)Resources.Load("TankBullet");

		hand = GameObject.Find("TheHand").transform;
		// Puts reticule on same plane as the hand
		Vector3 tempPosition = targetReticule.transform.position;
		tempPosition.z = hand.position.z;
		targetReticule.transform.position = tempPosition;

		foreach (Transform t in targetReticule.transform){
			targetChildren.Add(t.Find("Child"));
		}
	}

	// Activates the fire/aim mode
	public void ActivateAim(){
		aiming = true;
	}

	// Fires the bullet
	void FireBullet(){
		aimTimer = 0f;

		GameObject tempBullet = (GameObject)Instantiate(bullet);
		tempBullet.transform.position = firePoint.position;

		// Sets the bullet to go towards the target reticule
		tempBullet.transform.right = - (targetReticule.position - transform.position).normalized;

		tempBullet.gameObject.GetComponent<Rigidbody>().velocity = -tempBullet.transform.right * bulletVelocity;
	}
	
	// Update is called once per frame
	void Update () {
		TakeAim();
	}
	// Takes aim at the player
	void TakeAim(){
		// Only aim when in aiming mode
		if (!aiming){
			return;
		}

		// Need to wait a bit before it can attack (this keeps tank attacks from overlapping
		if (!passedInitialWait){
			aimTimer += Time.deltaTime;
			if (aimTimer > initialWaitPeriod){
				passedInitialWait = true;
				aimTimer = 0f;
			}
			return;
		}

		// Shifts the reticule positions
		for (int i = 0; i < targetChildren.Count; i++){
			Transform t = (Transform)targetChildren[i];
			t.localPosition = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(1f, -1f, 0f), (aimTimer-maxAimTimer/2)/(maxAimTimer/2));
			t.GetComponent<SpriteRenderer>().color = new Color(1f, 1f - Mathf.Clamp(aimTimer/maxAimTimer, 0f, 1f), 1f - Mathf.Clamp(aimTimer/maxAimTimer, 0f, 1f), 1f);
		}

		aimTimer += Time.deltaTime;
		if (aimTimer > maxAimTimer){
			// Hold position for a while...
			// Once you've reached the max time + max hold time, fire
			if (aimTimer >= maxAimTimer + holdAimTimer){
				FireBullet();
			}
		} else {
			targetReticule.transform.position = Vector3.MoveTowards(targetReticule.transform.position, hand.position, aimSpeed * Time.deltaTime);
		}
	}
}
