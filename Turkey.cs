using UnityEngine;
using System.Collections;

public class Turkey : GenericEnemy {
	PossessableScript ps;
	Animator m_Anim;
	Rigidbody2D m_Rigidbody2D;
	float moveDirection = 1f;
	float chaseSpeed = 4f;
	float walkSpeed = 4f;
	int facing = 1;
	bool possessedLastFrame = false;
	// Use this for initialization
	void Start () {
		ps = GetComponent<PossessableScript>();
		m_Anim = GetComponent<Animator>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		transform.Find("Plate").SetParent(null);
		
	}
	public void Move(){
		// Move the character
		m_Rigidbody2D.velocity = new Vector2(moveDirection*walkSpeed, m_Rigidbody2D.velocity.y);
		
		if (moveDirection > 0 && facing < 0)
		{
			// ... flip the player.
			Flip();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (moveDirection < 0 && facing > 0)
		{
			// ... flip the player.
			Flip();
		}
	}
	void PlayerControls(){
		if (!ps.inIntroAnim){
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
				moveDirection = 1;
			} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
				moveDirection = -1;
			} else {
				moveDirection = 0;
			}
			// Move the character
			m_Rigidbody2D.velocity = new Vector2(moveDirection*chaseSpeed, m_Rigidbody2D.velocity.y);
			
			if (moveDirection > 0 && facing < 0)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (moveDirection < 0 && facing > 0)
			{
				// ... flip the player.
				Flip();
			}
			// Sync up animations
			if (m_Rigidbody2D.velocity.x != 0f){
				m_Anim.SetFloat("Speed", 1f);
			} else {
				m_Anim.SetFloat("Speed", 0f);
			}
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		// Can't do anything if frozen
		if (!ps.frozen){
			if (ps.possessed){
				if (!possessedLastFrame){
					Camera.main.GetComponent<CameraTarget>().TriggerLightning();
					possessedLastFrame = true;
				}
				//rigid.isKinematic = false;
				m_Anim.SetBool("Possessed", true);
				PlayerControls();
			} else {
				possessedLastFrame = false;
				m_Anim.SetBool("Possessed", false);
				m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
			}
		} else {
		}
	}
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		if (moveDirection > 0) {
			facing = 1;
		} else if (moveDirection < 0){
			facing = -1;
		}
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
