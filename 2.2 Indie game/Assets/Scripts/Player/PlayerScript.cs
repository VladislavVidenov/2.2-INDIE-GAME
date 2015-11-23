using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    int health;
    int scrap;

    Camera mainCamera;

    SceneChangeManager sceneManager;

    [SerializeField]
    GameObject scrapHud;
    Text scrapText;


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
    }


    public void GetStats(out int pHealth, out int pMaxHealth, out int pScrap) {
        pHealth = health;
        pMaxHealth = maxHealth;
        pScrap = scrap;
    }

    public void SetStats(int pHealth, int pMaxHealth, int pScrap) {
        maxHealth =pMaxHealth;
        health = pHealth;
        scrap = pScrap;
    }

    public void IncreasePlayerStats(int pHealth, int pMaxHealth, int pScrap) {
        if(pScrap>0) IncreaseScrapHud(pScrap);
        maxHealth += pMaxHealth;
        health += pHealth;
        scrap += pScrap;
    }

    void IncreaseScrapHud(int Amount) {
        scrapHud.SetActive(true);
        StartCoroutine(IncreaseHud(scrap,Amount));
        Invoke("DeactiveScrapHud", 5);
    }

    void DeactiveScrapHud() {
        scrapHud.SetActive(false);
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
