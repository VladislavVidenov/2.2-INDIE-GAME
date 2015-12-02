using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour {

    Camera mainCamera;
    Camera weaponCamera;
    PlayerMovement playerMove;
    [SerializeField] GameObject head;
    [SerializeField] Transform muzzleTransform;
    [SerializeField] HudScript inGameHud;

    [HideInInspector] public Animator animator;
    Animation reloadAnimation;

    public enum Weapons { Pistol,Shotgun };
    public Weapons weapon;

    public enum WeaponMode { None, SemiFire, Auto };
    public WeaponMode currentWeaponMode;

    //Weapon properties.
    Vector3 hitPoint;
    public int ammoInClip;
    public int maxAmmoInClip;
    public int totalAmmo;
    public int maxTotalAmmo;
   

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

    //Used for shotgun
    float shotgunPelletsPerShot = 5;


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
    private float fovDamp = 0.1f;
    private float fovVel = 0;
    [SerializeField] float aimingFOV;
    [SerializeField] float normalFOV;
    #endregion

    #region Accuracy
    float inaccuracy = 0.05f;

    float minInaccuracy;
    float minInaccuracyStandHip = 5f;     //STAND + HIP 
    float minInaccuracyCrouchHip = 4.5f;    //CROUCH + HIP
    float minInaccuracyStandAim = 1.3f;    //STAND + AIM  
    float minInaccuracyCrouchAim = 1f;  //CROUCH +AIM

    float maxInaccuracy;
    float maxInaccuracyStandHip = 6f; //STAND + WALK + HIP
    float maxInaccuracyCrouchHip = 5.0f;//CROUCH + WALK + HIP
    float maxInaccuracyStandAim = 3f; //STAND + AIM+ WALK
    float maxInaccuracyCrouchAim = 1.5f; //CROUCH + AIM + WALK
    #endregion

    #region Sounds
    AudioSource audioSource;
    [SerializeField] AudioClip dryFireSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip pullOutSound;
    #endregion

    #region Shoot Decals
    [SerializeField] GameObject normalDecal;
    [SerializeField] GameObject muzzleParticlePistol;
    #endregion

    //temp crosshair
    public Texture2D crosshairTexture;
    Rect crosshairPos;
    static bool showCrosshair = true;

    void Awake() {
        animator = GetComponentInChildren<Animator>();
        playerMove = head.GetComponentInParent<PlayerMovement>();
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        weaponCamera = GameObject.FindGameObjectWithTag(Tags.weaponCamera).GetComponent<Camera>();
        reloadAnimation = GetComponentInParent<Animation>();

        crosshairPos = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
        // GetAmmoFromManager(weapon);
        UpdateHudValues();
    }
  
    void Update() {
        if (Input.GetKey(KeyCode.J)) {
            print("ammoInClip;"     + ammoInClip);
            print("maxAmmoInClip;"  +maxAmmoInClip);
            print("totalAmmo;"      +totalAmmo);
            print("maxTotalAmmo;"   +maxTotalAmmo);


        }


        if (weaponSelected) {
            Aiming();
            SetInaccuracyRange();
            CalculateInaccuracy();

            if (Input.GetButtonDown("Fire"))
                if (weapon == Weapons.Pistol) {
                    if (currentWeaponMode == WeaponMode.SemiFire)
                        SemiFirePistol();
                    else if (Input.GetButton("Fire"))
                        if (currentWeaponMode == WeaponMode.Auto)
                            SemiFirePistol();
                }
                else if (weapon == Weapons.Shotgun) {
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
    void CalculateInaccuracy() {
        inaccuracy = playerMove.isWalking() ? maxInaccuracy : minInaccuracy;
    }

    void SetInaccuracyRange() {
        minInaccuracy = isAiming ? playerMove.isCrouching ? minInaccuracyCrouchAim : minInaccuracyStandAim : playerMove.isCrouching ? minInaccuracyCrouchHip : minInaccuracyStandHip;
        maxInaccuracy = isAiming ? playerMove.isCrouching ? maxInaccuracyCrouchAim : maxInaccuracyStandAim : playerMove.isCrouching ? maxInaccuracyCrouchHip : maxInaccuracyStandHip;
    }

    void AdjustFOV() {
        if (isAiming) {
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, aimingFOV, ref fovVel, fovDamp);
            if (mainCamera.fieldOfView < aimingFOV)
                mainCamera.fieldOfView = aimingFOV;
            weaponCamera.fieldOfView = Mathf.SmoothDamp(weaponCamera.fieldOfView, aimingFOV, ref fovVel, fovDamp);
            if (weaponCamera.fieldOfView < aimingFOV)
                weaponCamera.fieldOfView = aimingFOV;
        }
        else {
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, normalFOV, ref fovVel, fovDamp);
            if (mainCamera.fieldOfView > normalFOV)
                mainCamera.fieldOfView = normalFOV;
            weaponCamera.fieldOfView = Mathf.SmoothDamp(weaponCamera.fieldOfView, normalFOV, ref fovVel, fovDamp);
            if (weaponCamera.fieldOfView > normalFOV)
                weaponCamera.fieldOfView = normalFOV;
        }
    }
    void Aiming() {
        if (Input.GetMouseButton(1) && weaponSelected && !isReloading) {
            playerMove.isRunning = false;

            if (!isAiming) {
                isAiming = true;
                playerMove.SendMessage("SetAim", isAiming);
                aimDistance = Vector3.Distance(aimPosition, transform.localPosition);
            }

            if (transform.localPosition != aimPosition) {
                AdjustFOV();
                if (aimDistance < aimDistance / aimSpeed * aimingDamp) {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, aimPosition, ref velocity, aimingDamp);
                }
            }
        }
        else {
            if (isAiming) {
                playerMove.releasedRun = true;
                isAiming = false;
                playerMove.SendMessage("SetAim", isAiming);
                aimDistance = Vector3.Distance(defaultPosition, transform.localPosition);
            }

            if (transform.localPosition != defaultPosition) {
                AdjustFOV();
                if (aimDistance < aimDistance / aimSpeed * aimingDamp) {
                    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, defaultPosition, ref velocity, aimingDamp);
                }
            }
        }
    }

    void OnGUI() {
        if (showCrosshair) {
            if (!isAiming) {
                GUI.DrawTexture(crosshairPos, crosshairTexture);
            }
        }

        GUI.contentColor = Color.red;
        GUI.Label(new Rect(10, 10, 100, 50), "mag" + ammoInClip);
        GUI.Label(new Rect(10, 25, 100, 50), "total" + totalAmmo);
        if (reloadInfo) GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 50), "You have no bullets left");
    }

    IEnumerator waitSound() {
        yield return new WaitForSeconds(0.2f);
        outOfAmmoSoundPlaying = false;
    }

    void SemiFireShotgun() {
        if (isReloading || ammoInClip <= 0) {
            if (ammoInClip <= 0) {
                DryFire();
            }
            return;
        }

        if (CanFire()) {
            float test = 0;
            while (test < shotgunPelletsPerShot) {
                FireShotGun();
                test++;
            }

            audioSource.PlayOneShot(fireSound);

            RecoilEffect();
            ammoInClip--;
        }
    }

    void FireShotGun() {
        playerMove.isRunning = false;
        isShooting = true;
        Vector3 shootDirection = mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inaccuracy, Random.Range(-0.01f, 0.01f) * inaccuracy, 1));
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, 100f))
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


        }
    }

    void SemiFirePistol() {//If we are currently reloading / or we ran out of ammo -> return and play dry fire sound:).
        if (isReloading || ammoInClip <= 0) {
            if (ammoInClip <= 0) {
                DryFire();
            }
            return;
        }

        if (CanFire()) FireOneBullet();
    }

    void FireOneBullet() {
        playerMove.isRunning = false;
        isShooting = true;
        Vector3 shootDirection = mainCamera.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * inaccuracy, Random.Range(-0.01f, 0.01f) * inaccuracy, 1));
        RaycastHit hit;
        // Debug.DrawRay(mainCamera.transform.position, shootDirection * 100f, Color.green, 5);
        if (Physics.Raycast(mainCamera.transform.position, shootDirection, out hit, 100f)) {
            Debug.DrawRay(mainCamera.transform.position, shootDirection * Vector3.Distance(mainCamera.transform.position, hit.point), Color.red, 5f);
            hitPoint = hit.point;
            GameObject muzzle = Instantiate(muzzleParticlePistol, muzzleTransform.position, Quaternion.identity) as GameObject;
            Destroy(muzzle, 1);

            switch (hit.transform.gameObject.tag) {
                case Tags.enemy:
                    hit.transform.GetComponent<EnemyScript>().TakeDamage(damage);
                    Debug.Log("Hit");
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

    IEnumerator shotAnim(float time) {
        yield return new WaitForSeconds(time);
        animator.SetBool("Shot2", false);
    }

    #region Reloading 
    IEnumerator ReloadTime(float time) {
        yield return new WaitForSeconds(time);
        playerMove.SendMessage("SetIsReloading", isReloading);
        int bulletsShot = maxAmmoInClip - ammoInClip;
        int tBCcopy = totalAmmo;
        totalAmmo -= bulletsShot;
        if (totalAmmo < 0) totalAmmo = 0;
        int delta = tBCcopy - totalAmmo;
        ammoInClip += delta;
        isReloading = false;
      
        playerMove.releasedRun = true;//if player still holds run button during reload => start running again.
        UpdateHudValues();
    }

    IEnumerator shutReloadInfo() {
        yield return new WaitForSeconds(2f);
        reloadInfo = false;
        reloadInfoStarted = false;
    }

    void UpdateHudValues() {
        inGameHud.AmmoMagLeft = ammoInClip;
        inGameHud.AmmoMagCap = maxAmmoInClip;
        inGameHud.AmmoCarryLeft = totalAmmo;
        inGameHud.AmmoCarryCap = maxTotalAmmo;
    }

    void Reload() {
        //if we are already reloading or we are already on full ammo in mag-> return;
        if (isReloading || ammoInClip == maxAmmoInClip) return;
        reloadInfo = (totalAmmo <= 0) ? true : false;
        if (reloadInfo && !reloadInfoStarted) { reloadInfoStarted = true; StartCoroutine(shutReloadInfo()); }

        if (ammoInClip >= 0 && totalAmmo > 0) {
            isReloading = true;
            playerMove.SendMessage("SetIsReloading", isReloading); // tell the movement 
            playerMove.isRunning = false;//so we stop running if we run and reload.
            reloadAnimation["PistolReload"].speed = reloadTime / 1.5f;
            reloadAnimation.CrossFade("PistolReload");
            audioSource.PlayOneShot(reloadSound);

            StartCoroutine(ReloadTime(reloadTime));
        }
    }

    #endregion
    void DryFire() {
        if (isReloading || outOfAmmoSoundPlaying) return;
        outOfAmmoSoundPlaying = true;
        audioSource.PlayOneShot(dryFireSound);
        StartCoroutine(waitSound());
    }

    void PullOutWeapon() {
        animator.SetBool("PullOutWeapon", true);
        StartCoroutine(waitPullOut(pullOutWeaponTime));

    }

    IEnumerator waitPullOut(float waittime) {
        yield return new WaitForSeconds(waittime);
        isReloading = false;
        animator.SetBool("PullOutWeapon", false);
        weaponSelected = true;
        showCrosshair = true;
        inGameHud.ShowCrosshair = true;
    }

    void HolsterWeapon() {
        //  Debug.Log("HolsterWeapon called!");
        weaponSelected = false;
        showCrosshair = false;
        inGameHud.ShowCrosshair = false;
    }

    void RecoilEffect() {
        head.transform.localRotation = Quaternion.Euler(head.transform.localRotation.eulerAngles - new Vector3(1, Random.Range(-1, 1), 0));
    }

    bool CanFire() {
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time) {
            nextFireTime = Time.time + fireRate;
            return true;
        }
        return false;
    }

    float LimitInaccuracy(float value, float min, float max) {
        if (value >= max)
            value = max;
        if (value <= min)
            value = min;
        return value;
    }
}
