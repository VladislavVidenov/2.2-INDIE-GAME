using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour {

	//functionality
	[SerializeField]
	List<Upgrade> allUpgrades;
    List<Upgrade> ownedUpgrades;

	Upgrade selectedUpgrade;
    Button selectedButton;

	//menuVisuals
    [SerializeField]
    GameObject buyscreen;
    [SerializeField]
    GameObject playerUpgrades;
    [SerializeField]
    GameObject toolUpgrades;
    [SerializeField]
    GameObject weaponUpgrades;

    GameObject currentUpgrades;


    [SerializeField]
    Button buyButton;
    [SerializeField]
	Image UpgradeSprite;
	[SerializeField]
	Text UpgradeDescriptionText;
    [SerializeField]
    Text electronicsText;
    [SerializeField]
    Text scrapText;
    [SerializeField]
    Text electronicsCostText;
    [SerializeField]
    Text scrapCostText;
 
    //player
    PlayerScript player;
    int playerScrap;
    int playerElectronics;

    bool AlreadyHave = false;

	void Start(){
        player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerScript>() ;

		ownedUpgrades = GameManager.Instance.OwnedUpgrades;
        ApplyUpgrades();

		//FindButtons ();
        DisableBuyScreen();
	}

	public void ActivateStation (){
        GetPlayerStats();
        ChangePlayerStatsText();
        buyscreen.SetActive(true);
        buyButton.gameObject.SetActive(false);
        electronicsCostText.text = "";
        scrapCostText.text = "";
	}

	public void DeActivateStation () {
        SetPlayerStats();
		SetImageAndText ();
        DisableBuyScreen();
		selectedUpgrade = null;
        UpgradeSprite.GetComponent<Mask>().showMaskGraphic = false;
        SetSelectedButtonColor(1, 1, 1);
	}

	public void SelectUpgrade (Upgrade upgrade) {
        if (!buyButton.gameObject.activeInHierarchy) {
            buyButton.gameObject.SetActive(true);
        }
		if (!buyButton.gameObject.activeInHierarchy) {
			buyButton.gameObject.SetActive( true);
		}

		selectedUpgrade = upgrade;
		
        UpdateUpgradeCosts();

        CheckUpgrade();
	}

    public void SelectButton(Button button) {
        if (selectedButton != null) {
            SetSelectedButtonColor(1, 1, 1);
        }

        selectedButton = button;
        SetSelectedButtonColor(216f/255, 1, 39/255);
        SetImageAndText(selectedUpgrade);
    }

	public void BuyUpgrade () {
		selectedUpgrade.Apply ();
		ownedUpgrades.Add (selectedUpgrade);
		ChangeBuyButton("Purchased",false);
        playerScrap -= selectedUpgrade.ScrapCost;
        playerElectronics -= selectedUpgrade.ElectronicsCost;
        ChangePlayerStatsText();
        selectedButton.image.sprite = selectedUpgrade.buttonSprite;
        SetSelectedButtonColor(1, 1, 1);
	}

    void CheckUpgrade() {
		if (ownedUpgrades.Contains (selectedUpgrade)) {
			ChangeBuyButton("Purchased",false);
		} else {
			if (selectedUpgrade.ScrapCost > playerScrap || selectedUpgrade.ElectronicsCost >playerElectronics) {
          
				ChangeBuyButton("Not enough resources",false);
			} else {
				ChangeBuyButton("Buy Upgrade",true);
			}
		}
    }

    //void FindButtons () {
    //    foreach (Upgrade upgrade in allUpgrades) {
    //        upgrade.buttonImage = GameObject.Find (upgrade.name + "Button").GetComponent<Image> ();
    //    }
    //}

    void DisableBuyScreen() {
        buyscreen.SetActive(false);
    }

    void SetSelectedButtonColor(float r, float g, float b) {
        if (selectedButton != null) {
            selectedButton.image.color = new Color(r, g, b);
        }
    }

    void ChangeBuyButton(string text, bool active) {
        buyButton.GetComponentInChildren<Text>().text = text;

        ChangeBuyButtonTextColorLight(buyButton.GetComponentInChildren<Text>());
        buyButton.interactable = active;
        if (!active) {
            buyButton.image.color = new Color(31f / 255, 1, 35f / 255);
        }
        else {
            buyButton.image.color = new Color(1, 1, 1);
        }
    }


    public List<Upgrade> GetOwndedList() {
            return ownedUpgrades;
    }

	void SetImageAndText (Upgrade upgrade = null) {
        if (!UpgradeSprite.GetComponent<Mask>().showMaskGraphic) {
            UpgradeSprite.GetComponent<Mask>().showMaskGraphic = true;
        }

		if (upgrade == null) {
			UpgradeSprite.sprite = null;
			UpgradeDescriptionText.text = null;
		} else {
			UpgradeSprite.sprite = selectedButton.image.sprite;
			UpgradeDescriptionText.text = upgrade.text;
		}
	}

    void ApplyUpgrades() {
        foreach (Upgrade upgrade in ownedUpgrades) {
            upgrade.Apply();
        }
    }

    void GetPlayerStats() {
        player.GetCurrencyStats( out playerScrap,out playerElectronics);
    }

    void SetPlayerStats() {
        player.SetCurrencyStats(playerScrap,playerElectronics);
    }

    void ChangePlayerStatsText() {
        scrapText.text = playerScrap.ToString();
        electronicsText.text = playerElectronics.ToString();
    }

    public void SelectPlayer() {
        SetGameObjectActive(playerUpgrades,true);
    }

    public void SelectTools() {
        SetGameObjectActive(toolUpgrades, true);
    }

    public void SelectWeapons() {
        SetGameObjectActive(weaponUpgrades, true);
    }

    void SetGameObjectActive(GameObject gameobject , bool on) {
        DisablePreviousUpgrades();
        gameobject.SetActive(on);
        currentUpgrades = gameobject;
    }

    void DisablePreviousUpgrades() {
        if (currentUpgrades != null) {
            currentUpgrades.SetActive(false);
        }
    }

    void UpdateUpgradeCosts() {
        scrapCostText.text = selectedUpgrade.ScrapCost.ToString();
        electronicsCostText.text = selectedUpgrade.ElectronicsCost.ToString();
    }

    public void ChangeBuyButtonTextColorDark(Text text) {

        if (buyButton.interactable) text.color = new Color(0, 51f / 255, 53f / 255);
    }
    public void ChangeBuyButtonTextColorLight(Text text) {
         text.color = new Color(9f / 255, 184f/255, 190f / 255);
    }
}
