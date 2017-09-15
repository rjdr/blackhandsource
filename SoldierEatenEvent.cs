using UnityEngine;
using System.Collections;

public class SoldierEatenEvent : MonoBehaviour {
	// Handles the event when the soldier is eaten
	// Use this for initialization
	Animator anim;
	GameObject hand;

	GameObject autoZoomCameraFirst;
	GameObject autoZoomCameraSecond;

	// Animation loops to go through before activating the scene
	int animationLoops = 0;
	int maxAnimationLoops = 5;
	bool finishedAnimation = false;
	bool startedAnimation = false;
	bool finishedEvent = false;

	// local grab beam is only enabled after we learn to eat
	GameObject grabBeam;

	void Start () {
		anim = GetComponent<Animator>();
		anim.enabled = false;

		hand = GameObject.Find("TheHand");
		autoZoomCameraFirst = GameObject.Find("AutofitCameraObjectFirst");
		autoZoomCameraSecond = GameObject.Find("AutofitCameraObjectSecond");

		autoZoomCameraSecond.SetActive(false);
		autoZoomCameraFirst.SetActive(false);

		grabBeam = transform.Find("GrabBeam").gameObject;
		grabBeam.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		// Freezes the hand during the duration of the event
		if (startedAnimation && !finishedEvent){
			hand.GetComponent<HandController>().Freeze(.2f);
		}
	}

	// Updates the camera after N animation loops
	public void ActivateSecondCamera(){
		animationLoops++;
		if (animationLoops >= maxAnimationLoops && !finishedAnimation){
			autoZoomCameraSecond.SetActive(true);
			autoZoomCameraFirst.SetActive(false);
			finishedAnimation = true;
			grabBeam.SetActive(true);
		}
	}

	// disables cameras
	public void DisableCameras(){
		autoZoomCameraFirst.SetActive(false);
		autoZoomCameraSecond.SetActive(false);

		// Finishes the event
		finishedEvent = true;
	}

	// Levels up player for the first time so it can eat
	public void PlayerLearnsToEat(){
		hand.GetComponent<HandController>().LevelUpEffect();
		hand.GetComponent<HandController>().SetIQ(100);
		Camera.main.GetComponent<CameraTarget>().AddDialogue("...These monsters. They can be eaten.");
	}

	// Enables the animator once it's on-screen
	void Activate(){
		if (!startedAnimation){
			anim.enabled = true;
			startedAnimation = true;

			autoZoomCameraFirst.SetActive(true);

			autoZoomCameraFirst.GetComponent<AutofitCamera>().manualOverrideFocus = true;
			autoZoomCameraSecond.GetComponent<AutofitCamera>().manualOverrideFocus = true;
		}
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject == hand){
			Activate();
		}
	}
}
