using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UpgradeImageScript : MonoBehaviour {

    public string text;

    public void ChangeText() {
        GameObject.Find("PauseMenuManager").GetComponent<PauseMenuScript>().ChangeText(text);
    }
    public void DeactiveText() {
        GameObject.Find("PauseMenuManager").GetComponent<PauseMenuScript>().DeactivateText();
    }
	
}
