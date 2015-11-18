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
            if (_instance == null)
                _instance = new GameObject("Game Manager").AddComponent<GameManager>();
            return _instance;
        }
    }
    
    
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

    GameState currentState;
    List<Upgrade> ownedUpgrades;
    [HideInInspector]
    public VendingMachine vendingMachine;
    [HideInInspector]
    public PauseMenuScript pauseMenu;

    void Awake()
    {
       
        if (Application.loadedLevel != 0)
        {
            currentState = GameState.InGame;
            FindGameObjects();
        }
        Debug.Log("---State is set to " + CurrentState + " by default---");
    }

    void Start()
    {
        ownedUpgrades = new List<Upgrade>();
    }
   
    void OnLevelWasLoaded()
    {
        Debug.Log("SOME LEVEL WAS LOADED!" + Application.loadedLevel);

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

        vendingMachine = GameObject.Find("VendingMachine").GetComponent<VendingMachine>();
        pauseMenu = GameObject.Find("PauseMenuManager").GetComponent<PauseMenuScript>();
    }

}
