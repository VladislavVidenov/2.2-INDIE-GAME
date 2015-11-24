using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour {

    Camera mainCamera;
    Camera weaponCamera;
    [SerializeField]
    GameObject head;
    public enum WeaponMode { None,SemiFire,Auto };
    public WeaponMode currentWeaponMode;

    //Weapon properties.
    public int bulletsInClip;
    public int totalBullets;
    int maxBulletsPerMag;

    public float damage;
    public float fireRate;
    public float reloadTime;
    public float pullOutWeaponTime;
    float nextFireTime;
    bool isReloading = false;

    bool weaponSelected = false;
    bool outOfAmmoSoundPlaying = false;
    bool reloadInfo = false;
    bool reloadInfoStarted = false;
    // bool isFiring = false;

    #region Aiming
    Vector3 defaultPosition = Vector3.zero;
    public Vector3 aimPosition;
    float aimDistance;
    bool isAiming = false;
    float aimSpeed = 0;
    bool animationRun = false;
    #endregion

    #region Field of View
    private Vector3 velocity = Vector3.zero;
    private float aimingDamp = 0.2f;
    private float fovDamp = 0.2f;
    private float fovVel = 0;
    [SerializeField]
    float aimingFOV;
    [SerializeField]
    float normalFOV;
    #endregion

    #region Sounds
    AudioSource audioSource;
    [SerializeField] AudioClip dryFireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip pullOutSound;
    #endregion

    #region Shoot Decals
    [SerializeField]
    GameObject normalDecal;
    #endregion


    public Texture2D crosshairTexture;
    Rect position;
    static bool OriginalOn = true;

    void Start () {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        weaponCamera = GameObject.FindGameObjectWithTag(Tags.weaponCamera).GetComponent<Camera>();
        maxBulletsPerMag = bulletsInClip;

        position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
    }


    void Update()
    {
        // Debug.Log(isReloading);
        if (weaponSelected)
        {
            if (Input.GetButtonDown("Fire"))
            {

                if (currentWeaponMode == WeaponMode.SemiFire)
                {
                    SemiFireMode();
                }

            }
            else if (Input.GetButton("Fire"))
            {
                if (currentWeaponMode == WeaponMode.Auto)
                {
                    SemiFireMode();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
           
        }
        Aiming();
        //--> Inaccuary system to be implemented.


    }

    void AdjustFOV(bool aiming)
    {
        if (aiming)
        {
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, aimingFOV, ref fovVel, fovDamp);
            if (mainCamera.fieldOfView < aimingFOV)
                mainCamera.fieldOfView = aimingFOV;
            weaponCamera.fieldOfView = Mathf.SmoothDamp(weaponCamera.fieldOfView, aimingFOV, ref fovVel, fovDamp);
            if (weaponCamera.fieldOfView < aimingFOV)
                weaponCamera.fieldOfView = aimingFOV;
        }
        else
        {
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, normalFOV, ref fovVel, fovDamp);
            if (mainCamera.fieldOfView > normalFOV)
                mainCamera.fieldOfView = normalFOV;
            weaponCamera.fieldOfView = Mathf.SmoothDamp(weaponCamera.fieldOfView, normalFOV, ref fovVel, fovDamp);
            if (weaponCamera.fieldOfView > normalFOV)
                weaponCamera.fieldOfView = normalFOV;
        }
    }
    void Aiming()
    {
        if (Input.GetMouseButton(1) && weaponSelected && !isReloading && !Input.GetKey(KeyCode.LeftShift))
        {
            if (!isAiming)
            {
                isAiming = true;
                aimDistance = Vector3.Distance(aimPosition, transform.localPosition);
            }
            
            if (transform.localPosition != aimPosition)
            {
                AdjustFOV(isAiming);
                if (aimDistance < aimDistance / aimSpeed * aimingDamp)
                {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, aimPosition, ref velocity, aimingDamp);
                }
            }
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
                aimDistance = Vector3.Distance(defaultPosition, transform.localPosition);
            }
           
            if (transform.localPosition != defaultPosition)
            {
                AdjustFOV(isAiming);
                if (aimDistance < aimDistance / aimSpeed * aimingDamp)
                {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, defaultPosition, ref velocity, aimingDamp);
                }
            }
        }
    }

    void OnGUI()
    {
        if (OriginalOn == true)
            GUI.DrawTexture(position, crosshairTexture);

        GUI.contentColor = Color.red;
        GUI.Label(new Rect(10, 10, 100, 50), "mag" + bulletsInClip);
        GUI.Label(new Rect(10, 25, 100, 50), "total" + totalBullets);
        if (reloadInfo) GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 50), "You have no bullets left");
    }
    IEnumerator waitSound()
    {
        yield return new WaitForSeconds(0.2f);
        outOfAmmoSoundPlaying = false;
    }
 
    void SemiFireMode()
    {//If we are currently reloading / or we ran out of ammo -> return and play dry fire sound:).
        if (isReloading || bulletsInClip <= 0)
        {
            if (bulletsInClip <= 0)
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
           // Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.CompareTag("Ground"))
            {
            
                GameObject go = Instantiate(normalDecal,new Vector3(hitPoint.x,hitPoint.y + 0.00001f,hitPoint.z),hitPointDecalPos) as GameObject;
                go.transform.parent = hit.transform;
            }
        }

        audioSource.PlayOneShot(fireSound);
        //play animations
      //  Debug.Log("Shooting");
        //apply kickback affect
        RecoilEffect();
        bulletsInClip--;

    }




    #region Reloading 
    IEnumerator ReloadTime(float time)
    {
        yield return new WaitForSeconds(time);
        int bulletsShot = maxBulletsPerMag - bulletsInClip;
        int tBCcopy = totalBullets;
        totalBullets -= bulletsShot;
        if (totalBullets < 0) totalBullets = 0;
        int delta = tBCcopy - totalBullets;
        bulletsInClip += delta;
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
        if (isReloading || bulletsInClip == maxBulletsPerMag) return;
        reloadInfo = (totalBullets <= 0) ? true : false;
        if (reloadInfo && !reloadInfoStarted) { reloadInfoStarted = true; StartCoroutine(shutReloadInfo()); }

        if (bulletsInClip >= 0 && totalBullets > 0)
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
      //  Debug.Log("Pull out wep called !");
        //enable crosshair 

    }
    void HolsterWeapon()
    {
      //  Debug.Log("HolsterWeapon called!");
        weaponSelected = false;
    //change field of view.
        //set a bool that crossshair is = false;
    }
    void RecoilEffect()
    {
        head.transform.localRotation = Quaternion.Euler(head.transform.localRotation.eulerAngles - new Vector3(1, Random.Range(-1, 1), 0));
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
