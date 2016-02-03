﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudScript : MonoBehaviour {
    //own class variables
    int ammoCarryCap;
    int ammoCarryLeft;
    int ammoMagCap;
    int ammoMagLeft;

    int playerHealthCap;
    float playerHealth, previousHealth;
    int playerStaminaCap;
    float playerStamina;

    bool showCrosshair;

    //references to own onscreen items
    [Header("Health")]
    [SerializeField] Text currentHealthText;
    [SerializeField] Text healthCapText;
    [SerializeField] Image hBarBg, hBarBorder, hBarFill;

    [Header("Stamina")]
    [SerializeField] Text currentStaminaText;
    [SerializeField] Text staminaCapText;
    [SerializeField] Image sBarBg, sBarBorder, sBarFill;

    [Header("Ammo")]
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
            print("playerStaminaCap: " + playerStaminaCap);
            print("playerStamina:    " + playerStamina);
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
        set { previousHealth = playerHealth; playerHealth = value; UpdateHealthHud(); }
    }

    public int PlayerStaminaCap {
        get { return playerStaminaCap; }
        set { playerStaminaCap = value; UpdateStaminaHud(); }
    }

    public float PlayerStamina {
        get { return playerStamina; }
        set { playerStamina = value; UpdateStaminaHud(); }
    }

    public bool ShowCrosshair {
        get { return showCrosshair; }
        set { showCrosshair = value; }
    }
    #endregion "Getters & Setters"

    public void UpdateHud() {
        UpdateHealthHud();
        UpdateAmmoHud();
        UpdateStaminaHud();
    }

    void UpdateHealthHud() {
        currentHealthText.text = Mathf.Floor(playerHealth).ToString();
        healthCapText.text = "/" + playerHealthCap.ToString();

        //update the bars
        hBarFill.fillAmount = (playerHealth / playerHealthCap);

    }

    void UpdateAmmoHud() {
        ammoMagLeftText.text = ammoMagLeft.ToString();
        ammoMagCapText.text = "/" + ammoMagCap.ToString();
        
        ammoCarryLeftText.text = ammoCarryLeft.ToString();
        ammoCarryCapText.text = "/" + ammoCarryCap.ToString();
    }

    void UpdateStaminaHud() {
        currentStaminaText.text = Mathf.Floor(playerStamina).ToString();
        staminaCapText.text = "/" + playerStaminaCap.ToString();

        //update the bars
        sBarFill.fillAmount = (playerStamina / playerStaminaCap);
    }
}
