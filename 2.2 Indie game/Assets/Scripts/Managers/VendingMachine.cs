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
	Image buyMenuImage;
	[SerializeField]
	Text buyText;
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
	}

	public void DeActivateStation () {
        SetPlayerStats();
		buyButton.gameObject.SetActive(false);
		SetImageAndText ();
        DisableBuyScreen();
		selectedUpgrade = null;
	}

	public void SelectUpgrade (Upgrade upgrade) {

		if (!buyButton.gameObject.activeInHierarchy) {
			buyButton.gameObject.SetActive( true);
		}

		selectedUpgrade = upgrade;

		SetImageAndText (selectedUpgrade);
        UpdateUpgradeCosts();

        CheckUpgrade();
	}

	public void BuyUpgrade () {
		selectedUpgrade.Apply ();
		ownedUpgrades.Add (selectedUpgrade);
		ChangeBuyButton("Allready have",false);
        playerScrap -= selectedUpgrade.ScrapCost;
        playerElectronics -= selectedUpgrade.ElectronicsCost;
        ChangePlayerStatsText();
	}

    void CheckUpgrade() {
		if (ownedUpgrades.Contains (selectedUpgrade)) {
			ChangeBuyButton("Allready have",false);
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

	void ChangeBuyButton (string text, bool active){
		buyButton.GetComponentInChildren<Text>().text = text;
			buyButton.interactable = active;
	}


    public List<Upgrade> GetOwndedList() {
            return ownedUpgrades;
    }

	void SetImageAndText (Upgrade upgrade = null) {
        if (!buyMenuImage.GetComponent<Mask>().showMaskGraphic) {
            buyMenuImage.GetComponent<Mask>().showMaskGraphic = true;
        }

		if (upgrade == null) {
			buyMenuImage.sprite = null;
			buyText.text = null;
		} else {
			buyMenuImage.sprite = upgrade.upGradeImage;
			buyText.text = upgrade.text;
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
}
