using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// _1 = leftlane, _2 = middlelane , _3 = rightlane
/// </summary>

/// Extension on ToggleGroup
public static class ToggleGroupExtension {
    public static Toggle GetActive(this ToggleGroup tGroup) {
        return tGroup.ActiveToggles().FirstOrDefault();
    }
}


public class PauseMenuScript : MonoBehaviour {
    VendingMachine vendingMachine;
    List<Upgrade> ownedUpgrades;
    List<GameObject> UpgadeList;

    GameObject pauseUpgrade;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject upgradeImage;
    [SerializeField] Text upgradeText;
    [SerializeField] ToggleGroup toggleGroupOne, toggleGroupTwo;

    GameObject quit_2;
    GameObject options_2, options_3_Game, options_3_Display, options_3_Audio;
    GameObject current_2_Buttons, current_3_Buttons;


    int upgradeCount = 0;
    float xOffset = 75;

    void Start() {
        vendingMachine = GameObject.Find("VendingMachine").GetComponent<VendingMachine>();
        pauseUpgrade = GameObject.Find("PauseUpgrade");
        UpgadeList = new List<GameObject>();
        GetButtonReferences(false);
        DisablePauseScreen();
    }

    /// <summary>
    /// Gets references to a few buttons.
    /// </summary>
    /// <param name="visible">If set to false, the buttons will be deactivated on Start</param>
    void GetButtonReferences(bool visible = true) {
        quit_2 = GameObject.Find("Quit_2");
        options_2 = GameObject.Find("Options_2");
        options_3_Game = GameObject.Find("Options_3_Game");
        options_3_Display = GameObject.Find("Options_3_Display");
        options_3_Audio = GameObject.Find("Options_3_Audio");

        if (!visible) {
            quit_2.SetActive(false);
            options_2.SetActive(false);
            options_3_Game.SetActive(false);
            options_3_Display.SetActive(false);
            options_3_Audio.SetActive(false);
        }
    }

    public void ActivatePauseMenu() {
        pauseMenu.SetActive(true);
        ownedUpgrades = vendingMachine.GetOwndedList();
        if (ownedUpgrades != null) {
            DrawUpgrades();
        }

    }

    public void DeactivatePauseMenu() {
        if (toggleGroupOne.GetActive() != null) toggleGroupOne.GetActive().isOn = false;
        if (toggleGroupTwo.GetActive() != null) toggleGroupTwo.GetActive().isOn = false;

        DisableCurrent_2_Buttons();
        DisableCurrent_3_Buttons();
        pauseMenu.SetActive(false);
        DestroyImages();
        upgradeCount = 0;

        current_2_Buttons = null;
        current_3_Buttons = null;
    }

    void DrawUpgrades() {
        foreach (Upgrade upgrade in ownedUpgrades) {
            GameObject go = Instantiate(upgradeImage, pauseUpgrade.transform.position, Quaternion.identity) as GameObject;
            SetParent(go);
            go.GetComponent<Image>().sprite = upgrade.descriptionImage;
            go.GetComponent<UpgradeImageScript>().text = upgrade.text;
            UpgadeList.Add(go);
            upgradeCount++;
        }
    }

    void SetParent(GameObject child) {
        child.transform.SetParent(pauseUpgrade.transform);

        child.transform.position = pauseUpgrade.transform.position - pauseUpgrade.transform.position / 2 + new Vector3( 0 + upgradeCount * xOffset , pauseUpgrade.transform.position.y -5, 0);
    }

    void DisablePauseScreen() {
        pauseMenu.SetActive(false);
    }

    public void ChangeText(string text) {
        upgradeText.text = text;
    }

    public void DeactivateText() {
        upgradeText.text = "";
    }

    void ActiveButtons(GameObject buttons, bool active) {
        if (buttons == null) return;
        buttons.SetActive(active);
    }

    #region "Quit"
    public void Quit_2_ActiveButtons(bool active) {
        if (active && current_2_Buttons != null) DisableCurrent_2_Buttons();
        if (active && current_3_Buttons != null) DisableCurrent_3_Buttons();

        current_2_Buttons = quit_2;
        ActiveButtons(quit_2, active);
    }
    #endregion "Quit"

    #region "Options"
    public void Options_2_ActiveButtons(bool active) {
        if (active && current_2_Buttons != null) DisableCurrent_2_Buttons();
        if (active && current_3_Buttons != null) DisableCurrent_3_Buttons();

        current_2_Buttons = options_2;
        ActiveButtons(options_2, active);
    }

    public void Options_3_Game_ActivateButtons(bool active) {
        if (active && current_3_Buttons != null) DisableCurrent_3_Buttons();
        current_3_Buttons = options_3_Game;
        ActiveButtons(options_3_Game, active);
    }
    public void Options_3_Display_ActivateButtons(bool active) {
        if (active && current_3_Buttons != null) DisableCurrent_3_Buttons();
        current_3_Buttons = options_3_Display;
        ActiveButtons(options_3_Display, active);
    }
    public void Options_3_Audio_ActivateButtons(bool active) {
        if (active && current_3_Buttons != null) DisableCurrent_3_Buttons();
        current_3_Buttons = options_3_Audio;
        ActiveButtons(options_3_Audio, active);
    }

    #endregion "Options"

    void DisableCurrent_2_Buttons() {
        ActiveButtons(current_2_Buttons, false);
    }

    void DisableCurrent_3_Buttons() {
        ActiveButtons(current_3_Buttons, false);
    }

    void DestroyImages() {
        foreach (GameObject go in UpgadeList) {
            Destroy(go);
        }
        UpgadeList.Clear();
    }
}
