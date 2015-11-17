using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

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


    void Start() {
        ownedUpgrades = new List<Upgrade>();
        currentState = GameState.InMenu;
    }
    void Update() {
        Debug.Log(currentState);
    }

    // Use this for initialization
    public void SetCurrentState(GameState state) {
        currentState = state;
    }
    public GameState GetCurrentState() {
        return currentState;
    }

    public List<Upgrade> OwnedUpgrades{
        get {
            return ownedUpgrades;
        }
        set {
            ownedUpgrades = value;
        }
    }

    public void ResetGameManager() {
        ownedUpgrades.Clear(); 
    }

}
