using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour {
    WeaponManager weaponManager;
    public enum WeaponMode { None,SemiFire,Auto };
    public WeaponMode currentWeaponMode;

    //Components
    [SerializeField]
    GameObject weaponCamera;
    GameObject mainCamera;
    GameObject player;
    [SerializeField]
    GameObject recoilEffectGO;
    AudioSource audioSource;
    #region Sounds
    [SerializeField] AudioClip dryFireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip pullOutSound;

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

    public float damage = 20;
    public float fireRate = 0.1f;
    public float reloadTime = 3.0f;
    public float pullOutWeaponTime;
    public float nextFireTime;
    bool isReloading = false;
    bool isFiring = false;
    bool weaponSelected = false;
    bool outOfAmmoSoundPlaying = false;
    bool reloadInfo = false;
    bool reloadInfoStarted = false;
    //--------------


    //---Weapon Accuracy ---

    //----------------------



    //--- FOV ---

    //-----------

    void Start () {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main.gameObject;
        player = GameManager.Instance.Player;
        maxBulletsPerMag = bulletsInMagazine;
        weaponManager = FindObjectOfType<WeaponManager>();
    }
	
	
	void Update () {
       // Debug.Log(isReloading);
        if (Input.GetButtonDown("Fire"))
        {
          
            if (currentWeaponMode == WeaponMode.SemiFire)
            {
                SemiFireMode();
            }

        }else if (Input.GetButton("Fire"))
        {
            if(currentWeaponMode == WeaponMode.Auto)
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
        if (reloadInfo) GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 50), "You have no bullets left");
    }
    IEnumerator waitSound()
    {
        yield return new WaitForSeconds(0.2f);
        outOfAmmoSoundPlaying = false;
    }
 
    void SemiFireMode()
    {//If we are currently reloading / or we ran out of ammo -> return and play dry fire sound:).
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

        audioSource.PlayOneShot(fireSound);
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
        int bulletsShot = maxBulletsPerMag - bulletsInMagazine;
        int tBCcopy = totalBulletCount;
        totalBulletCount -= bulletsShot;
        if (totalBulletCount < 0) totalBulletCount = 0;
        int delta = tBCcopy - totalBulletCount;
        bulletsInMagazine += delta;
        isReloading = false;
    }
    IEnumerator shutReloadInfo()
    {
        yield return new WaitForSeconds(2f);
        reloadInfo = false;
        reloadInfoStarted = false;
    }
    void Reload()
    {
        //if we are already reloading or we are already on full ammo in mag-> return;
        if (isReloading || bulletsInMagazine == maxBulletsPerMag) return;
        reloadInfo = (totalBulletCount <= 0) ? true : false;
        if (reloadInfo && !reloadInfoStarted) { reloadInfoStarted = true; StartCoroutine(shutReloadInfo()); }

        if (bulletsInMagazine >= 0 && totalBulletCount > 0)
        {
            isReloading = true;
            //--- set animation reload speed
            //--- play reload gun animation
            audioSource.PlayOneShot(reloadSound);
            StartCoroutine(ReloadTime(reloadTime));
        }

    }
    #endregion
    void DryFire()
    {
        if (isReloading || outOfAmmoSoundPlaying) return;
        outOfAmmoSoundPlaying = true;
        audioSource.PlayOneShot(dryFireSound);
        StartCoroutine(waitSound());
    }
    void PullOutWeapon()
    {
        //play draw weapon sound
        //play "pullout wep animation" with playmode.stopall
        //play "pull out animation" by crossfade.
        StartCoroutine(waitPullOut(pullOutWeaponTime));

    }
    IEnumerator waitPullOut(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        isReloading = false;
        weaponSelected = true;
        Debug.Log("Pull out wep called !");
        //enable crosshair 

    }
    void HolsterWeapon()
    {
        Debug.Log("HolsterWeapon called!");
        weaponSelected = false;
    //change field of view.
        //set a bool that crossshair is = false;
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
