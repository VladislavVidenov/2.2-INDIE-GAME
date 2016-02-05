using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {

    public string levelName;

	public void SwitchScene()
    {
        if (levelName == "MenuScene")
        {
            GameObject.Find("SceneManager").GetComponent<SceneChangeManager>().SwitchToMainMenu();
        }
        else
        {
            Application.LoadLevel(levelName);
        }
    }
}
