using UnityEngine;
using System.Collections;

public class TrainEngine : GenericEnemy {
	MeshRenderer mr;
	Color c = new Color(1f, .2f, .6f);
	Color black = new Color(0f, 0f, 0f);
	public Texture2D possessedTexture;
	public Texture2D possessedCabinTexture;
	bool possessed = false;
	PossessableScript ps;
	Transform cameraLockPoint;
	Transform finalCameraPoint;
	Transform cam;
	GameObject engine;
	Animation ani;
	float maxSpeed = 3f;

	// Timer for dramatic camera rotation
	float camMoveTimer = -8f;
	// Point at which the train starts going beyond the screen
	float camMoveBackStart = 10f;
	float slowTrainEater = 8f;
	float camMoveBackFinish = 20f;
	float camRecenterFinal = 29f; 		// When the camera starts retargeting the train
	float camRecenterVel = .0f;			// Speed at which it retargets (increases for dramatic effect
	float exitLevelTime = 40f;			// When the game fades to black
	bool faded = false;
	float exitDelayTimer = 0f;

	Vector3 velocity = new Vector3 (0f, 0f, 0f);
	float xVel = .01f;
	float maxXVel = 1f;
	float xAccel = .015f;

	//Repeating ground object (reference it to speed it up)
	RepeatingGround repeatingGround;

	// Reference to the Train Eater to keep it under control & a point to move it to once the train's possessed
	GameObject trainEater;
	Transform trainEaterPoint;


	// Use this for initialization
	void Start () {
		mr = transform.Find("trainengine").Find("pCylinder4").gameObject.GetComponent<MeshRenderer>();
		ps = GetComponent<PossessableScript>();
		// Where the camera starts upon appearing
		cameraLockPoint = transform.Find("CameraLockPoint");
		// Where the camera ends up
		finalCameraPoint = transform.Find("CameraLockPoint2");
		engine = transform.Find("trainengine").gameObject;
		ani = engine.GetComponent<Animation>();

		repeatingGround = GameObject.Find("RepeatingTrainGroundContainer").GetComponent<RepeatingGround>();

		trainEater = GameObject.Find("TrainEater");
		trainEaterPoint = transform.Find("TrainEaterPosition");
	}

	public override void SpecialHighlighting(){
		mr.material.SetColor("_OutlineColor", c);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Activates special possession code
		if (!possessed && ps.possessed){
			// Slightly decreases fog density to better see the train
			RenderSettings.fogDensity = RenderSettings.fogDensity * .6f;
			// Swaps out textures for the possessed ones
			possessed = true;
			ps.DisableDepossession();
			mr.material.mainTexture = possessedTexture;
			transform.Find("trainengine").Find("TopThing1").gameObject.GetComponent<MeshRenderer>().material.mainTexture = possessedTexture;
			transform.Find("trainengine").Find("TopThing2").gameObject.GetComponent<MeshRenderer>().material.mainTexture = possessedTexture;
			transform.Find("trainengine").Find("Exhaust").gameObject.GetComponent<MeshRenderer>().material.mainTexture = possessedTexture;
			transform.Find("trainengine").Find("Cabin").gameObject.GetComponent<MeshRenderer>().material.mainTexture = possessedCabinTexture;
			transform.Find("trainengine").Find("OuterPanel").gameObject.GetComponent<MeshRenderer>().material.mainTexture = possessedCabinTexture;
			GetComponent<TraincarScript>().SetTriggerCount(-1);
			cam = Camera.main.transform;
			cam.GetComponent<LightningScript>().EnableFlash();
			cam.GetComponent<CameraTarget>().EnslaveCamera();

			// Sets the train eater to be right behind the train for dramatic effect
			trainEater.transform.position = trainEaterPoint.position;
			//trainEater.GetComponent<TrainEater>().MakeAppearance();
			trainEater.GetComponent<TrainEater>().beaten = true;
			trainEater.GetComponent<TrainEater>().SetVelocity(.8f);
			trainEater.GetComponent<TrainEater>().SetBaseVelocity(trainEater.GetComponent<TrainEater>().GetBaseVelocity()*.89f);
			trainEater.GetComponent<TrainEater>().DisableRigidBody();
			trainEater.transform.localScale *= 1.95f;
		} else if (possessed){
			// Moves camera by LERPing to the targeted point
			float timer = camMoveTimer;
			if (timer < 0f){
				timer = 0f;
			} else if (timer > 1f){
				timer = 1f;
			}

			// Moves camera to center on the train
			if (camMoveTimer < camMoveBackStart){
				cam.position = Vector3.Lerp(cameraLockPoint.position, finalCameraPoint.position, timer);
				cam.forward = Vector3.Lerp(cameraLockPoint.forward, finalCameraPoint.forward, timer);
				// Slows Train Eater just a tad
				trainEater.GetComponent<TrainEater>().SetBaseVelocity(trainEater.GetComponent<TrainEater>().GetBaseVelocity()*.9981f);
				// Makes the TrainEater slightly larger
				Vector3 tempScale = trainEater.transform.localScale;
				tempScale.y += .0015f;
				trainEater.transform.localScale = tempScale;
				trainEater.transform.Translate(0f, .0015f, 0f);
			// Moves train forward
			} else {
				trainEater.GetComponent<TrainEater>().SetBaseVelocity(trainEater.GetComponent<TrainEater>().GetBaseVelocity()*.96f);
				Vector3 tempPos = transform.position;

				if (xVel > maxXVel){
					xVel = maxXVel;
				} else if (xVel < -maxXVel/100f){
					xVel = -maxXVel/100f;
				}
				if (camMoveTimer > camMoveBackFinish){
					xVel += xAccel/2f;
				} else {
					xVel -= xAccel/40f;
				}
				tempPos += velocity;
				transform.position = tempPos;
				velocity.x = xVel;
			}
			// Makes the camera catch up with the train once it escapes
			if (camMoveTimer > camRecenterFinal){
				Vector3 destPoint = Camera.main.transform.position;
				destPoint.x = transform.position.x;
				camRecenterVel += .06f;
				Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, destPoint, camRecenterVel);
			}
			// Fades to black to exit the level
			if (camMoveTimer > exitLevelTime){
				if (!faded){
					Camera.main.GetComponent<FadeToBlack>().FadeOut();
					faded = true;
				// Loads next level once fade is complete
				} else {
					if (Camera.main.GetComponent<FadeToBlack>().GetAlpha() >= .99f){
						exitDelayTimer += Time.fixedDeltaTime;
						if (exitDelayTimer >= .5f){
							Application.LoadLevel("MapScene");
						}
					}
				}
			}

			camMoveTimer += Time.fixedDeltaTime*4f;

			if (ani["Take 001"].speed < maxSpeed){
				repeatingGround.SetActualVelocity(repeatingGround.GetVelocity() - new Vector3(.15f, 0f, 0f));
				ani["Take 001"].speed += .005f;
			}
		}
	}
	public void OnMouseExit(){
		mr.material.SetColor("_OutlineColor", black);
	}
}
