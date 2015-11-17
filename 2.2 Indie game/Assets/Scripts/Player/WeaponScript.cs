using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    public enum WeaponMode { None,SemiFire,Auto };
    public WeaponMode currentWeaponMode;

    //Components
    [SerializeField]
    GameObject weaponCamera;
    GameObject mainCamera;
    GameObject player;
    [SerializeField]
    GameObject recoilEffectGO;
    //Variables
    
    //---Aiming---
    Vector3 defaultPosition;
    Vector3 aimPosition;
    bool isAiming = false;
    float aimSpeed = 0;
    //bool animationRun = false;
    //-----------

    //---Shooting---
    int bulletsLeft = 12222;
    bool isReloading = false;
    bool isFiring = false;
    bool weaponSelected = false;
    public float damage = 20;
    public float fireRate = 0.1f;
    public float reloadTime = 3.0f;
    public float nextFireTime;

    //--------------


    //---Weapon Accuracy ---

    //----------------------



    //--- FOV ---

    //-----------

    void Start () {
        mainCamera = Camera.main.gameObject;
        player = GameManager.Instance.Player;

	}
	
	
	void Update () {
        if (Input.GetButtonDown("Fire"))
        {
            if (currentWeaponMode == WeaponMode.SemiFire)
            {
                SemiFireMode();
            }
        }
	}


    void SemiFireMode()
    {
        if (isReloading || bulletsLeft <= 0)
        {
            if (bulletsLeft == 0)
            {
                // OutOfAmmo();
                Debug.Log("IM OUT OF AMMO BISH!");
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            FireOneBullet();
            nextFireTime = Time.time + fireRate;
        }


    }
 
    void FireOneBullet()
    {
        if (nextFireTime > Time.time)
        {
            if (bulletsLeft <= 0)
            {
                Debug.Log("IM OUT OF AMMO BISH!");
            }
            return;
        }

        Vector3 shootDirection = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f,0.01f), Random.Range(-0.01f, 0.01f),1));
        RaycastHit hit;
        Vector3 shootingPos = transform.parent.position;
        Debug.DrawRay(shootingPos, shootDirection * 100f, Color.red, 2);
        if(Physics.Raycast(shootingPos,shootDirection, out hit, 100f))
        {

            Vector3 hitPoint = hit.point;
            Quaternion hitPointDecalPos = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (hit.transform.CompareTag("Ground"))
            {
                //instantitate decal of the shot.
            }
        }

        //play sound
        //play animations
        Debug.Log("Shooting");
        //apply kickback affect
        RecoilEffect();
        bulletsLeft--;

    }

    void Reload()
    {

    }




    void RecoilEffect()
    {
        recoilEffectGO.transform.localRotation = Quaternion.Euler(recoilEffectGO.transform.localRotation.eulerAngles - new Vector3(1, Random.Range(-1, 1), 0));
    }
}
