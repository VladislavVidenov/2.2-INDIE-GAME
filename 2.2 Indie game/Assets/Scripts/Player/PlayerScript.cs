﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    float health;
    float stamina;
    int maxStamina;
    int bits;
    int electronics;

    int bitsBoost;
    int electronicsBoost;

    int screwUpgBoost;
    int hammerUpgBoost;
    int wrenchUpgBoost;

    float healthRegenRate = 100f;
    float healthRegenDelay = 10.0f;
    float timeNotHit;
    float healthRegenAmount;

    float staminaRegenRate = 100f;
    float staminaRegenDelay = 5.0f;
    [HideInInspector] public float timeNotRun; //public to set it in PlayerMovement
    float staminaRegenAmount;

    [SerializeField] Image hp75;
    [SerializeField] Image hp50;
    [SerializeField] Image hp25;
    [SerializeField] Image hitIndicator;

    Camera mainCamera;

    SceneChangeManager sceneManager;

    [Tooltip("Reference to the Bits HUD")]
    [SerializeField] GameObject bitsHud;
    Text bitsText;

    [Tooltip("Reference to the Ingame HUD")]
    [SerializeField] HudScript inGameHud;

    Tool currentTool;

    float cutOffBits;



    // Use this for initialization
    void Start () {
        bitsText = bitsHud.GetComponentInChildren<Text>();
        DeactiveBitsHud();
        mainCamera = Camera.main;
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        GetPlayerStatsFromGameManager();
        UpdateHealthHud();
        UpdateStaminaHud();
    }

    void Update() {
        RegenHealth();
        RegenStamina();
        DeathHud();
    }

    void DeathHud() {
        hp75.color = new Color(1, 1, 1, ((float)maxHealth - (float)health) / 25);
        hp50.color = new Color(1, 1, 1, ((float)75 - (float)health) / 25);
        hp25.color = new Color(1, 1, 1, ((float)50 - (float)health) / 25);
    }

    void RegenHealth() {
        timeNotHit += Time.deltaTime;

        if (timeNotHit > healthRegenDelay) {
            healthRegenAmount += 0.2f;
            if (healthRegenAmount >= 1f) {
                ChangeHealth(1 * healthRegenRate * Time.deltaTime);
                healthRegenAmount = 0;
            }
        }
    }

    void RegenStamina() {
        timeNotRun += Time.deltaTime;
        if (timeNotRun > staminaRegenDelay) {
            staminaRegenAmount += 0.2f;
            if (staminaRegenAmount >= 1f) {
                ChangeStamina(1 * staminaRegenRate * Time.deltaTime);
                staminaRegenAmount = 0;
            }
        }
    }

    public void ChangeHealth(float amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;

        inGameHud.PlayerHealth = health;

        if (health <= 0) { health = 0; inGameHud.PlayerHealth = 0; Died(); }
    }

    public void ChangeStamina(float amount) {
        //Debug.Log("STAMINA: " + stamina);
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;

        inGameHud.PlayerStamina = stamina;

        if (stamina <= 0) { stamina = 0; inGameHud.PlayerStamina = 0; }
    }

    public void ShowHitCircle(RaycastHit hit) {
        Vector3 direction = Camera.main.WorldToScreenPoint(hit.normal);
        print(direction);
        hitIndicator.transform.rotation = Quaternion.Euler(0,0,  Quaternion.LookRotation(hit.normal).z);
    }

    void UpdateHealthHud() {
        inGameHud.PlayerHealth = health;
        inGameHud.PlayerHealthCap = maxHealth;
    }

    void UpdateStaminaHud() {
        inGameHud.PlayerStamina = stamina;
        inGameHud.PlayerStaminaCap = maxStamina;
    }

    public void TakeDamage(int amount){
		ChangeHealth (-amount);
        timeNotHit = 0;
		//show direction
		//knockback?
	}

    public void Died() {
        sceneManager.SetState(GameState.InMenu);
        Application.LoadLevel(0);
    }

    void Respawn() {
        sceneManager.SetState(GameState.InGame);
    }

    public bool HasStamina() {
        if (stamina > 0) return true;
        else return false;
    }
  
    void GetPlayerStatsFromGameManager() {
        maxHealth = GameManager.Instance.maxHealth;
        health = GameManager.Instance.health;
        stamina = GameManager.Instance.stamina;
        maxStamina = GameManager.Instance.maxStamina;
        bits = GameManager.Instance.bits;
        electronics = GameManager.Instance.electronics;
    }

  
    public void GetCurrencyStats( out int pBits,out int pElectronics) {
        pBits = bits;
        pElectronics = electronics;
    }

    public void SetCurrencyStats( int pBits, int pElectronics) {
        bits = pBits;
        electronics = pElectronics;
    }

    public void GetHealthStats(out float pHealth, out int pMaxHealth) {
        pHealth = health;
        pMaxHealth = maxHealth;
    }

    public void SetHealthStats(float pHealth, int pMaxHealth) {
        health = pHealth;
        maxHealth = pMaxHealth;
    }

    public void GetStaminaStats(out float pStamina, out int pMaxStamina) {
        pStamina = stamina;
        pMaxStamina = maxStamina;
    }

    public void SetStaminaStats(float pStamina, int pMaxStamina) {
        stamina = pStamina;
        maxStamina = pMaxStamina;
    }

    public void IncreasePlayerStats(float pHealth, int pMaxHealth, float pStamina, int pMaxStamina, int pBits, int pElectronics) {
        if(pBits>0) IncreaseBitsHud(pBits);
        maxHealth += pMaxHealth;
        health += pHealth;
        stamina += pStamina;
        maxStamina += pMaxStamina;
        bits += pBits + bitsBoost;
        Debug.Log(bits);
        electronics += pElectronics + electronicsBoost;

        UpdateHealthHud();
        UpdateStaminaHud();
    }

    public void SetRegenDelay(float newRegenDelay) {
        healthRegenDelay = newRegenDelay;
    }

    void IncreaseBitsHud(int Amount) {
        if (bitsHud.gameObject.activeInHierarchy) {
            StopAllCoroutines();
            CancelInvoke("DeactiveBitsHud");
            StartCoroutine(IncreaseHud(cutOffBits, Amount + (bits - cutOffBits)));
        }
        else {
            bitsHud.SetActive(true);
            StartCoroutine(IncreaseHud(bits, Amount));
           
        }
    }

    void DeactiveBitsHud() {
        bitsHud.SetActive(false);
    }

    public void SetCurrentTool(Tool tool)
    {
        currentTool = tool;
        SetBoostUpgrades();
    }

    void SetBoostUpgrades()
    {
        switch (currentTool.Type)
        {
            case ToolTypes.Screwdriver:
                electronicsBoost = currentTool.electronicsBoost + screwUpgBoost;
                break;

            case ToolTypes.Hammer:
                bitsBoost = currentTool.bitsBoost + hammerUpgBoost;
                break;

            case ToolTypes.Wrench:
                bitsBoost = currentTool.bitsBoost + wrenchUpgBoost;
                electronicsBoost = currentTool.electronicsBoost + wrenchUpgBoost;
                break;
        }
    }

    IEnumerator IncreaseHud(float text, float Amount) {

        float stepTime = 0.02f;
        float addAmount = (float)Amount / 100;
        
        for (int i = 0; i < 100+1; i++) {
            yield return new WaitForSeconds(stepTime);
            float adding = addAmount * i;
            cutOffBits = (text + Mathf.Floor(adding));
            bitsText.text = cutOffBits.ToString();
            if (i > 60) stepTime = 0.03f;
            if (i > 70) stepTime = 0.04f;
            if (i > 80) stepTime = 0.05f;

            if (i > 99) Invoke("DeactiveBitsHud", 5);
        }
    }

}
