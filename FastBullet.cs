﻿using UnityEngine;
using System.Collections;

public class FastBullet : Weapon {
	float speed = 36f;
	public float damageVal = 5f;
	bool destroyed = false;
	float timer = 9f;
	// Use this for initialization
	void Start () {
	
	}

	/// <summary>
	/// Fires it straight from the given vector
	/// </summary>
	public void Fire(Vector3 v){
		transform.right = v;
		GetComponent<Rigidbody2D>().velocity = speed * transform.right;
	}
		
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f){
			destroyed = true;
		}
		if (destroyed){
			Destroy(gameObject, 0f);
		}
	}
	void OnTriggerStay2D(Collider2D col){
		if (col.gameObject == gameObject || col.gameObject.GetComponent<SoundRing>() || col.gameObject.GetComponent<FastBullet>()){
			return;
		}
		if (col.gameObject.GetComponent<GenericEnemy>() && col.gameObject != spawn){
			col.gameObject.GetComponent<GenericEnemy>().DamagedEvent(gameObject);
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			// Triggers an event when it damages an object
			destroyed = true;
		} else if (col.isTrigger == false){
			destroyed = true;
		}
	}
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject == gameObject || col.gameObject.GetComponent<SoundRing>() || col.gameObject.GetComponent<FastBullet>()){
			return;
		}
		if (col.gameObject.GetComponent<GenericEnemy>() && col.gameObject != spawn){
			col.gameObject.GetComponent<GenericEnemy>().DamagedEvent(gameObject);
			col.gameObject.GetComponent<GenericEnemy>().DelayedDamage(damageVal);
			// Triggers an event when it damages an object
			destroyed = true;
		} else if (col.gameObject.GetComponent<GenericEnemy>() == null){
			destroyed = true;
		}
	}
}
