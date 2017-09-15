using UnityEngine;
using System.Collections;

public class AutofitCamera : MonoBehaviour {
	float aspect;
	float fov;
	float camHeight;
	float camWidth;
	float depthDelta = 1f;
	float origCameraDepth = 0f;
	float errorAllowance = .2f; 	// How different we can be from the desired width
	float zoomVelocity = 1f;
	public Vector3 velocity = new Vector3(.75f, .75f, .75f);

	GameObject center;
	CameraTarget ct;
	
	float width;

	public bool disableRendering = true;

	// If this is set, the camera will scroll to focus on this object whether or not it's onscreen
	public bool manualOverrideFocus = false;

	GameObject hand;

	// Currently focusing (use a time in case of async)
	float focusedTimer = 0f;
	float maxFocusedTimer = .1f;

	// Camera matches the angle of the object
	public bool matchAngle = false;
	public float angleSpeed = 1.5f;

	// If touched once, we don't need to make contact with the player again
	public bool touchOnceActivate = false;

	// Use this for initialization
	void Start () {
		aspect = Camera.main.aspect;
		fov = Camera.main.fieldOfView;

		center = transform.Find("Center").gameObject;

		// Width of the object
		width = transform.Find("Right").transform.position.x - transform.Find("Left").transform.position.x;

		// Original depth of the camera
		origCameraDepth = Camera.main.transform.position.z;

		// Grab the camera
		ct = Camera.main.GetComponent<CameraTarget>();

		// Disables rendering of the object
		GetComponent<SpriteRenderer>().enabled = !disableRendering;

		hand = GameObject.Find("TheHand");
	}

	// Update is called once per frame
	void Update () {
		// Check if the point is on-screen
		if (focusedTimer >= 0f || manualOverrideFocus){
			focusedTimer -= Time.deltaTime;
			// How far the camera is from the fitting object
			depthDelta = (Camera.main.transform.position.z - transform.position.z);
			camHeight = 2 * Mathf.Tan(0.5f * fov * Mathf.Deg2Rad) * depthDelta;
			camWidth = camHeight * aspect;

			// To get the desired depth delta, just work backwards
			float destDelta = width / aspect / (2 * Mathf.Tan(0.5f * fov * Mathf.Deg2Rad));
			Vector3 tempPosition = Camera.main.transform.position;
			tempPosition.z = transform.position.z - destDelta;

			// Slide the camera's x & y to match the object's x & y
			tempPosition.x = transform.position.x;
			tempPosition.y = transform.position.y;

			ct.SetAutofit(tempPosition, velocity);

			//Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, tempPosition, 1f*Time.deltaTime);

			// Moves the camera to match the angle of the object
			if (matchAngle){
				Camera.main.transform.eulerAngles = Vector3.MoveTowards(Camera.main.transform.eulerAngles, transform.eulerAngles, angleSpeed*Time.deltaTime);
			}
		}
	}

	// Check for collisions with the hand
	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player"){
			focusedTimer = maxFocusedTimer;
			if (touchOnceActivate){
				focusedTimer = 10000f;
			}
		}
	}


}
