using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;

public class MapCursor : MonoBehaviour {
	Transform mapIcon;						// The player icon on the map
	Transform cameraDistancePoint; 			// Roughly the X & Z position the camera should maintain in relation to the player
	Vector3 cameraStartPoint;
	Vector3 startPos;
	Vector3 destPos;
	Vector3 middlePos1;
	Vector3 middlePos2;

	float iconMoveTimer = 0f;
	const float maxIconMoveTimer = .25f;
	bool iconMoving = false;

	float cameraMoveTimer = 0f;
	float maxCameraMoveTimer = .25f;
	bool cameraMoving = false;

	string selectedLevel = "";
	string displayedLevelText = "";

	Transform textSelectionPanel;

	// For hovering over map points
	ArrayList mapPoints = new ArrayList();
	GameObject hoveredPoint = null;
	Vector3 hoveredPointSize = new Vector3(.205f, .205f, .205f);
	Vector3 maxHoveredPointSize = new Vector3(.5f, .5f, .5f);

	// For transitioning out of the map
	Transform levelTransitionObject;
	ArrayList squareSprites = new ArrayList();
	Transform levelTransitionSprite;
	Vector3 transitionScaleRate = new Vector3(2f, 2f, 2f);
	bool transitioningOut = false;

	// Use this for initialization
	void Start () {
		mapIcon = GameObject.Find("MapIcon").transform;
		cameraDistancePoint = mapIcon.Find("CameraDistancePoint");

		// Text display for going to a mission
		textSelectionPanel = Camera.main.transform.Find("TextSelectionPanel");
		textSelectionPanel.gameObject.SetActive(false);

		// Map point list
		GameObject[] g = GameObject.FindGameObjectsWithTag("Wall");
		foreach (GameObject i in g){
			mapPoints.Add(i);
		}

		// Tiles for the level transition
		levelTransitionObject = Camera.main.transform.Find("LevelTransition");
		squareSprites.Add(Camera.main.transform.Find("LevelTransition").Find("squaresprite"));
		squareSprites.Add(Camera.main.transform.Find("LevelTransition").Find("squaresprite1"));
		squareSprites.Add(Camera.main.transform.Find("LevelTransition").Find("squaresprite2"));
		squareSprites.Add(Camera.main.transform.Find("LevelTransition").Find("squaresprite3"));
		levelTransitionSprite = Camera.main.transform.Find("LevelTransition").Find("MainSprite");
		levelTransitionObject.gameObject.SetActive(false);

		// Erases the checkpoint file
		CheckpointData.DeleteCheckpointData();
	}

	// Shows the illum icon and fades out
	void TransitionOutIcon(){
		//levelTransitionObject.eulerAngles += new Vector3(0f, 0f, 360f*Time.deltaTime);
		levelTransitionSprite.localScale -= Time.deltaTime * transitionScaleRate;
		foreach (Transform t in squareSprites){
			t.localScale += Time.deltaTime * transitionScaleRate;
		}
		// Goes to the next level once the transition completes
		if (levelTransitionSprite.localScale.x <= -.25f){
			LoadSelectedLevel();
		}
	}

	// Update is called once per frame
	void Update () {
		Vector3 newMousePos = Input.mousePosition;
		Vector3 pos = newMousePos;
		pos.z += 5f;
		transform.position = Camera.main.ScreenToWorldPoint(pos);
		// Have the forward vector go straight in the direction from the camera to the cursor so we can aim accurately
		transform.forward = (transform.position - Camera.main.transform.position).normalized;

		// Moves to position
		if (iconMoving){
			iconMoveTimer += Time.deltaTime;
			mapIcon.position = CubeBezier3(startPos, middlePos1, middlePos2, destPos, iconMoveTimer/maxIconMoveTimer);
			if (iconMoveTimer >= maxIconMoveTimer){
				iconMoving = false;
				cameraMoving = true;
				cameraMoveTimer = 0f;
				mapIcon.position = destPos;
				cameraStartPoint = Camera.main.transform.position;
			}
		}

		// Moves the camera to be well-placed in relation to the icon
		if (cameraMoving){
			cameraMoveTimer += Time.deltaTime;
			if (cameraMoveTimer >= maxCameraMoveTimer){
				// Upon reaching the map point, show a menu allowing the player to start the level
				string text = "Invade "+displayedLevelText+"?";
				cameraMoving = false;
				textSelectionPanel.gameObject.SetActive(true);
				textSelectionPanel.Find("LevelNameText1").GetComponent<Text>().text = text;
				textSelectionPanel.Find("LevelNameText2").GetComponent<Text>().text = text;
			}
			Vector3 destCameraPos = cameraDistancePoint.position;
			destCameraPos.y = Camera.main.transform.position.y;
			Camera.main.transform.position = Vector3.Lerp(cameraStartPoint, destCameraPos, cameraMoveTimer / maxCameraMoveTimer);
		// Allows gradual panning of the camera
		} else {
			float camHeight = Screen.height;
			float camWidth = Screen.width;
			Vector3 mousePos = Input.mousePosition;
			if (mousePos.x < camWidth/5f){
				float xSpeed = (camWidth/5f) - Input.mousePosition.x;
				xSpeed *= -.0002f;
				xSpeed = Mathf.Clamp(xSpeed, -7f*Time.deltaTime, 0f);
				Camera.main.transform.position += new Vector3(xSpeed, 0f, 0f);
			} else if (mousePos.x > 4*camWidth/5f){
				float xSpeed = 4*(camWidth/5f) - Input.mousePosition.x;
				xSpeed *= -.0002f;
				xSpeed = Mathf.Clamp(xSpeed, 0f, 7f*Time.deltaTime);
				Camera.main.transform.position += new Vector3(xSpeed, 0f, 0f);
			}
			if (mousePos.y < camHeight/6f){
				float ySpeed = (camHeight/6f) - Input.mousePosition.y;
				ySpeed *= -.00025f;
				ySpeed = Mathf.Clamp(ySpeed, -7f*Time.deltaTime, 0f);
				Camera.main.transform.position += new Vector3(0, 0f, ySpeed);
			} else if (mousePos.y > 5*camHeight/6f){
				float ySpeed = 5*(camHeight/6f) - Input.mousePosition.y;
				ySpeed *= -.00025f;
				ySpeed = Mathf.Clamp(ySpeed, 0f, 7f*Time.deltaTime);
				Camera.main.transform.position += new Vector3(0, 0f, ySpeed);
			}
		}

		// Makes the circles of hovered map points scale
		foreach (GameObject g in mapPoints){
			Transform circle = g.transform.Find("FullCircle");
			if (g == hoveredPoint){
				circle.localScale = Vector3.MoveTowards(circle.transform.localScale, maxHoveredPointSize, 6f*Time.deltaTime);
			} else {
				circle.localScale = Vector3.MoveTowards(circle.transform.localScale, hoveredPointSize, 8f*Time.deltaTime);
			}
		}

		// Transitions to the next level
		if (transitioningOut){
			TransitionOutIcon();
		}
	}

	// Loads the selected level
	public void LoadSelectedLevel(){
		if (selectedLevel != ""){
			Application.LoadLevel(selectedLevel);
		}
	}

	// Begins transition to selected level
	public void ActivateLevelTransition(){
		levelTransitionObject.gameObject.SetActive(true);
		textSelectionPanel.gameObject.SetActive(false);
		transitioningOut = true;
	}

	// Turns off the selection menu
	public void DisableSelectionMenu(){
		textSelectionPanel.gameObject.SetActive(false);	
	}

	// Detects what level we're hovering over
	void OnTriggerStay(Collider col){
		// Upon clicking a map point, set the player to move to it
		if (Input.GetMouseButtonDown(0) && !iconMoving && !transitioningOut){
			if (col.gameObject.GetComponent<MapPoint>()){
				iconMoveTimer = 0f;
				iconMoving = true;
				startPos = mapIcon.position;
				destPos = col.gameObject.transform.position;
				middlePos1 = startPos + new Vector3(0f, 3f, 0f);
				middlePos2 = destPos + new Vector3(0f, 2f, 0f);

				selectedLevel = col.gameObject.GetComponent<MapPoint>().levelName;
				displayedLevelText = col.gameObject.GetComponent<MapPoint>().displayedName;
			}
		// Hovered-over points are marked
		} else {
			hoveredPoint = col.gameObject;
		}
	}
	void OnTriggerExit(Collider col){
		hoveredPoint = null;
	}

	public static Vector3 CubeBezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float r = 1f - t;
		float f0 = r * r * r;
		float f1 = r * r * t * 3;
		float f2 = r * t * t * 3;
		float f3 = t * t * t;
		return f0*p0 + f1*p1 + f2*p2 + f3*p3;
	}
}
