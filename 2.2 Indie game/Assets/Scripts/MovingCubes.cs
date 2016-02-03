using UnityEngine;
using System.Collections;

public class MovingCubes : MonoBehaviour {

	Light myLight;
	Animator myAnimator;
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator> ();
		myLight = GetComponentInChildren<Light> ();
		myLight.intensity = 0;
		myAnimator.speed = 0;
		//myAnimator.enabled = false;
	}
	void MoveAnimation()
	{
		myLight.intensity = 8;
		myAnimator.speed = 8;
		Invoke ("StopAnimator", 0.2f);
	}
	void StopAnimator()
	{
		myLight.intensity = 0;
		myAnimator.speed = 0;
	}

	void OnEnable()
	{
		WeaponScript.OnPistolShoot += MoveAnimation;
		print("Subscribed !");
	}
	
	
	void OnDisable()
	{
		WeaponScript.OnPistolShoot -= MoveAnimation;
		print("UnSubscribed !");
	}
}
