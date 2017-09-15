using UnityEngine;
using System.Collections;

public class SetCameraZ : MonoBehaviour {
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

	GameObject hand;

	// Currently focusing (use a time in case of async)
	float focusedTimer = 0f;
	float maxFocusedTimer = .1f;

	public float zoomChange = -2f;
	float startZ = 0f;

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
		startZ = ct.transform.position.z;

		// Disables rendering of the object
		GetComponent<SpriteRenderer>().enabled = !disableRendering;

		hand = GameObject.Find("TheHand");
	}

	// Update is called once per frame
	void Update () {
		focusedTimer -= Time.deltaTime;
		// Check if the point is on-screen
		if (focusedTimer > 0f){
			ct.SlowZoom(zoomChange+startZ, 2f*Time.deltaTime);
		}
	}

	// Check for collisions with the hand
	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player"){
			focusedTimer = maxFocusedTimer;
		}
	}


}
