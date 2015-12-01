using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {
    //own class variables
    int ammoCarryCap;
    int ammoCarryLeft;
    int ammoMagCap;
    int ammoMagLeft;

    int playerHealthCap;
    float playerHealth;

    void Start() {
        UpdateHud();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
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

    void UpdateHealthHud() { }


    void UpdateAmmoHud() {

    }

}
