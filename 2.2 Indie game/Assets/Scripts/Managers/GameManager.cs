using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class has only one instance and REMAINS between all scene / menu switching.
/// This class is used to store DATA between scene changing.
/// </summary>

public class GameManager : MonoBehaviour {
    //Singleton for game manager.
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) {
                _instance = new GameObject("Game Manager").AddComponent<GameManager>();
                _instance.tag = Tags.gameController;
            }
            return _instance;
        }
    }
    
    // Player
    private GameObject _player;
    [HideInInspector]
    public GameObject Player
    {
        get
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player");
            return _player;
        }
    }

    PlayerScript playerScript {
        get {
            return _player.GetComponent<PlayerScript>();
        }
    }
    WeaponScript[] weapons
    {
        get
        {
            return _player.GetComponentsInChildren<WeaponScript>();
        }
    }

    [HideInInspector]
    public int maxHealth = 100;
    [HideInInspector]
    public float health = 100;
    [HideInInspector]
    public int maxStamina = 100;
    [HideInInspector]
    public float stamina = 100;
    [HideInInspector]
    public int scrap = 50;
    [HideInInspector]
    public int electronics = 50;

    //endPlayer

    GameState currentState;
    List<Upgrade> ownedUpgrades;
    [HideInInspector]
    public VendingMachine vendingMachine;
    [HideInInspector]
    public PauseMenuScript pauseMenu;
	public CoverSpotScript[] coverSpots;
    public WeaponManager weaponManager;

    public bool isWaving = false;
    
    public int pistolCurrentClipAmmo;
    public int pistolTotalAmmo;

    public int shotgunCurrentClipAmmo;
    public int shotgunTotalAmmo;


    void Awake() {
        if (Application.loadedLevel != 0) {
            currentState = GameState.InGame;
            FindGameObjects();
        }

        ownedUpgrades = new List<Upgrade>();
    }
   
    void OnLevelWasLoaded()
    {
        if(currentState == GameState.InGame)
        {
            FindGameObjects();
        }
    }

    public GameState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public List<Upgrade> OwnedUpgrades
    {
        get
        {
            return ownedUpgrades;
        }
        set
        {
            ownedUpgrades = value;
        }
    }

    public void ResetGameManager()
    {
        ownedUpgrades.Clear();
    }

    public void FindGameObjects() {
        vendingMachine = FindObjectOfType<VendingMachine>();
        pauseMenu = GameObject.Find("PauseMenuManager").GetComponent<PauseMenuScript>();
        coverSpots = FindObjectsOfType<CoverSpotScript>();
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    public void SavePlayerStats() {
        playerScript.GetCurrencyStats(out scrap, out electronics);
        playerScript.GetHealthStats(out health, out maxHealth);
        playerScript.GetStaminaStats(out stamina, out maxStamina);
    }
    public void SetWeaponStats()
    {        
        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponScript curr = weapons[i];
            if(curr.weapon == WeaponScript.Weapons.Pistol)
            {
                curr.ammoInClip = pistolCurrentClipAmmo;
                curr.totalAmmo = pistolTotalAmmo;
            }
            else if (curr.weapon == WeaponScript.Weapons.Shotgun)
            {
                curr.ammoInClip = shotgunCurrentClipAmmo;
                curr.totalAmmo = shotgunTotalAmmo;
            }
        }
    }
    public void SaveWeaponStats()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponScript curr = weapons[i];
            if (curr.weapon == WeaponScript.Weapons.Pistol)
            {
                pistolCurrentClipAmmo = curr.ammoInClip;
                pistolTotalAmmo = curr.totalAmmo;
            }
            else if (curr.weapon == WeaponScript.Weapons.Shotgun)
            { 
                shotgunCurrentClipAmmo = curr.ammoInClip;
                shotgunTotalAmmo = curr.totalAmmo;
            }
        }
    }

}
