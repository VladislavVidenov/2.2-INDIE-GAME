using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudScript : MonoBehaviour {
    //own class variables
    int ammoCarryCap;
    int ammoCarryLeft;
    int ammoMagCap;
    int ammoMagLeft;

    int playerHealthCap;
    float playerHealth;

    //references to own onscreen items
    [Header("Health Texts")]
    [SerializeField] Text currentHealthText;
    [SerializeField] Text healthCapText;

    [Header("Ammo Texts")]
    [SerializeField] Text ammoMagLeftText;
    [SerializeField] Text ammoMagCapText;
    [SerializeField] Text ammoCarryLeftText;
    [SerializeField] Text ammoCarryCapText;

    void Start() {
        UpdateHud();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            print("ammoCarryCap:    " + ammoCarryCap);
            print("ammoCarryLEft:   " + ammoCarryLeft);
            print("ammoMagCap:      " + ammoMagCap);
            print("ammoMagLeft:     " + ammoMagLeft);
            print("playerHealthCap: " + playerHealthCap);
            print("playerHealth:    " + playerHealth);
        }
    }

    #region "Getters & Setters"
    public int AmmoCarryCap {
        get { return ammoCarryCap; }
        set { ammoCarryCap = value; UpdateAmmoHud(); }
    }

    public int AmmoCarryLeft {
        get { return ammoCarryLeft; }
        set { ammoCarryLeft = value; UpdateAmmoHud(); }
    }

    public int AmmoMagCap {
        get { return ammoMagCap; }
        set { ammoMagCap = value; UpdateAmmoHud(); }
    }

    public int AmmoMagLeft {
        get { return ammoMagLeft; }
        set { ammoMagLeft = value; UpdateAmmoHud(); }
    }

    public int PlayerHealthCap {
        get { return playerHealthCap; }
        set { playerHealthCap = value; UpdateHealthHud(); }
    }

    public float PlayerHealth {
        get { return playerHealth; }
        set { playerHealth = value; UpdateHealthHud(); }
    }
    #endregion "Getters & Setters"


    public void UpdateHud() {
        UpdateHealthHud();
        UpdateAmmoHud();
    }

    void UpdateHealthHud() {
        currentHealthText.text = playerHealth.ToString();
        healthCapText.text = "/" + playerHealthCap.ToString();
    }


    void UpdateAmmoHud() {
        ammoMagLeftText.text = ammoMagLeft.ToString();
        ammoMagCapText.text = "/" + ammoMagCap.ToString();
        
        ammoCarryLeftText.text = ammoCarryLeft.ToString();
        ammoCarryCapText.text = "/" + ammoCarryCap.ToString();
    }

}
