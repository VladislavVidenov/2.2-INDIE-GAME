﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    int health;
    int scrap;
    int electronics;

    int scrapBoost;
    int electronicsBoost;

    int screwUpgBoost;
    int hammerUpgBoost;
    int wrenchUpgBoost;

    int regenRate = 5;
    float timeNotHit;
    float regenAmount;

    [SerializeField] Image hp75;
    [SerializeField] Image hp50;
    [SerializeField] Image hp25;
    [SerializeField] Image hitIndicator;


    Camera mainCamera;

    SceneChangeManager sceneManager;

    [Tooltip("Reference to the Scrap HUD")]
    [SerializeField] GameObject scrapHud;
    Text scrapText;

    [Tooltip("Reference to the Ingame HUD")]
    [SerializeField] HudScript inGameHud;

    Tool currentTool;

    float cutOffScrap;
	// Use this for initialization
	void Start () {
        scrapText = scrapHud.GetComponentInChildren<Text>();
        DeactiveScrapHud();
        mainCamera = Camera.main;
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        GetPlayerStatsFromGameManager();
        UpdateHealthHud();
    }

    void Update() {
        RegenLife();
        DeathHud();
    }

    void DeathHud() {
        hp75.color = new Color(1, 1, 1, ((float)maxHealth - (float)health) / 25);
        hp50.color = new Color(1, 1, 1, ((float)75 - (float)health) / 25);
        hp25.color = new Color(1, 1, 1, ((float)50 - (float)health) / 25);
    }

    void RegenLife() {
        timeNotHit += Time.deltaTime;

        if (timeNotHit > 2) {
            regenAmount += 0.2f;
            if (regenAmount >= 1f) {
                ChangeHealth(1);
                regenAmount = 0;
            }
        }

    }

    public void ChangeHealth(int amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;

        inGameHud.PlayerHealth = health;

        //if (health <= 0) { health = 0; Died(); }
    }

    public void ShowHitCircle(RaycastHit hit) {
    //    hit.no

        Vector3 direction = Camera.main.WorldToScreenPoint(hit.normal);
        print(direction);
        hitIndicator.transform.rotation = Quaternion.Euler(0,0,  Quaternion.LookRotation(hit.normal).z);
        
    }


    void UpdateHealthHud() {
        inGameHud.PlayerHealth = health;
        inGameHud.PlayerHealthCap = maxHealth;
    }

    public void TakeDamage(int amount){
		ChangeHealth (-amount);
        timeNotHit = 0;
		// show direction
		//knockback?
	}

    public void Died() {
        sceneManager.SetState(GameState.PlayerDied);
        Invoke("Respawn", 2);
    }

    void Respawn() {
        sceneManager.SetState(GameState.InGame);
    }

  
    void GetPlayerStatsFromGameManager() {
        maxHealth = GameManager.Instance.maxHealth;
        health = GameManager.Instance.health;
        scrap = GameManager.Instance.scrap;
        electronics = GameManager.Instance.electronics;
    }

  
    public void GetCurrencyStats( out int pScrap,out int pElectronics) {
        pScrap = scrap;
        pElectronics = electronics;
    }

    public void SetCurrencyStats( int pScrap, int pElectronics) {
        scrap = pScrap;
        electronics = pElectronics;
    }

    public void GetHealthStats(out int pHealth, out int pMaxHealth) {
        pHealth = health;
        pMaxHealth = maxHealth;
    }

    public void SetHealthStats(int pHealth, int pMaxHealth) {
        health = pHealth;
        maxHealth = pMaxHealth;
    }

    public void IncreasePlayerStats(int pHealth, int pMaxHealth, int pScrap, int pElectronics) {
        if(pScrap>0) IncreaseScrapHud(pScrap);
        maxHealth += pMaxHealth;
        health += pHealth;
        scrap += pScrap + scrapBoost;
        Debug.Log(scrap);
        electronics += pElectronics + electronicsBoost;
    }

    void IncreaseScrapHud(int Amount) {
        if (scrapHud.gameObject.activeInHierarchy) {
            StopAllCoroutines();
            CancelInvoke("DeactiveScrapHud");
            StartCoroutine(IncreaseHud(cutOffScrap, Amount + (scrap - cutOffScrap)));
        }
        else {
            scrapHud.SetActive(true);
            StartCoroutine(IncreaseHud(scrap, Amount));
           
        }
    }

    void DeactiveScrapHud() {
        scrapHud.SetActive(false);
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
                scrapBoost = currentTool.scrapBoost + hammerUpgBoost;
                break;

            case ToolTypes.Wrench:
                scrapBoost = currentTool.scrapBoost + wrenchUpgBoost;
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
            cutOffScrap = (text + Mathf.Floor(adding));
            scrapText.text = cutOffScrap.ToString();
            if (i > 60) stepTime = 0.03f;
            if (i > 70) stepTime = 0.04f;
            if (i > 80) stepTime = 0.05f;

            if (i > 99) Invoke("DeactiveScrapHud", 5);
        }

        
    }
}
