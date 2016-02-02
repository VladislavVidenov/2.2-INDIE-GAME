using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter() {
        GameManager.Instance.SavePlayerStats();
        GameManager.Instance.SaveWeaponStats();
        GameManager.Instance.SaveUpgrades();
        Application.LoadLevel("Level2");

    }
}
