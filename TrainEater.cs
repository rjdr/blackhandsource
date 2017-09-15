using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainEater : MonoBehaviour {
	Rigidbody2D rb;
	// For keeping track of the Dustcloud spawn points
	List<Transform> dustSpawnPoints = new List<Transform>();
	Quaternion zeroRotation = new Quaternion();
	// Reference to the cloud so it's not endlessly reading from the hard drive
	Object dustCloud;

	// How far up it'll move on the y axis when appearing
	float startY;
	float endY = 8.5f;

	// Rate at which it moves
	// It should have a sinusoidal velocity so it puts along
	Vector2 velocity = new Vector2(.3f, 2f);
	float xVelocityAng = 0f;
	float xVelocityDAng = 5f*Mathf.Deg2Rad;
	float xVelocity = 2f;
	float baseXVelocity = 4f;
	float yVelocity = 0f;

	Vector2 cameraShake = new Vector2(0f, 0f);
	float maxShake = .2f;
		
	Vector3 bodyShake = new Vector3(0f, 0f, 0f);

	Transform renderers;
	
	// Set the TrainEater to be the camera target when it first appears
	float appearTargetTime = 4f;
	bool appeared = false;

	// Objects it's touched and will eat
	ArrayList hitObjects = new ArrayList();

	// The player and the point he needs to move beyond to activate the train eater
	Transform player;
	Transform activationPoint;
	Vector3 lastMoveUpPoint;

	// For controlling initial appearance events
	// Move faster when initially appearing & have it move higher
	float initialAppearanceTimer = 0f;
	float maxInitialAppearanceTimer = 9.5f;
	// Start from in the ground and move upwards
	Vector3 topPoint;
	Vector3 startPoint;
	float moveUpRate = 1f;
	bool reachedTop = false;

	// Have a bit of rubberbanding when there's huge distance or small distance between the player
	float minDist = 12f;
	float maxDist = 25f;

	// We got to the train in time
	public bool beaten = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		dustCloud = Resources.Load("Dustcloud");
		dustSpawnPoints.Add (transform.Find("Dustspawn1"));
		dustSpawnPoints.Add (transform.Find("Dustspawn2"));
		dustSpawnPoints.Add (transform.Find("Dustspawn3"));

		startY = transform.position.y;
		endY += startY;

		renderers = transform.Find("Renderers");

		player = GameObject.Find("TheHand").transform;
		activationPoint = transform.Find("ActivationPoint");

		// For coming up during the first appearance
		topPoint = renderers.transform.localPosition;
		topPoint.x = 0f;
		startPoint = transform.Find("ComeUpFromPoint").localPosition;
		startPoint.x = 0f;
		lastMoveUpPoint = transform.Find("ComeUpFromPoint").localPosition;
		lastMoveUpPoint.x = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (appeared){
			//SpawnDust();
			Chase ();
		} else {
			if (player.position.x > activationPoint.position.x){
				MakeAppearance();
			}
		}
	}
	void SpawnDust(){
		if (Random.value > .89f){
			Instantiate(dustCloud, dustSpawnPoints[0].position + new Vector3(Random.Range(-2f, 0f), Random.Range(0f, .5f), 0f), zeroRotation);
		}
		if (Random.value > .95f){
			Instantiate(dustCloud, dustSpawnPoints[1].position + new Vector3(Random.Range(-2f, 0f), 0f, 0f), zeroRotation);
		}
		if (Random.value > .97f){
			Instantiate(dustCloud, dustSpawnPoints[2].position, zeroRotation);
		}
	}

	// Make the TrainEater appear
	public void MakeAppearance(){
		if (!appeared){
			appeared = true;
			// Move the renderer down
			Vector3 tempStart = startPoint;
			tempStart.x = 0f;
			tempStart.z = 0f;
			renderers.transform.localPosition = tempStart;

			// Move the Eater to the proper position
			transform.position = transform.Find("MoveToPoint").position;

			// Unzooms camera to show the eater
			Camera.main.GetComponent<CameraTarget>().QuickZoom(Camera.main.GetComponent<CameraTarget>().GetOriginalZoom() - 2.5f);
			Camera.main.GetComponent<CameraTarget>().SetUnzoomDelay(appearTargetTime);
		}
	}

	// Sets the velocity
	public void SetVelocity(float f){
		xVelocity = f;
	}
	public void SetBaseVelocity(float f){
		baseXVelocity = f;
	}
	public float GetBaseVelocity(){
		return baseXVelocity;
	}
	public void DisableRigidBody(){
		rb.isKinematic = true;
	}

	// Actions involving being on screen and attacking
	void Chase(){
		if (appeared){
			// Makes the camera target the player when it first appears
			if (appearTargetTime > 0f){
				Camera.main.GetComponent<CameraTarget>().target = transform;
				appearTargetTime -= Time.fixedDeltaTime;
				// Sets focus back to player
				if (appearTargetTime <= 0f){
					GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
					for (int i = 0; i < p.Length; i++){
						if (p[i].activeSelf){
							Camera.main.GetComponent<CameraTarget>().target = p[i].transform;
							break;
						}
					}
				}
			}
			// Gives the body a rattle to make it more interesting
			renderers.localPosition -= bodyShake;
			Vector3 tempMoveUpPoint = lastMoveUpPoint;
			tempMoveUpPoint.x = 0f;
			tempMoveUpPoint.z = 0f;
			renderers.localPosition -= tempMoveUpPoint;
			Vector3 tempRenderPos = renderers.localPosition;
			bodyShake = new Vector3(Random.Range(-.02f, .02f), Random.Range(-.02f, .02f), 0f);
			//bodyShake = new Vector3(0f, Random.Range(-.05f, .05f), 0f);
			
			tempRenderPos += bodyShake;
			if (tempRenderPos.x < -.02f){
				tempRenderPos.x = -.02f;
			} else if (tempRenderPos.x > .02f){
				tempRenderPos.x = .02f;
			}
			if (tempRenderPos.y < -.02f){
				tempRenderPos.y = -.02f;
			} else if (tempRenderPos.y > .02f){
				tempRenderPos.y = .02f;
			}
			renderers.localPosition = tempRenderPos;
			// Moves the renderers up when it first appears
			lastMoveUpPoint = Vector3.MoveTowards(lastMoveUpPoint, topPoint, moveUpRate*Time.fixedDeltaTime);
			tempMoveUpPoint = lastMoveUpPoint;
			tempMoveUpPoint.x = 0f;
			tempMoveUpPoint.z = 0f;
			// Mark that we've reached the top
			if (tempMoveUpPoint.y == 0f){
				reachedTop = true;
			}
			renderers.localPosition += tempMoveUpPoint;

			//velocity.x = baseXVelocity + xVelocity * Mathf.Abs(Mathf.Sin(xVelocityAng));
			velocity.x = baseXVelocity;
			// Move faster when it first appears
			if (initialAppearanceTimer < maxInitialAppearanceTimer){
				velocity.x *= 2.5f;
				initialAppearanceTimer += Time.fixedDeltaTime;
				// Little bits of cleanup and modification, like using smaller particles
				if (initialAppearanceTimer >= maxInitialAppearanceTimer){
					//transform.Find("Dust").GetComponent<ParticleEmitter>().maxSize = transform.Find("Dust").GetComponent<ParticleSystem>().maxSize * .66f;
					//transform.Find("Dust").GetComponent<ParticleEmitter>().minSize = transform.Find("Dust").GetComponent<ParticleEmitter>().minSize * .66f;
					//ParticleSystem ps = transform.Find("Dust").GetComponent<ParticleSystem>();
					//ps.startSize = 2f;
				}
			}
			// Slow down a bit if the distance is very small
			if (!beaten){
				if (Mathf.Abs(transform.position.x - player.position.x) < minDist){
					velocity.x *= .75f;
				// Speed up a bit to catch up to the player if the distance is small
				} else if (initialAppearanceTimer > maxInitialAppearanceTimer && Mathf.Abs(transform.position.x - player.position.x) > minDist){
					velocity.x *= 1.4f;
				}
			}
		
			xVelocityAng += xVelocityDAng;
			if (transform.position.y > endY){
				yVelocity = 0f;
			}
			velocity.y = yVelocity;
			if (!rb.isKinematic){
				rb.velocity = velocity;
			} else {
				Vector3 tempVel = new Vector3(velocity.x, velocity.y, 0f) * Time.fixedDeltaTime*.92f;
				transform.position += tempVel;
			}
		}
	}

	void OnTriggerStay2D(Collider2D col){
		//if (!hitObjects.Contains(col.gameObject)){
		//	hitObjects.Add(col.gameObject);
		//}
		if (col.transform == player){
			player.GetComponent<GenericEnemy>().DelayedDamage(1000f);
		}
	}
	void OnTriggerExit2D(Collider2D col){
		if (hitObjects.Contains(col.gameObject) && col.transform.position.x < transform.position.x){
			GameObject g = col.gameObject;
			// Won't eat the train engine
			if (!(g.name == "TrainEngine" || g.name == "trainengine")){
				hitObjects.Remove(g);
				//Destroy(g, 0f);
			}
		}
	}
}
