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
public class RepeatingGround : MonoBehaviour {
	Transform leftBoundary;
	Transform rightBoundary;
	Camera camera;
	public GameObject rightCopy = null;
	public GameObject leftCopy = null;
	public GameObject parent = null;
	public int parentDirection = 0;
	public Vector3 origVelocity = new Vector3(-.6f*60f, 0f, 0f);
	Vector3 velocity = new Vector3(0f, 0f, 0f);

	// Uses a linked list for scrolling the background
	ArrayList linkedList = new ArrayList();
	public float maxXDistFromCam = 70f;
	
	public string Prefab;
	// Use this for initialization
	void Start () {
		//leftBoundary = transform.Find("LeftBoundary");
		//rightBoundary = transform.Find("RightBoundary");
		camera = Camera.main;

		velocity = origVelocity;

		linkedList.Add(transform.Find("Repeater1").gameObject);
		linkedList.Add(transform.Find("Repeater2").gameObject);
		linkedList.Add(transform.Find("Repeater3").gameObject);
		if (transform.Find("Repeater4")){
			linkedList.Add(transform.Find("Repeater4").gameObject);
		}
	}
	public void OldMethodSpawning(){
		// Spawns a new piece of repeating ground to the right
		if (rightCopy == null && camera.WorldToScreenPoint(rightBoundary.position).x < Screen.width*1.5f){
			if (parent == null || parentDirection != 1){
				// Recursive call to get the left object
				bool movedLeftChild = false;
				if (parent != null){
					if (parent.GetComponent<RepeatingGround>().leftCopy){
						movedLeftChild = true;
						GameObject r = parent.GetComponent<RepeatingGround>().leftCopy;
						r.GetComponent<RepeatingGround>().parent = gameObject;
						r.GetComponent<RepeatingGround>().parentDirection = -1;
						leftCopy = gameObject;
						r.transform.position = transform.position + new Vector3(rightBoundary.position.x - leftBoundary.position.x, 0f, 0f);
					}
				}
				if (!movedLeftChild) {
					rightCopy = (GameObject)Instantiate(Resources.Load(Prefab), transform.position + new Vector3(rightBoundary.position.x - leftBoundary.position.x, 0f, 0f), new Quaternion());
					rightCopy.GetComponent<RepeatingGround>().parent = gameObject;
					rightCopy.GetComponent<RepeatingGround>().parentDirection = -1;
					//rightCopy.GetComponent<RepeatingGround>().velocity = origVelocity;
					rightCopy.GetComponent<RepeatingGround>().SetVelocity(origVelocity);
				}
			}
		}
		// Spawns a piece to the left
		if (leftCopy == null && camera.WorldToScreenPoint(leftBoundary.position).x > 0){
			if (parent == null || parentDirection != -1){
				leftCopy = (GameObject)Instantiate(Resources.Load(Prefab), transform.position - new Vector3(rightBoundary.position.x - leftBoundary.position.x, 0f, 0f), new Quaternion());
				leftCopy.GetComponent<RepeatingGround>().parent = gameObject;
				leftCopy.GetComponent<RepeatingGround>().parentDirection = 1;
				//leftCopy.GetComponent<RepeatingGround>().velocity = origVelocity;
				leftCopy.GetComponent<RepeatingGround>().SetVelocity(origVelocity);
				// Sets left child to be parent of the root node
				if (parent == null){
					parent = leftCopy;
				}
			}
		}
	}
	// Just shifts objects right once they're off screen
	public void NewMethodSpawning(){
		GameObject leftmostObj = (GameObject)linkedList[0];
		if (leftmostObj.GetComponent<RepeatingGroundElement>().rightBoundary.position.x < camera.transform.position.x - maxXDistFromCam){
			GameObject rightmostObj = (GameObject)linkedList[linkedList.Count-1];
			leftmostObj.transform.position = rightmostObj.transform.position + new Vector3(rightmostObj.GetComponent<RepeatingGroundElement>().Width(), 0f, 0f);

			// Creates a new array instead of shifting
			/*
			ArrayList tempList = new ArrayList();
			for (int i = 1; i < linkedList.Count; i++){
				tempList.Add(linkedList[i]);
			}
			tempList.Add(leftmostObj);
			linkedList = tempList;
			*/
			// Shifts array down
			for (int i = 0; i < linkedList.Count-1; i++){
				linkedList[i] = linkedList[i+1];
			}
			linkedList[linkedList.Count-1] = leftmostObj;

			//linkedList.Add(leftmostObj);
			//if (linkedList.Count > 3){
				//linkedList.RemoveAt(0);
			//} else {
			//	print ("tried to remove too many");
			//}
		}
	}
	// Update is called once per frame
	void Update () {
		//OldMethodSpawning();
		NewMethodSpawning();
		transform.Translate(velocity*Time.deltaTime);
	}
	public void SetVelocity(Vector3 vel){
		origVelocity = vel;
	}
	public void SetActualVelocity(Vector3 vel){
		velocity = vel;
	}
	public Vector3 GetVelocity(){
		return velocity;
	}
}
