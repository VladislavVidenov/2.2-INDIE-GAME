using UnityEngine;
using System.Collections;

public class Startgame : MonoBehaviour {
    void Start()
    {
        DontDestroyOnLoad(GameManager.Instance);
    }
	void OnGUI()
    {
        
        if(GUI.Button(new Rect(100, 100, 50, 50), "START"))
        {
            Application.LoadLevel(1);
        }
    }
}
