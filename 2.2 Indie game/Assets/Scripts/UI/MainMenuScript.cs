using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {
    void Start()
    {
        DontDestroyOnLoad(GameManager.Instance);
    }

    public void StartLevel(int level) {
        Application.LoadLevel(level);
    }
}
