using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuScript : MonoBehaviour {

    VendingMachine vendingMachine;
    List<Upgrade> ownedUpgrades;
    List<GameObject> UpgadeList;

    GameObject pauseUpgrade;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject upgradeImage;
    [SerializeField]
    Text upgradeText;

    int upgradeCount = 0;
    float xOffset = 75;


    void Start() {
        vendingMachine = GameObject.FindWithTag(Tags.vendingMachine).GetComponent<VendingMachine>();
        pauseUpgrade = GameObject.Find("PauseUpgrade");
        DisablePauseScreen();
        UpgadeList = new List<GameObject>();
    }

    public void ActivatePauseMenu() {
        pauseMenu.SetActive(true);
        ownedUpgrades = vendingMachine.GetOwndedList();
        if (ownedUpgrades != null) {
            DrawUpgrades();
        }

    }
    public void DeActivatePauseMenu() {
        pauseMenu.SetActive(false);
        DestroyImages();
        upgradeCount = 0;

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

    void DestroyImages() {
        foreach (GameObject go in UpgadeList) {
            Destroy(go);
        }
        UpgadeList.Clear();
    }
}
