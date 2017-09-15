using UnityEngine;
using System.Collections;

/*
 * NOTE:
 * The start and destination point are to be marked by the 'connection' variable
 * Intermediate points are instead marked within the 'PipeParent' object
 */

public class WarpPipe : MonoBehaviour {
	public bool active = false;
	public WarpPipe connection;
	float waitToWarpTimer = 0f;
	float maxWaitToWarpTimer = .75f;
	float exitVelocity = 15f;

	Transform arrowPoint;
	Vector3 arrowPointPos;
	Vector3 maxArrowPointPos;
	float arrowMoveTimer = 0f;
	float maxArrowMoveTimer = 2f;
	float arrowMoveSpeed = 4f;

	// Points that the pipe connects to
	public Transform nextPoint1;
	public Transform nextPoint2;

	ArrayList nodes;
	float nextNodeTimer = 0f;
	float maxNextNodeTimer = .15f;

	GameObject collidedObject = null;

	// Use this for initialization
	void Start () {
		Transform arrow = connection.transform.parent.Find("Arrow");
		if (arrow != null){
			arrow.gameObject.SetActive(true);
			arrow.position = connection.transform.position;
			//Vector3 tempScale = arrow.parent.localScale;
			//tempScale.x = 1f/tempScale.x;
			//tempScale.y = 1f/tempScale.y;
			//tempScale.z = 1f/tempScale.z;
			//arrow.localScale = tempScale;
			//arrow.right = transform.position - arrow.position;
			arrow.SetParent(null);
			arrow.eulerAngles = new Vector3(0f, 0f, 0f);
			arrow.up = -connection.transform.forward;
			arrow.eulerAngles += new Vector3(0f, 0f, 90f);
			arrow.transform.position += connection.transform.forward * 1f;
			arrow.localScale = new Vector3(.4f, .4f, .4f);

			arrowPoint = arrow;
			arrowPointPos = arrow.position;
			maxArrowPointPos = arrowPointPos - arrow.right * .25f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		waitToWarpTimer -= Time.deltaTime;

		if (arrowPoint != null){
			arrowPoint.position = Vector3.Lerp(arrowPointPos, maxArrowPointPos, arrowMoveTimer/maxArrowMoveTimer);
			arrowMoveTimer += Time.deltaTime*arrowMoveSpeed;
			if (arrowMoveTimer >= maxArrowMoveTimer){
				arrowMoveSpeed = -Mathf.Abs(arrowMoveSpeed);
			} else if (arrowMoveTimer <= 0f){
				arrowMoveSpeed = Mathf.Abs(arrowMoveSpeed);	
			}
		}

		// An object is currently moving through the pipes
		if (collidedObject != null){
			if (nodes.Count > 0){
			// Move from node to node
				nextNodeTimer += Time.deltaTime;
				if (nextNodeTimer >= maxNextNodeTimer){
					nextNodeTimer = 0f;
					collidedObject.transform.position = ((Transform)nodes[0]).position;
					nodes.RemoveAt(0);
				}
			} else {
			// Move to the end point
				LaunchObject();
			}
		}
	}

	// Gets the chain of points we're going to go through
	public void GetConnectionChain(){
		int maxNumLoops = 20; 	// If somehow we get caught in an infinite loop, break after this many points
		ArrayList points = new ArrayList();
		Transform current = transform.parent;
		Transform last = current;
		// Loops through the connections, moving to the next of whichever has not yet been visited
		while (true){
			maxNumLoops--;
			if (maxNumLoops <= 0){
				break;
			}
			points.Add(current);
			Transform p1 = current.GetComponent<PipeParent>().nextPipe1;
			Transform p2 = current.GetComponent<PipeParent>().nextPipe2;
			if (p1 != null && !points.Contains(p1)){
				last = current;
				current = p1;
				continue;
			}
			if (p2 != null && !points.Contains(p2)){
				last = current;
				current = p2;
				continue;
			}
			break;
		}
		foreach (Transform t in points){
			print(t.name);
		}
		nodes = points;
	}

	// Sets the object to not accept a warp instantly
	public void SetDelay(){
		waitToWarpTimer = maxWaitToWarpTimer;
	}

	// Launches object out of the other end of the pipe
	void LaunchObject(){
		SetDelay();
		connection.SetDelay();
		collidedObject.SetActive(true);
		collidedObject.transform.position = connection.transform.position;
		Vector3 tempVelocity = connection.transform.forward * exitVelocity;
		collidedObject.GetComponent<Rigidbody2D>().velocity = tempVelocity;
		collidedObject = null;
	}

	// The hand can warp through pipes while its in its goo form
	void OnTriggerStay2D(Collider2D col){
		if (waitToWarpTimer <= 0f && col.gameObject.name == "TheHand"){
			collidedObject = col.gameObject;
			GetConnectionChain();
			col.gameObject.SetActive(false);

			//LaunchObject();
		}
	}
}
