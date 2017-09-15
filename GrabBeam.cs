using UnityEngine;
using System.Collections;

public class GrabBeam : MonoBehaviour {
	protected Transform leftPoint;
	protected Transform rightPoint;
	protected float maxTimer = .1f;
	protected float timer = 0f;
	protected Vector3 startPosition = new Vector3(0f, 0f, 0f);
	protected float velocity = 0f;
	protected GameObject player;
	protected MeshRenderer mr;
	protected Color c = new Color(1f, .2f, .6f);
	protected Color black = new Color(.1f, .1f, .1f);
	// Use this for initialization
	void Start () {
		mr = transform.Find("grabbeam").gameObject.GetComponent<MeshRenderer>();
		leftPoint = transform.Find("LeftPoint");
		rightPoint = transform.Find("RightPoint");
		mr.material.SetColor("_OutlineColor", black);
		player = GameObject.Find("TheHand");
		// Set the bar to be at the same depth as the player (otherwise it'll look fucked up)
		//Vector3 tempPos = transform.position;
		//tempPos.z = player.transform.position.z;
		//transform.position = tempPos;
	}

	// Pulls the player up once the hand starts returning
	void FixedUpdate(){
		if (timer > 0f){
			if (player.GetComponent<HandController>().GrabReturning()){
				timer -= Time.fixedDeltaTime;
				// Don't mess with player's Z depth
				Vector3 destVect = transform.position;
				destVect.z = player.transform.position.z;
				player.transform.position = Vector3.MoveTowards(player.transform.position, destVect, velocity);
				if (timer <= 0f){
					FlipPlayer();
				}
			}
		}
	}

	// Makes the player stick to the surface (since the GrabBeam's up vector faces down, have the player's up vector be the opposite of that
	void FlipPlayer(){
		Vector3 up = -transform.up;
		up.z = 0f;
		up = up.normalized;
		player.GetComponent<HandController>().StickToSurface(up);
	}

	/// <summary>
	/// When clicked, the player moves to the the beam
	/// 			NOTE:
	/// If there's another collider (including triggers) in front of the beam, the beam will be unclickable
	/// Make sure the grab beam is always closer to the camera than other potential colliders
	/// </summary>
	void OnMouseDown(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		if (player.activeSelf){
			// Escape if the player and beam are on different layers
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.layer == bgLayer && (gameObject.layer == fgLayer || gameObject.layer == defaultLayer)){
				return;
			}
			if (player.layer == defaultLayer && gameObject.layer == bgLayer){
				return;
			}
			if (player.GetComponent<HandController>().AttackMenu.activeSelf == false || player.GetComponent<HandController>().grabbedObject == null){
				// Checks if player can make contact with the center or edges of the beam
				if (player.GetComponent<HandController>().ClearPathToTarget(transform.position) || player.GetComponent<HandController>().ClearPathToTarget(rightPoint.position) || 
				 player.GetComponent<HandController>().ClearPathToTarget(leftPoint.position)){
					// Disables attack menu to pull towards a rod
					if (player.GetComponent<HandController>().AttackMenu.activeSelf){
						player.GetComponent<HandController>().SetAttackMenuStatus(false);
						player.GetComponent<HandController>().NormalTime();
					}

					player.GetComponent<HandController>().TimedLockControls(maxTimer);
					//player.transform.position = transform.position;
					timer = maxTimer;
					startPosition = player.transform.position;
					velocity = Vector3.Distance(player.transform.position, transform.position) / (maxTimer/(Time.fixedDeltaTime));
				}
			}
		}
	}

	public void OnMouseEnter(){
		int bgLayer = LayerMask.NameToLayer("BackgroundLayer");
		int fgLayer = LayerMask.NameToLayer("ForegroundOnly");
		int defaultLayer = LayerMask.NameToLayer("Default");
		if (player.activeSelf){
			// Escape if the player and beam are on different layers
			// Only grab if the attack menu is disabled OR (if the attack menu is open but nothing is grabbed)
			if (player.layer == bgLayer && (gameObject.layer == fgLayer || gameObject.layer == defaultLayer)){
				return;
			}
			if (player.layer == defaultLayer && gameObject.layer == bgLayer){
				return;
			} else {
				mr.material.SetColor("_OutlineColor", c);
			}
		}
	}

	public void OnMouseExit(){
		mr.material.SetColor("_OutlineColor", black);
	}
}
