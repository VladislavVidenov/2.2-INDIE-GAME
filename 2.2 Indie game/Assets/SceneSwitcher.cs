using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {

    public string levelName;

	public void SwitchScene()
    {
        Application.LoadLevel(levelName);

    }
}
