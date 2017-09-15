using UnityEngine;
using System.Collections;

/*
 * 
 * 		NOTE:
 * 		PipeParent only retains connections of the points. The WarpPipe script gets those references.
 * 
 */
public class PipeParent : MonoBehaviour {
	// Points contained within the pipe
	// Use these for progressing to the next point
	public Transform point1 = null;
	public Transform point2 = null;

	// The point that touches the next piece of pipe
	public Transform nextPipe1;
	public Transform nextPipe2; 		// If the pipe isn't an endpoint, it'll border on two pipes

	int currConnection = 1;
	// Use this for initialization
	void Awake () {
		Transform[] ts = transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in ts){
			if (t.name != "Arrow" && !t.name.Contains("Pipe")){
				if (point1 == null){
					point1 = t;
				} else {
					point2 = t;
					break;
				}
			}
		}
	}
	void SetConnection(Transform t, bool a, bool b, bool c, bool d){
		Transform connection = null;
		if (a){
			connection = t.GetComponent<PipeParent>().point1;
		} else if (b){
			connection = t.GetComponent<PipeParent>().point1;
		} else if (c){
			connection = t.GetComponent<PipeParent>().point2;
		} else if (d){
			connection = t.GetComponent<PipeParent>().point2;
		}
		if (currConnection == 1){
			//connection1 = connection;
			currConnection = 2;
		} else {
			//connection2 = connection;
		}
		//t.GetComponent<PipeParent>().conn1 = connection1;
		//t.GetComponent<PipeParent>().conn2 = connection2;
	}
	// Initialize connections
	void Start(){
		/*
		point1.GetComponent<CircleCollider2D>().enabled = true;
		point2.GetComponent<CircleCollider2D>().enabled = true;
		// Always use a PipeGroup to make connections easier
		Transform pipeGroup = transform.parent;
		Transform[] ts = pipeGroup.GetComponentsInChildren<Transform>(true);
		//foreach (Transform t in pipeGroup){
		foreach (Transform t in ts){
			if (t != transform){
				if (t.GetComponent<PipeParent>()){
					t.GetComponent<PipeParent>().point1.GetComponent<CircleCollider2D>().enabled = true;
					t.GetComponent<PipeParent>().point2.GetComponent<CircleCollider2D>().enabled = true;

					bool col1 = Physics2D.IsTouching(point1.GetComponent<CircleCollider2D>(), t.GetComponent<PipeParent>().point1.GetComponent<CircleCollider2D>());
					bool col2 = Physics2D.IsTouching(point1.GetComponent<CircleCollider2D>(), t.GetComponent<PipeParent>().point2.GetComponent<CircleCollider2D>());
					bool col3 = Physics2D.IsTouching(point2.GetComponent<CircleCollider2D>(), t.GetComponent<PipeParent>().point1.GetComponent<CircleCollider2D>());
					bool col4 = Physics2D.IsTouching(point2.GetComponent<CircleCollider2D>(), t.GetComponent<PipeParent>().point2.GetComponent<CircleCollider2D>());
					print(t.name + ", " + transform.name);

					SetConnection(t, col1, col2, col3, col4);

					t.GetComponent<PipeParent>().point1.GetComponent<CircleCollider2D>().enabled = true;
					t.GetComponent<PipeParent>().point2.GetComponent<CircleCollider2D>().enabled = true;
				}
			}
		}
		// Disables colliders on the pipes unless they're meant for entering the pipe
		if (!(point1.GetComponent<WarpPipe>() && point1.GetComponent<WarpPipe>().active)){
			point1.GetComponent<CircleCollider2D>().enabled = false;
		}
		if (!(point2.GetComponent<WarpPipe>() && point2.GetComponent<WarpPipe>().active)){
			point2.GetComponent<CircleCollider2D>().enabled = false;
		}
		*/
	}
}
