using UnityEngine;
using System.Collections;

public class Powerline : MonoBehaviour {
	// Use this for initialization
	void Start () {
		LineRenderer line = GetComponent<LineRenderer>();
		EdgeCollider2D col = GetComponent<EdgeCollider2D>();
		Vector3 endPoint = transform.Find("EndPoint").position - transform.position;
		Vector3 point1 = endPoint * .33f + new Vector3(0f, -1.5f, 0f);
		Vector3 point2 = endPoint * .66f + new Vector3(0f, -1.5f, 0f);
		line.SetPosition(1, point1);
		line.SetPosition(2, point2);
		line.SetPosition(3, endPoint);
		col.points = new Vector2[]{new Vector2(0f, 0f), point1, point2, endPoint};
		//col.points[1] = point1;
		//col.points[2] = point2;
		//col.points[3] = endPoint;
	}
}
