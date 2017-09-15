using UnityEngine;
using System.Collections;

public class Bird : GenericEnemy {
	PossessableScript ps;
	Rigidbody2D rb;
	Transform fallCheck;
	Animator anim;
	float moveDirection = 0;
	int direction = 1;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		rb = GetComponent<Rigidbody2D>();
		fallCheck = transform.Find("FallCheck");
		anim = GetComponent<Animator>();
	}
	void Flip(int dir){
		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * dir;
		transform.localScale = scale;
	}
	// Update is called once per frame
	void Update () {
		if (ps.possessed){
			anim.SetBool("possessed", true);
			if (Input.GetMouseButtonDown(0) && rb.velocity.y < 9f){
				rb.velocity += new Vector2(0f, 4f);
				anim.SetBool("flying", true);
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				moveDirection = 1;
				direction = 1;
				Flip(direction);
			} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
				moveDirection = -1;
				direction = -1;
				Flip(direction);
			} else {
				moveDirection = 0;
			}
		} else {
			anim.SetBool("possessed", false);
		}
		if (fallCheck.GetComponent<WallCheck>().IsColliding()){
			moveDirection = 0;
			anim.SetBool("flying", false);
		}
		Vector3 tempVelocity = rb.velocity;
		if (moveDirection != 0){
			tempVelocity.x += moveDirection * 6f * Time.deltaTime;
			if (tempVelocity.x > 5f){
				tempVelocity.x = 5f;
			} else if (tempVelocity.x < -5f){
				tempVelocity.x = -5f;
			}
		}
		if (moveDirection == 0){
			tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, 0f, 6f * Time.deltaTime);
		}

		rb.velocity = tempVelocity;

	}
}
