using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour {

    public void HighlightToggle(GameObject toggleGo) {
        Toggle toggle = toggleGo.GetComponent<Toggle>();

        if (toggle.isOn) {
            toggle.interactable = false;
        } else {
            toggle.interactable = true;
        }

        Color textColor = toggleGo.GetComponentInChildren<Text>().color;
        Color toggleColor = toggleGo.GetComponent<Image>().color;

        toggleGo.GetComponentInChildren<Text>().color = toggleColor;
        toggleGo.GetComponent<Image>().color = textColor;
    }
}
