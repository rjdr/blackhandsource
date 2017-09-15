using UnityEngine;
using System.Collections;

public class MissionText : MonoBehaviour {
	float moveTogetherTimer = 0f;
	float maxMoveTogetherTimer = .25f;

	Transform BlackUp;
	Transform BlackDown;
	Transform Container;
	Transform BarUp;
	Transform BarDown;
	Transform TextBox;
	Transform Description;

	public string text = "DO THIS";

	float timer = 0f;
	float startBarTimer = 1f;
	float endAllTimer = 4f;
	float maxEndAllTimer = 5f;
	bool started = false;
	Vector3 destBarUp;
	Vector3 destBarDown;
	Vector3 destDescription;

	// Use this for initialization
	void Start () {
		BlackUp = transform.Find("BlackUp");
		BlackDown = transform.Find("BlackDown");
		Container = transform.Find("Container");
		BarUp = Container.Find("BarUp");
		BarDown = Container.Find("BarDown");
		TextBox = Container.Find("TextBox");
		Description = Container.Find("Description");

		destBarUp = BarUp.GetComponent<RectTransform>().localPosition;
		destBarDown = BarDown.GetComponent<RectTransform>().localPosition;
		destDescription = Description.GetComponent<RectTransform>().localPosition;
		BarUp.GetComponent<RectTransform>().localPosition += new Vector3(-2000f, 0f, 0f);
		BarDown.GetComponent<RectTransform>().localPosition += new Vector3(2000f, 0f, 0f);
		Description.GetComponent<RectTransform>().localPosition += new Vector3(0f, -1000f, 0f);
		Description.GetComponent<UnityEngine.UI.Text>().text = text;
	}

	// Slides the bars to the center of the screen
	// They'll overlap
	// Then they spread apart to reveal the text "MISSION"
	// Then this changes to show the mission description
	void Slide(){
		timer += Time.unscaledDeltaTime;
		BlackUp.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(BlackUp.GetComponent<RectTransform>().localPosition, new Vector3(0f, 1440f, 0f), 1200f*Time.unscaledDeltaTime);
		BlackDown.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(BlackDown.GetComponent<RectTransform>().localPosition, new Vector3(0f, -1440f, 0f), 1200f*Time.unscaledDeltaTime);

		if (timer > startBarTimer && timer < endAllTimer){
			BarUp.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(BarUp.GetComponent<RectTransform>().localPosition, destBarUp, 6000f*Time.unscaledDeltaTime);
			BarDown.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(BarDown.GetComponent<RectTransform>().localPosition, destBarDown, 6000f*Time.unscaledDeltaTime);
			Description.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(Description.GetComponent<RectTransform>().localPosition, destDescription, 3000f*Time.unscaledDeltaTime);
		// Slides off screen then destroys the object once done
		} else if (timer > endAllTimer){
			Container.GetComponent<RectTransform>().localPosition += new Vector3(0f, 2000f, 0f)*Time.deltaTime;
			if (timer > maxEndAllTimer){
				Destroy(gameObject, 0f);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (started){
			Slide();
		}
		started = true;
	}
}
