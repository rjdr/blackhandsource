using UnityEngine;
using System.Collections;

public class RippleController : MonoBehaviour {
	// Controls the ripple effect
	SpriteRenderer ripple;
	SpriteRenderer self;
	public bool startAtZero = false;
	// Use this for initialization
	void Start () {
		ripple = transform.Find("Ripple").GetComponent<SpriteRenderer>();
		self = GetComponent<SpriteRenderer>();
		if (startAtZero){
			ripple.material.SetFloat("_BumpAmt", 0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		ripple.sprite = self.sprite;
	}

	/// <summary>
	/// Sets how visible the ripple is. 0 is not visible, 1 is completely visible
	/// </summary>
	/// <param name="visPercent">Visibilty percentage</param>
	public void SetRippleVisibility(float visPercent){
		//ripple.color = new Color(1f, 1f, 1f, visPercent);
		ripple.material.SetFloat("_BumpAmt", Mathf.Clamp(visPercent*48f, 0, 48f));
		self.color = new Color(1f, 1f, 1f, 1-visPercent);
	}
	// Sets ripple visibility without touching the main sprite
	public void SetRippleOnly(float visPercent){
		ripple.material.SetFloat("_BumpAmt", Mathf.Clamp(visPercent*48f, 0, 48f));
	}
}
