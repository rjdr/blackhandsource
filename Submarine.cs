using UnityEngine;
using System.Collections;

public class Submarine : GenericEnemy {
	public GameObject bubble;
	GameObject torpedo;
	GameObject waterSpirit;
	Transform spawn;
	Transform bubblePoint;
	Rigidbody2D rb;
	PossessableScript ps;
	float speed = 8.5f;
	float accel = 48f;
	float torpedoTimer = 0f;
	float maxTorpedoTimer = .4f;

	float bubbleTimer = 0f;
	float bubbleInterval = .1f;
	float incrBubbleInterval = .125f;

	// Starts appearance
	bool appearing = true;
	float appearTimer = 0f;
	float maxAppearTimer = 2f;
	Vector3 moveToSpawnPosition;
	Vector3	moveFromSpawnPosition;

	bool movedToNextPhase = false;	// Move to the escape phase of the battle where we chase the Water Spirit
	Vector3 MoveToPoint;			// Where we move to once possessed
	Vector3 WaterSpiritStart;
	Vector3 WaterSpiritDest;
	float moveWaterSpiritTimer = 0f;
	float maxMoveWaterSpiritTimer = 3f;

	float addPreDeathText = 1f;

	// Ends the boss battle
	bool startedBossEnd = false;
	bool battleComplete = false;

	// Ending images
	Transform BlackOut;
	Transform BlackOut2;
	Transform FabricImage;
	float fadeOutColorTimer = -5f;
	float maxFadeOutColorTimer = 3f;
	float reverseFadeOutColorTimer = 5f;
	float fadeDir = 1f;

	// Fades into a black screen once the boss is beaten
	float fadeInTimer = -.5f;
	float maxFadeInTimer = 2f;

	// Limits for where the sub can go
	Vector3 MinX;
	Vector3 MaxX;
	Vector3 MaxY;
	Vector3 MinY;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		ps = GetComponent<PossessableScript>();
		torpedo = (GameObject)Resources.Load("Torpedo");
		spawn = transform.Find("Spawn");
		bubblePoint = transform.Find("BubbleSpawn");

		waterSpirit = GameObject.Find("WaterSpirit");

		moveToSpawnPosition = transform.position;
		moveFromSpawnPosition = transform.position + new Vector3(0f, -6f, 0f);

		waterSpirit = GameObject.Find("WaterSpirit");
		MoveToPoint = transform.Find("MoveToPoint").position;
		WaterSpiritStart = transform.Find("WaterSpiritStart").position;
		WaterSpiritDest = transform.Find("WaterSpiritDest").position;

		// Handles a fade effect once the battle is over
		BlackOut = transform.Find("BlackOut");
		BlackOut2 = transform.Find("BlackOut2");
		FabricImage = transform.Find("FabricImage");
		BlackOut.transform.parent = Camera.main.transform;
		BlackOut2.transform.parent = Camera.main.transform;
		FabricImage.transform.parent = Camera.main.transform;
		BlackOut.transform.localPosition = new Vector3(-10f, -10f, 5f);
		BlackOut2.transform.localPosition = new Vector3(-10f, -10f, 5f);
		FabricImage.transform.localPosition = new Vector3(0f, 0f, 4.5f);
		BlackOut.gameObject.SetActive(false);
		BlackOut2.gameObject.SetActive(false);
		FabricImage.gameObject.SetActive(false);

		MinX = transform.Find("MinX").position;
		MaxX = transform.Find("MaxX").position;
		MaxY = transform.Find("MaxY").position;
		MinY = transform.Find("MinY").position;
	}

	void FadeIn(){
		if (fadeInTimer < maxFadeInTimer){
			fadeInTimer += Time.deltaTime;
			BlackOut2.GetComponent<SpriteRenderer>().color = Vector4.Lerp(Vector4.zero, Color.black, fadeInTimer / maxFadeInTimer);
			// Adds a quick chat once the fade is complete
			if (fadeInTimer >= maxFadeInTimer){
				BlackOut.gameObject.SetActive(true);
				//BlackOut2.gameObject.SetActive(true);
				FabricImage.gameObject.SetActive(true);
				AddFastText(DialogueTable.GetChat("waterspiritdie").text, -200f, 100f, false, false);
			}
		}
		// Once the fade to black is finished, fade out to a Hand logo, then the map
		if (fadeInTimer >= maxFadeInTimer){
			FadeOut();
		}
	}

	// Fades into an image+text, then goes backwards and goes to the title screen
	void FadeOut(){
		fadeOutColorTimer += fadeDir * Time.deltaTime;
		BlackOut.GetComponent<SpriteRenderer>().color = Vector4.Lerp(Color.black, Vector4.zero, fadeOutColorTimer / maxFadeOutColorTimer);
		FabricImage.transform.localEulerAngles = Vector3.zero;
		// Reverses the timer so that we fade back out
		if (fadeOutColorTimer > reverseFadeOutColorTimer){
			fadeDir = -1f;
		}
		// Loads the map
		if (fadeDir < 0f && fadeOutColorTimer < -1f){
			Application.LoadLevel("MapScene");
		}
	}
	
	// Update is called once per frame
	void Update () {
		damageTimer -= Time.deltaTime;
		// Move up once it initially appears
		if (appearing){
			appearTimer += Time.deltaTime;
			transform.position = Vector3.Lerp(moveFromSpawnPosition, moveToSpawnPosition, appearTimer / maxAppearTimer);
			if (appearTimer >= maxAppearTimer || ps.possessed){
				appearing = false;
			}
		}

		// Possessed and in control
		torpedoTimer += Time.deltaTime;
		if (!ps.possessed){
			life = maxLife;
		}
		else if (ps.possessed){
			// Keeps locked to field of movement
			if (transform.position.x < MinX.x){
				transform.position = new Vector3(MinX.x, transform.position.y, transform.position.z);
			} else if (transform.position.x > MaxX.x){
				transform.position = new Vector3(MaxX.x, transform.position.y, transform.position.z);
			}
			if (transform.position.y > MaxY.y){
				transform.position = new Vector3(transform.position.x, MaxY.y, transform.position.z);
			} else if (transform.position.y < MinY.y){
				transform.position = new Vector3(transform.position.x, MinY.y, transform.position.z);
			}
			// Move to the escape phase of the battle
			if (!movedToNextPhase){
				Camera.main.GetComponent<CameraTarget>().TriggerLightning();
				movedToNextPhase = true;
				transform.position = MoveToPoint;
				Vector3 tempCamPos = Camera.main.transform.position;
				tempCamPos = new Vector3(transform.position.x, transform.position.y, tempCamPos.z);
				Camera.main.transform.position = tempCamPos;
				// Moves the Y Border out of the way so the camera will move down
				Camera.main.GetComponent<CameraTarget>().SetYBorderPos(-100f);
			}
			// Moves the Water Spirit to the correct place
			if (moveWaterSpiritTimer < maxMoveWaterSpiritTimer){
				// Makes the Water Spirit bigger for this phase
				if (moveWaterSpiritTimer == 0f){
					waterSpirit.GetComponent<WaterSpirit>().Enlarge();
				}
				moveWaterSpiritTimer += Time.deltaTime;
				waterSpirit.transform.position = Vector3.Lerp(WaterSpiritStart, WaterSpiritDest, moveWaterSpiritTimer / maxMoveWaterSpiritTimer);
				// Sets this as the centerpoint for moving around and begins the next phase
				if (moveWaterSpiritTimer >= maxMoveWaterSpiritTimer){
					waterSpirit.GetComponent<WaterSpirit>().SetEscapeCenterPoint(WaterSpiritDest);
				}
			}

			Vector2 vel = rb.velocity;
			bool movingY = false;
			if (Input.GetAxisRaw("Vertical") > 0f){
				vel.y += accel * Time.deltaTime;
				movingY = true;
			} else if (Input.GetAxisRaw("Vertical") < 0f){
				vel.y -= accel * Time.deltaTime;
				movingY = true;
			}
			vel.y = Mathf.Clamp(vel.y, -speed, speed);
			// Slows down if not moving
			if (!movingY){
				vel.y = Mathf.MoveTowards(vel.y, 0f, accel * Time.deltaTime * 1.5f);
			}

			bool movingX = false;
			if (Input.GetAxisRaw("Horizontal") > 0f){
				vel.x += accel * Time.deltaTime;
				movingX = true;
			} else if (Input.GetAxisRaw("Horizontal") < 0f){
				vel.x -= accel * Time.deltaTime;
				movingX = true;
			}
			vel.x = Mathf.Clamp(vel.x, -speed, speed);
			// Slows down if not moving
			if (!movingX){
				vel.x = Mathf.MoveTowards(vel.x, 0f, accel * Time.deltaTime * 1.5f);
			}

			rb.velocity = vel;

			// Releases a torpedo
			if (Input.GetButton("Fire1") && torpedoTimer >= maxTorpedoTimer){
				torpedoTimer = 0f;
				GameObject t = (GameObject)Instantiate(torpedo);
				t.transform.position = spawn.position;
			}
			// Ends the boss battle and prepares the sub to eat the Waterspirit
			if (!startedBossEnd && waterSpirit.GetComponent<GenericEnemy>().life <= 0f){
				startedBossEnd = true;
				Camera.main.GetComponent<CameraTarget>().TriggerLightning();
				GetComponent<Animator>().SetBool("Eat", true);
				transform.position = MoveToPoint;
			}
			// Adds the dialogue to mark the death
			else if (startedBossEnd){
				if (addPreDeathText > 0f){
					addPreDeathText -= Time.deltaTime;
					if (addPreDeathText <= 0f){
						AddFastText(DialogueTable.GetChat("waterspiritdoit").text, -200f, 100f, true, true);
					}
				}
			}

			// Fade event to end the battle
			if (battleComplete){
				FadeIn();
				rb.velocity = Vector2.zero;
			}

			// Spawns bubbles from behind
			bubbleTimer += Time.deltaTime;
			if (bubbleTimer >= bubbleInterval){
				bubbleInterval += incrBubbleInterval * Random.Range(.5f, 1f);
				GameObject b = (GameObject)Instantiate(bubble);
				b.transform.position = bubblePoint.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), 0f);
			}
		}
	}

	// Eats the boss and ends the level
	void OnTriggerEnter2D(Collider2D col){
		if (startedBossEnd && col.gameObject.name == "WaterSpirit" && !battleComplete){
			life = maxLife;
			battleComplete = true;
			col.gameObject.GetComponent<Shatterable>().Shatter();
			//AddFastText(DialogueTable.GetChat("waterspiritdie").text, -200f, 100f, true, true);
			Destroy(col.gameObject, 0f);

			Camera.main.GetComponent<CameraTarget>().AddShake(.8f);
			//BlackOut.gameObject.SetActive(true);
			BlackOut2.gameObject.SetActive(true);
			//FabricImage.gameObject.SetActive(true);
		}
	}
}
