using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    int maxHealth;
    int health;
    int scrap;

	// Use this for initialization
	void Start () {
        GetPlayerStats();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeHealth(int amount) {
        health += amount;
        if (health > 100) health = 100;
        if (health <= 0) Died();
    }

    public void Died() {
        Respawn();
    }

    void Respawn() {
    }

    void GetPlayerStats() {
        int maxHealth = GameManager.Instance.MaxHealth;
        int health = GameManager.Instance.Health;
        int scrap = GameManager.Instance.Scrap;
    }

    void SetPlayerStats() {

    }
}
