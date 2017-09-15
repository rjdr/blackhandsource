/*
using UnityEngine;
using System.Collections;


// The purpose of this class is to make sure ground repeats when it goes off screen
public class RepeatingGround : MonoBehaviour {
	Transform leftBoundary;
	Transform rightBoundary;
	Camera camera;
	GameObject rightCopy = null;
	GameObject leftCopy = null;
	public GameObject parent = null;
	public int parentDirection = 0;
	public Vector3 velocity = new Vector3(-.6f, 0f, 0f);

	public string Prefab;
	// Use this for initialization
	void Start () {
		leftBoundary = transform.Find("LeftBoundary");
		rightBoundary = transform.Find("RightBoundary");
		camera = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Spawns a new piece of repeating ground
		if (rightCopy == null && camera.WorldToScreenPoint(rightBoundary.position).x < Screen.width*1.5f){
			if (parent == null || parentDirection != 1){
				rightCopy = (GameObject)Instantiate(Resources.Load(Prefab), transform.position + new Vector3(rightBoundary.position.x - leftBoundary.position.x, 0f, 0f), new Quaternion());
				rightCopy.GetComponent<RepeatingGround>().parent = gameObject;
				rightCopy.GetComponent<RepeatingGround>().parentDirection = -1;
				rightCopy.GetComponent<RepeatingGround>().velocity = velocity;
			}
		}
		if (leftCopy == null && camera.WorldToScreenPoint(leftBoundary.position).x > 0){
			if (parent == null || parentDirection != -1){
				leftCopy = (GameObject)Instantiate(Resources.Load(Prefab), transform.position - new Vector3(rightBoundary.position.x - leftBoundary.position.x, 0f, 0f), new Quaternion());
				leftCopy.GetComponent<RepeatingGround>().parent = gameObject;
				leftCopy.GetComponent<RepeatingGround>().parentDirection = 1;
				leftCopy.GetComponent<RepeatingGround>().velocity = velocity;
			}
		}
		transform.Translate(velocity);
	}
}
*/

using UnityEngine;
using System.Collections;


// The purpose of this class is to make sure ground repeats when it goes off screen
public class RepeatingGroundElement : MonoBehaviour {
	public Transform leftBoundary;
	public Transform rightBoundary;
	Camera camera;
	public GameObject rightCopy = null;
	public GameObject leftCopy = null;
	public GameObject parent = null;
	public int parentDirection = 0;
	public Vector3 origVelocity = new Vector3(-.6f*60f, 0f, 0f);
	Vector3 velocity = new Vector3(0f, 0f, 0f);
	
	// Uses a linked list for scrolling the background
	ArrayList linkedList = new ArrayList();
	float maxXDistFromCam = 70f;
	float width = 0f;
	
	public string Prefab;
	// Use this for initialization
	void Start () {
		leftBoundary = transform.Find("LeftBoundary");
		rightBoundary = transform.Find("RightBoundary");
		width = Mathf.Abs(rightBoundary.position.x - leftBoundary.position.x);
		camera = Camera.main;
		
		velocity = origVelocity;
	}

	public float Width(){
		return width;
	}

	public void SetVelocity(Vector3 vel){
		origVelocity = vel;
	}
}
