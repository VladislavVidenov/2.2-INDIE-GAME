using UnityEngine;
using System.Collections;

public class MovingCubes : MonoBehaviour {

	Light[] myLights;
	Animator myAnimator;
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator> ();
		myLights = GetComponentsInChildren<Light> ();
		myLights[0].intensity = 0;
        myLights[1].intensity = 0;
        myAnimator.speed = 0;
		//myAnimator.enabled = false;
	}
    void MoveAnimation()
    {
        myLights[0].intensity = 8;

        myLights[1].intensity = 8;
        myAnimator.speed = 8;
        Invoke("StopAnimator", 0.2f);
    }
	void StopAnimator()
	{
		myLights[0].intensity = 0;
        myLights[1].intensity = 0;
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
