using UnityEngine;
using System.Collections;
/// <summary>
/// This script is attached to the main gameobject of the gun.
/// Used [Serialize Field] on what is needed to be edited in the inspector and not accessible from other classes.
/// Public is used on some variables that are going to be upgraded throughout the game.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour { 
    public delegate void PistolShoot();
    public static event PistolShoot OnPistolShoot;

    Camera mainCamera;
    Camera weaponCamera;
    PlayerMovement playerMove;

    [Header("References")]
    [SerializeField] GameObject head;
    [SerializeField] Transform muzzleTransform;
    [SerializeField] HudScript hud;

    [HideInInspector] public Animator animator;
    Animation reloadAnimation;
        
    public enum Weapons { Pistol, Shotgun };

    [Header("Type & Mode")]
    public Weapons weapon;

    public enum WeaponMode { None, SemiFire, Auto };
    public WeaponMode currentWeaponMode;

    [Header("Weapon Properties")]
    private Vector3 hitPoint;
    public int ammoInClip;
    public int maxAmmoInClip;
    public int totalAmmo;
    public int maxTotalAmmo;

    [Header("Properties: General")]
    public float shotgunPelletsPerShot = 5;
    public float weaponRange;
    public int damage;
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
    [Header("Properties: Aiming")]
    [SerializeField] Vector3 aimPosition;
    float aimDistance;
    bool isAiming = false;
    float aimSpeed = 0;
    #endregion

    #region Field of View
    private Vector3 velocityRef = Vector3.zero;
    public float aimingDamp = 0.2f; //made public for upgrades
    public float fovDamp = 0.1f;    //made public for upgrades
    private float fovVel = 0;
    [Header("Properties: FOV")]
    [SerializeField] float aimingFOV;
    [SerializeField] float normalFOV;
    #endregion

    #region INACCURACY
    float inaccuracy = 0.05f;
    float minInaccuracy;
    [Header("Properties: Accuracy")]
    [SerializeField] float minInaccStandHip = 5f;     //STAND + HIP 
    [SerializeField] float minInaccCrouchHip = 4.5f;    //CROUCH + HIP
    [SerializeField] float minInaccStandAim = 1.3f;    //STAND + AIM  
    [SerializeField] float minInaccCrouchAim = 1f;  //CROUCH +AIM

    float maxInaccuracy;
    [SerializeField] float maxInaccStandHip = 6f; //STAND + WALK + HIP
    [SerializeField] float maxInaccCrouchHip = 5.0f;//CROUCH + WALK + HIP
    [SerializeField] float maxInaccStandAim = 3f; //STAND + AIM+ WALK
    [SerializeField] float maxInaccCrouchAim = 1.5f; //CROUCH + AIM + WALK
    #endregion

    #region Sounds
    AudioSource audioSource;
    [Header("Sound")]
    [SerializeField] AudioClip dryFireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip pullOutSound;
    #endregion

    #region Shoot Decals
    [Header("Decals")]
    [SerializeField] GameObject normalDecal;
    [SerializeField] GameObject muzzleParticlePistol;
    #endregion

    [Header("Crosshair (temp)")]
    public Texture2D crosshairTexture;
    Rect crosshairPos;
    static bool showCrosshair = true;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMove = head.GetComponentInParent<PlayerMovement>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        weaponCamera = GameObject.FindGameObjectWithTag(Tags.weaponCamera).GetComponent<Camera>();
        reloadAnimation = GetComponentInParent<Animation>();

        crosshairPos = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        UpdateHudValues();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.J))
        {
            print("ammoInClip;" + ammoInClip);
            print("maxAmmoInClip;" + maxAmmoInClip);
            print("totalAmmo;" + totalAmmo);
            print("maxTotalAmmo;" + maxTotalAmmo);
        }


        if (weaponSelected)
        {
            Aiming();
            SetInaccuracyRange();
            CalculateInaccuracy();

            if (Input.GetButtonDown("Fire"))
                if (weapon == Weapons.Pistol)
                {
                    if (currentWeaponMode == WeaponMode.SemiFire)
                        SemiFirePistol();
                    else if (Input.GetButton("Fire"))
                        if (currentWeaponMode == WeaponMode.Auto)
                            SemiFirePistol();
                }
                else if (weapon == Weapons.Shotgun)
                {
                    if (currentWeaponMode == WeaponMode.SemiFire)
                        SemiFireShotgun();
                    else if (Input.GetButton("Fire"))
                        if (currentWeaponMode == WeaponMode.Auto)
                            SemiFireShotgun();
                }
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }
    }

    public void UpgradeTotalAmmo(int amount)
    {
        maxTotalAmmo += amount;
    }

    public void UpgradeReloadTime(float amount)
    {
        reloadTime += amount;
    }

    public void UpgradeDamage(int amount)
    {
        damage += amount;
    }

    public void IncreaseTotalAmmo(int amount)
    {
        totalAmmo += amount;
        if (totalAmmo >= maxTotalAmmo)
            totalAmmo = maxTotalAmmo;
    }

    void CalculateInaccuracy()
    {
        inaccuracy = playerMove.isWalking() ? maxInaccuracy : minInaccuracy;
    }

    void SetInaccuracyRange()
    {
        minInaccuracy = isAiming ? playerMove.isCrouching ? minInaccCrouchAim : minInaccStandAim : playerMove.isCrouching ? minInaccCrouchHip : minInaccStandHip;
        maxInaccuracy = isAiming ? playerMove.isCrouching ? maxInaccCrouchAim : maxInaccStandAim : playerMove.isCrouching ? maxInaccCrouchHip : maxInaccStandHip;
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
                playerMove.SendMessage("SetAim", isAiming);
                aimDistance = Vector3.Distance(aimPosition, transform.localPosition);
            }

            if (transform.localPosition != aimPosition)
            {
                AdjustFOV();
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, aimPosition, ref velocityRef, aimingDamp);
            }
        }
        else
        {
            if (isAiming)
            {
                playerMove.releasedRun = true;
                isAiming = false;
                playerMove.SendMessage("SetAim", isAiming);
                aimDistance = Vector3.Distance(defaultPosition, transform.localPosition);
            }

            if (transform.localPosition != defaultPosition)
            {
                AdjustFOV();
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, defaultPosition, ref velocityRef, aimingDamp);
            }
        }
    }

    void OnGUI()
    {
        if (showCrosshair)
        {
            if (!isAiming)
            {
                GUI.DrawTexture(crosshairPos, crosshairTexture);
            }
        }

        GUI.contentColor = Color.red;
        GUI.Label(new Rect(10, 10, 100, 50), "mag" + ammoInClip);
        GUI.Label(new Rect(10, 25, 100, 50), "total" + totalAmmo);
        if (reloadInfo) GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 50), "You have no bullets left");
    }

    IEnumerator waitSound()
    {
        yield return new WaitForSeconds(0.2f);
        outOfAmmoSoundPlaying = false;
    }

    void SemiFireShotgun()
    {

        if (isReloading || ammoInClip <= 0)
        {
            if (ammoInClip <= 0)
            {
                DryFire();
            }
            return;
        }

        if (CanFire())
        {
            float pelletsCount = 0;
            while (pelletsCount < shotgunPelletsPerShot)
            {
                FireShotgunPellets();
                pelletsCount++;
            }
            animator.Play("Shoot", 0, 0);
            audioSource.PlayOneShot(fireSound);
            RecoilEffect();
            ammoInClip--;
            UpdateHudValues();
        }
    }

    void FireShotgunPellets()
    {
        playerMove.isRunning = false;
        isShooting = true;

        Vector3 shootDirection = mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inaccuracy, Random.Range(-0.01f, 0.01f) * inaccuracy, 1));
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, weaponRange))
        {

            Debug.DrawRay(mainCamera.transform.position, shootDirection * Vector3.Distance(mainCamera.transform.position, hit.point), Color.red, 5f);
            hitPoint = hit.point;

            switch (hit.transform.gameObject.tag)
            {
                case Tags.enemy:
                    hit.transform.GetComponentInParent<EnemyScript>().TakeDamage(damage);
                    break;
            }


            //Quaternion decalRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            //GameObject go = Instantiate(normalDecal, hitPoint + (hit.normal * 0.1f), decalRotation) as GameObject;
            //go.transform.parent = hit.transform;
            GameObject go = Instantiate(normalDecal, hitPoint, Quaternion.identity) as GameObject;
            Destroy(go, 2);
        }
    }

    void SemiFirePistol()
    {//If we are currently reloading / or we ran out of ammo -> return and play dry fire sound:).
        if (isReloading || ammoInClip <= 0)
        {
            if (ammoInClip <= 0)
            {
                DryFire();
            }
            return;
        }

        if (CanFire()) FireOneBullet();
    }

    void FireOneBullet()
    {
        if (OnPistolShoot != null) OnPistolShoot();
        playerMove.isRunning = false;
        isShooting = true;
        Vector3 shootDirection = mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inaccuracy, Random.Range(-0.01f, 0.01f) * inaccuracy, 1));
        RaycastHit hit;
        // Debug.DrawRay(mainCamera.transform.position, shootDirection * 100f, Color.green, 5);
        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, weaponRange))
        {
            Debug.DrawRay(mainCamera.transform.position, shootDirection * Vector3.Distance(mainCamera.transform.position, hit.point), Color.red, 5f);
            hitPoint = hit.point;
            GameObject muzzle = Instantiate(muzzleParticlePistol, muzzleTransform.position, Quaternion.identity) as GameObject;
            Destroy(muzzle, 1);

            switch (hit.transform.gameObject.tag)
            {
                case Tags.enemy:
                    hit.transform.GetComponent<EnemyScript>().TakeDamage(damage);
                    break;
            }

            Quaternion decalRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject go = Instantiate(normalDecal, hitPoint + (hit.normal * 0.1f), decalRotation) as GameObject;
            go.transform.parent = hit.transform;
            Destroy(go, 2);
        }

        animator.Play("Shoot", 0, 0);

        audioSource.PlayOneShot(fireSound);

        RecoilEffect();
        ammoInClip--;
        UpdateHudValues();
    }

    IEnumerator shotAnim(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Shot2", false);
    }

    #region Reloading 
    IEnumerator ReloadTime(float time)
    {
        yield return new WaitForSeconds(time);
        isReloading = false;
        playerMove.SendMessage("SetIsReloading", isReloading);
        int bulletsShot = maxAmmoInClip - ammoInClip;
        int tBCcopy = totalAmmo;
        totalAmmo -= bulletsShot;
        if (totalAmmo < 0) totalAmmo = 0;
        int delta = tBCcopy - totalAmmo;
        ammoInClip += delta;


        playerMove.releasedRun = true;//if player still holds run button during reload => start running again.
        UpdateHudValues();
    }

    IEnumerator shutReloadInfo()
    {
        yield return new WaitForSeconds(2f);
        reloadInfo = false;
        reloadInfoStarted = false;
    }

    public void UpdateHudValues()
    {
        hud.AmmoMagLeft = ammoInClip;
        hud.AmmoMagCap = maxAmmoInClip;
        hud.AmmoCarryLeft = totalAmmo;
        hud.AmmoCarryCap = maxTotalAmmo;
    }

    void Reload()
    {
        //if we are already reloading or we are already on full ammo in mag-> return;
        if (isReloading || ammoInClip == maxAmmoInClip) return;
        reloadInfo = (totalAmmo <= 0) ? true : false;
        if (reloadInfo && !reloadInfoStarted) { reloadInfoStarted = true; StartCoroutine(shutReloadInfo()); }

        float reloadSpeedFactor = 2.66666666f;  // This is the time the animation should play for a 1 second reloadTime.
        if (ammoInClip >= 0 && totalAmmo > 0)
        {
            isReloading = true;
            playerMove.SendMessage("SetIsReloading", isReloading); // tell the movement 
            playerMove.isRunning = false;//so we stop running if we run and reload.
            reloadAnimation["AllReload"].speed = reloadSpeedFactor / reloadTime;
            reloadAnimation.CrossFade("AllReload");
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
        animator.SetBool("PullOutWeapon", true);
        StartCoroutine(waitPullOut(pullOutWeaponTime));

    }

    IEnumerator waitPullOut(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        isReloading = false;
        animator.SetBool("PullOutWeapon", false);
        weaponSelected = true;
        showCrosshair = true;
    }

    void HolsterWeapon()
    {
        //  Debug.Log("HolsterWeapon called!");
        weaponSelected = false;
        showCrosshair = false;
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
