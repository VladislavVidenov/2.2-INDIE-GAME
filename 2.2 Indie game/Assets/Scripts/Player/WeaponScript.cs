using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
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
    AudioSource audio;
    #region Sounds
    [SerializeField]
    AudioClip dryFireSound;
    [SerializeField]
    AudioClip reloadSound;
    [SerializeField]
    AudioClip fireSound;
    #endregion

    //---SHOOT DECALS-------
    [SerializeField]
    GameObject normalDecal;
    //----------------------
    //Variables
    
    //---Aiming---
    Vector3 defaultPosition;
    Vector3 aimPosition;
    bool isAiming = false;
    float aimSpeed = 0;
    //bool animationRun = false;
    //-----------

    //---Shooting---
    
    public int bulletsInMagazine = 6;
    public int totalBulletCount = 24;
    int maxBulletsPerMag;
    bool isReloading = false;
    bool isFiring = false;
    bool weaponSelected = false;
    bool outOfAmmoSoundPlaying = false;
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
        audio = GetComponent<AudioSource>();
        mainCamera = Camera.main.gameObject;
        player = GameManager.Instance.Player;
        maxBulletsPerMag = bulletsInMagazine;
    }
	
	
	void Update () {
        if (Input.GetButtonDown("Fire"))
        {
          
            if (currentWeaponMode == WeaponMode.SemiFire)
            {
                SemiFireMode();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
	}



    void OnGUI()
    {
        GUI.contentColor = Color.red;
        GUI.Label(new Rect(10, 10, 100, 50), "mag" + bulletsInMagazine);
        GUI.Label(new Rect(10, 25, 100, 50), "total" + totalBulletCount);
    }
    IEnumerator waitSound()
    {
        yield return new WaitForSeconds(0.2f);
        outOfAmmoSoundPlaying = false;
    }
 
    void SemiFireMode()
    {
        if (isReloading || bulletsInMagazine <= 0)
        {
            if (bulletsInMagazine <= 0)
            {
                DryFire();
            }
            return;
        }

        if (CanFire()) FireOneBullet();
    }
 
    void FireOneBullet()
    {
        Vector3 shootDirection = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f,0.01f), Random.Range(-0.01f, 0.01f),1));
        RaycastHit hit;
        Vector3 shootingPos = transform.parent.position;
        Debug.DrawRay(shootingPos, shootDirection * 100f, Color.red, 2);
        audio.PlayOneShot(fireSound);
        if(Physics.Raycast(shootingPos,shootDirection, out hit, 100f))
        {

            Vector3 hitPoint = hit.point;
            Quaternion hitPointDecalPos = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.CompareTag("Ground"))
            {
            
                GameObject go = Instantiate(normalDecal,new Vector3(hitPoint.x,hitPoint.y + 0.00001f,hitPoint.z),hitPointDecalPos) as GameObject;
                go.transform.parent = hit.transform;
            }
        }

        //play sound
        //play animations
        Debug.Log("Shooting");
        //apply kickback affect
        RecoilEffect();
        bulletsInMagazine--;

    }




    #region Reloading 
    IEnumerator ReloadTime(float time)
    {
        yield return new WaitForSeconds(time);
        int bulletsToLoad = maxBulletsPerMag - bulletsInMagazine;
        totalBulletCount -= bulletsToLoad;
        bulletsInMagazine += bulletsToLoad;
        isReloading = false;
    }

    void Reload()
    {
        //if we are already reloading or we are already on full ammo in mag-> return;
        if (isReloading || bulletsInMagazine == maxBulletsPerMag) return;

        if (bulletsInMagazine >= 0 && totalBulletCount > 0)
        {
            isReloading = true;
            //--- set animation reload speed
            //--- play reload gun animation
            audio.PlayOneShot(reloadSound);
            StartCoroutine(ReloadTime(reloadTime));
        }

    }
    #endregion
    void DryFire()
    {
        if (isReloading || outOfAmmoSoundPlaying) return;
        outOfAmmoSoundPlaying = true;
        audio.PlayOneShot(dryFireSound);
        StartCoroutine(waitSound());
    }
    void RecoilEffect()
    {
        recoilEffectGO.transform.localRotation = Quaternion.Euler(recoilEffectGO.transform.localRotation.eulerAngles - new Vector3(1, Random.Range(-1, 1), 0));
    }
    bool CanFire()
    {
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            nextFireTime = Time.time + fireRate;
            return true;
        }
        return false;
    }
}
