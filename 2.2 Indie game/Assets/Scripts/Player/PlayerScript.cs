using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    int health;
    int scrap;

    Camera camera;

    SceneChangeManager sceneManager;

	// Use this for initialization
	void Start () {
        camera = Camera.main;
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
        camera.GetComponent<Animation>().Play();
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
}
