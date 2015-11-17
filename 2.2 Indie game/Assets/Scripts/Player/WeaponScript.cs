using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    enum WeaponMode { None,SemiFire,Auto };
    WeaponMode currentWeaponMode;

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
    int bulletsLeft = 0;
    bool isReloading = false;
    bool isFiring = false;
    bool weaponSelected = false;
    float damage = 20;
    float fireRate = 0.1f;
    float reloadTime = 3.0f;
    float nextFireTime;
    float delayFire = 0.6f;
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
        if (Input.GetButton("Fire"))
        {
            if (currentWeaponMode == WeaponMode.SemiFire)
            {

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

        Vector3 shootDirection = gameObject.transform.TransformDirection(1, 1, 1);
        RaycastHit hit;
        Vector3 shootingPos = transform.parent.position;
        if(Physics.Raycast(shootingPos,shootDirection, out hit, 100f))
        {
            Vector3 hitPoint = hit.point;
            Quaternion hitPointDecalPos = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (hit.transform.CompareTag("TestGround"))
            {
                //instantitate decal of the shot.
            }
        }

        //play sound
        //play animations

        //apply kickback affect
        RecoilEffect();
        bulletsLeft--;

    }

    void Reload()
    {

    }

    public bool canShoot()
    {
        if (Time.time < nextFireTime) { return false; }
        else { return true; }
    }

    void RecoilEffect()
    {
        recoilEffectGO.transform.localRotation = Quaternion.Euler(recoilEffectGO.transform.localRotation.eulerAngles - new Vector3(1, Random.Range(-1, 1), 0));
    }
}
