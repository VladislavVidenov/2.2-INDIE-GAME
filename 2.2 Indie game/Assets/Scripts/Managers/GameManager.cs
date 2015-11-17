using UnityEngine;
using System.Collections;


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
    bool InMenu;
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
