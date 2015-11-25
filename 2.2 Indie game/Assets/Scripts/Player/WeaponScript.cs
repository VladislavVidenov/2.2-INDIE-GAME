using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour {

    Camera mainCamera;
    Camera weaponCamera;
    PlayerMovement playerMove;
    [SerializeField]
    GameObject head;
    [SerializeField]
    Transform weaponNozzle;
    Animation weaponAnimations;
    public enum WeaponMode { None, SemiFire, Auto };
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
    bool isShooting = false;
    bool weaponSelected = false;
    bool outOfAmmoSoundPlaying = false;
    bool reloadInfo = false;
    bool reloadInfoStarted = false;

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
    #region Accuracy
    float inaccuracy = 0.05f;

    float minInaccuracy;
    float minInaccuracyHip = 1.5f;
    float minInaccuracyAim = 0.005f;

    float maxInaccuracy;
    float maxInaccuracyHip = 5.0f;
    float maxInaccuracyAim = 1.0f;

    float inaccuracyIncreaseWalk = 0.5f;
    float inaccuracyDecrease = 0.5f;
    float increaseInaccuracy = 0.2f;
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

    //temp crosshair
    public Texture2D crosshairTexture;
    Rect position;
    static bool showCrosshair = true;
    void Awake()
    {
        weaponAnimations = GetComponentInChildren<Animation>();
        playerMove = head.GetComponentInParent<PlayerMovement>();
    }
    void Start() {
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
            Aiming();
            SetInaccuracyRange();
            CalculateInaccuracy();
        }

        //--> Inaccuary system to be implemented.


    }
    void CalculateInaccuracy()
    {
        //if you are running
        if (playerMove.rigidBody.velocity.magnitude > (playerMove.runSpeed - 0.3f))
        {
            Debug.Log("Im runnin brah");
            // inaccuracy += inaccuracyIncreaseRun;      

        } else if (playerMove.rigidBody.velocity.magnitude > (playerMove.walkSpeed - 0.2f))
        {
            inaccuracy += inaccuracyIncreaseWalk;              //^^^so if we decide to change walk speed , inaccuracy doesnt brake. 
            Debug.Log("i AM WALKING BRUH");                    //   veloc.magnituted is always 0.2 less than the speed
        }//else if(playerMove.isCrouching)

        if (isShooting)
        {
            inaccuracy += increaseInaccuracy;
        }
        else
        {   //if we are not shooting and not moving.
            if (playerMove.rigidBody.velocity.magnitude < 1)
                inaccuracy -= inaccuracyDecrease;
        }
      
        inaccuracy = LimitInaccuracy(inaccuracy, minInaccuracy, maxInaccuracy);
        Debug.Log(inaccuracy + "inac");
    }
   
    void SetInaccuracyRange()
    {
        minInaccuracy = isAiming ? minInaccuracyAim : minInaccuracyHip;
        maxInaccuracy = isAiming ? maxInaccuracyAim : maxInaccuracyHip;
    }
    void AdjustFOV()
    {
        if (isAiming)
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
        if (Input.GetMouseButton(1) && weaponSelected && !isReloading)
        {
            playerMove.isRunning = false;
            if (!isAiming)
            {
                isAiming = true;
                aimDistance = Vector3.Distance(aimPosition, transform.localPosition);
            }
            
            if (transform.localPosition != aimPosition)
            {
                AdjustFOV();
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
                playerMove.releasedRun = true;
                isAiming = false;
                aimDistance = Vector3.Distance(defaultPosition, transform.localPosition);
            }
           
            if (transform.localPosition != defaultPosition)
            {
                AdjustFOV();
                if (aimDistance < aimDistance / aimSpeed * aimingDamp)
                {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, defaultPosition, ref velocity, aimingDamp);
                }
            }
        }
    }

    void OnGUI()
    {
        if (showCrosshair)
        {
            if (!isAiming)
            {

                GUI.DrawTexture(position, crosshairTexture);
            }
        }
       

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
        playerMove.isRunning = false;
        isShooting = true;
        Vector3 shootDirection = mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inaccuracy, Random.Range(-0.01f, 0.01f) * inaccuracy, 1));
        RaycastHit hit;
        Debug.DrawRay(mainCamera.transform.position, shootDirection * 100f, Color.green, 5);
        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, 100f))
        {

            Vector3 hitPoint = hit.point;
            Quaternion decalRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (!hit.transform.CompareTag(Tags.enemy))
            {
                GameObject go = Instantiate(normalDecal, hitPoint + (hit.normal * 0.01f), decalRotation) as GameObject;
                go.transform.parent = hit.transform;
            }
        }
        

        audioSource.PlayOneShot(fireSound);
     //   weaponAnimations.Rewind("Shoot");
       // weaponAnimations.Play("Shoot");

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
        playerMove.releasedRun = true;
    //    playerMove.SendMessage("finishedReloading");
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
            playerMove.isRunning = false;
           // playerMove.SendMessage("startedReloading");
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
        weaponAnimations["Draw"].speed = pullOutWeaponTime * 2;
      //  weaponAnimations.Play("Draw", PlayMode.StopAll);
       // weaponAnimations.Play("Draw");
        weaponAnimations.CrossFade("Draw");
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
    float LimitInaccuracy(float value, float min, float max)
    {
        if (value >= max)
            value = max;
        if (value <= min)
            value = min;
        return value;
    }


}
