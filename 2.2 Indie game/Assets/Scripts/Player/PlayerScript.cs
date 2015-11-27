using UnityEngine;
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


    Camera mainCamera;

    SceneChangeManager sceneManager;

    [SerializeField]
    GameObject scrapHud;
    Text scrapText;

    Tool currentTool;
	// Use this for initialization
	void Start () {
        scrapText = scrapHud.GetComponentInChildren<Text>();
        DeactiveScrapHud();
        mainCamera = Camera.main;
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        GetPlayerStatsFromGameManager();
	}
	
	// Update is called once per frame
	void Update () {
         if (Input.GetKeyDown(KeyCode.I)) Died();
    }

    public void ChangeHealth(int amount) {
        health += amount;
        if (health > 100) health = 100;
        if (health <= 0) Died();
    }

    public void Died() {
        sceneManager.SetState(GameState.PlayerDied);
        mainCamera.GetComponent<Animation>().Play();
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
        electronics += pElectronics + electronicsBoost;
    }

    void IncreaseScrapHud(int Amount) {
        scrapHud.SetActive(true);
        StartCoroutine(IncreaseHud(scrap,Amount));
        Invoke("DeactiveScrapHud", 5);
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
    IEnumerator IncreaseHud(int text, int Amount) {

        float stepTime = 0.02f;
        float addAmount = (float)Amount / 100;
        
        for (int i = 0; i < 100+1; i++) {
            yield return new WaitForSeconds(stepTime);
            float scrapAmount = i * addAmount;
            scrapText.text = (text + Mathf.Floor(scrapAmount)).ToString();
            if (i > 60) stepTime = 0.03f;
            if (i > 70) stepTime = 0.04f;
            if (i > 80) stepTime = 0.05f;
        }
    }
}
