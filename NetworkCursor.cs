using UnityEngine;
using System.Collections;

public class NetworkCursor : MonoBehaviour {
	Transform hand;
	SpriteRenderer fuck; 	// Name overlapped with fucking EVERYTHING and autocomplete kept fucking me in the ass
	private Color allClear = new Color(1f, 1f, 1f, 1f);
	private Color blocked = new Color(1f, 1f, 1f, .2f);
	private GameObject cloud;

	float cursorSpeed = .25f;

	float zOffset = 0f;
	Vector3 lastCameraPos = new Vector3(0f, 0f, 0f);
	Vector3 lastMousePos = new Vector3(0f, 0f, 0f);
	float incr = 0f;

	float accelRate = .5f;

	BoxCollider2D bc;

	bool RELATIVE = true;
	bool ALTERNATE = false;
	bool positioningMode;

	// For locking onto objects
	bool lockedOn = false;
	GameObject lockedTarget = null;
	float lockEscapeVelocity = .1f; 		// If we're locked on, moving the cursor at this velocity will release it

	bool ready = false;

	// Use this for initialization
	public void Init(GameObject curr) {
		hand = curr.transform;
		fuck = GetComponent<SpriteRenderer>();
		cloud = transform.Find("Cloud").gameObject;

		Vector3 pos = Input.mousePosition;

		//transform.position = new Vector3(7f, 2f, transform.position.z);
		//UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		//Time.timeScale = .1f;

		zOffset = -Camera.main.transform.position.z + hand.transform.position.z;
		pos.z = zOffset;
		transform.position = Camera.main.ScreenToWorldPoint(pos);

		lastCameraPos = Camera.main.transform.position;
		lastMousePos = Input.mousePosition;

		positioningMode = RELATIVE;
		if (positioningMode == ALTERNATE){
			UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		}

		bc = GetComponent<BoxCollider2D>();

		ready = true;
	}

	//void OnGUI () {
	void Update(){
		if (!ready){
			return;
		}
		//RelativeCursorPositioning();
		//AbsoluteCursorPositioning();
		//RelativeCursorPositioningOld();
		if (positioningMode == RELATIVE){
			RelativeCursorPositioningOld();
		} else if (positioningMode == ALTERNATE){
			AlternateCursorPositioning();
		}


		Vector3 ang = new Vector3(0f, 0f, Mathf.Atan2(transform.position.y - hand.position.y, transform.position.x - hand.position.x)*Mathf.Rad2Deg + 90f + 180f);
		transform.eulerAngles = ang;

		// Checks if the cursor is blocked by an object and you can't possess it
		bool cursorBlocked = false;
		RaycastHit2D[] hits;
		Vector3 rayDirection = transform.position - hand.transform.position;
		float distance = rayDirection.magnitude;
		rayDirection.Normalize();
		hits = Physics2D.RaycastAll(hand.transform.position, rayDirection, distance);
		foreach (RaycastHit2D hit in hits){
			if (!hit.collider.isTrigger && hit.collider.transform != hand && hit.collider.transform.parent != hand){
				cursorBlocked = true;
			}
		}
		if (cursorBlocked){
			fuck.color = blocked;
			cloud.SetActive(true);
		} else {
			fuck.color = allClear;
			cloud.SetActive(false);
		}
	}

	// Binds the cursor to an object
	public void LockOn(GameObject o){
		lockedOn = true;
		lockedTarget = o;
	}

	// Releases cursor from object
	public void ReleaseLockOn(){
		if (lockedTarget != null){
			bool outside = !(lockedTarget.GetComponent<BoxCollider2D>().bounds.Intersects(bc.bounds));
			Vector2 speed = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			if (outside && speed.magnitude > lockEscapeVelocity){
				ForceReleaseLockOn();
			}
		}
	}
	// Releases lock on, no exceptions
	public void ForceReleaseLockOn(){
		lockedOn = false;
		lockedTarget = null;
	}

	void AlternateCursorPositioning(){
		Vector3 tempPos = transform.position;
		tempPos.x += Input.GetAxis("Mouse X") * accelRate;
		tempPos.y += Input.GetAxis("Mouse Y") * accelRate;
		transform.position = tempPos;
	}

	void RelativeCursorPositioning(){
		incr += .05f;
		Vector3 newMousePos = Input.mousePosition;
		Vector3 mouseVelocity = newMousePos - lastMousePos;
		Vector3 pos = lastMousePos + mouseVelocity;
		pos.z = zOffset;
		Vector3 cameraVelocity = Camera.main.transform.position - lastCameraPos;
		//pos.x -= cameraVelocity.x;
		//pos.y -= cameraVelocity.y;
		pos = Camera.main.ScreenToWorldPoint(pos);
		pos.x = cameraVelocity.x + incr;
		pos.y = cameraVelocity.y;
		transform.position = pos;

		//lastMousePos = newMousePos;
		lastMousePos = lastMousePos + mouseVelocity;
		lastCameraPos = Camera.main.transform.position;
	}
	void RelativeCursorPositioningOld(){
		Vector3 newMousePos = Input.mousePosition;
		Vector3 pos = newMousePos;
		pos.z = zOffset;
		transform.position = Camera.main.ScreenToWorldPoint(pos);
	}

	void AbsoluteCursorPositioning(){
		Vector3 pos = transform.position;
		float xV = Input.GetAxis("Mouse X");
		float yV = Input.GetAxis("Mouse Y");
		Vector2 speed = new Vector2(xV, yV);
		float mag = speed.magnitude;
		speed = speed.normalized * mag;
		pos.x += speed.x * cursorSpeed;
		pos.y += speed.y * cursorSpeed;
		transform.position = pos;
	}
}
