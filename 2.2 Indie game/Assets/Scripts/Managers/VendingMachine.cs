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
	Image buyMenuImage;
	[SerializeField]
	Text buyText;
    [SerializeField]
    Text healthText;
    [SerializeField]
    Text scrapText;
    [SerializeField]
    Button buyButton;

    //player
    PlayerScript player;
    int playerHealth;
    int playerScrap;
    int playerMaxHealth;




	void Start(){
        player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerScript>() ;

		ownedUpgrades = GameManager.Instance.OwnedUpgrades;
        ApplyUpgrades();

		FindButtons ();
        DisableBuyScreen();
	}

	public void ActivateStation (){
        GetPlayerStats();
        ChangePlayerStatsText();
        buyscreen.SetActive(true);
	}

	public void DeActivateStation () {
        SetPlayerStats();
		if(selectedUpgrade != null) ChangeSelectedButton(Color.white);
		buyButton.gameObject.SetActive(false);
		SetImageAndText ();
        DisableBuyScreen();
		selectedUpgrade = null;
	}

	public void SelectUpgrade (Upgrade upgrade) {
        if (selectedUpgrade != null) {
			ChangeSelectedButton(Color.white);
        }
		if (!buyButton.gameObject.activeInHierarchy) {
			buyButton.gameObject.SetActive( true);
		}

		selectedUpgrade = upgrade;

		ChangeSelectedButton(Color.red);

		SetImageAndText (selectedUpgrade);

        CheckUpgrade();
	}

	public void BuyUpgrade () {
		selectedUpgrade.Apply ();
		ownedUpgrades.Add (selectedUpgrade);
		ChangeBuyButton("Allready have",false);
        playerScrap -= selectedUpgrade.Cost;
        ChangePlayerStatsText();
	}

    void CheckUpgrade() {
		if (ownedUpgrades.Contains (selectedUpgrade)) {
			ChangeBuyButton("Allready have",false);
		} else {
			if (selectedUpgrade.Cost > playerScrap) {
				ChangeBuyButton("no Money",false);
			} else {
				ChangeBuyButton("Buy Upgrade",true);
			}
		}
    }

	void FindButtons () {
		foreach (Upgrade upgrade in allUpgrades) {
			upgrade.buttonImage = GameObject.Find (upgrade.name + "Button").GetComponent<Image> ();
		}
	}

    void DisableBuyScreen() {
        buyscreen.SetActive(false);
    }

	void ChangeBuyButton (string text, bool active){
		buyButton.GetComponentInChildren<Text>().text = text;
			buyButton.interactable = active;
	}

	void ChangeSelectedButton (Color color) {
		selectedUpgrade.buttonImage.color = color;
	}

    public List<Upgrade> GetOwndedList() {
            return ownedUpgrades;
    }

	void SetImageAndText (Upgrade upgrade = null) {
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
        player.GetStats(out playerHealth,out playerMaxHealth, out playerScrap);
    }

    void SetPlayerStats() {
        player.SetStats(playerHealth, playerMaxHealth, playerScrap);
    }

    void ChangePlayerStatsText() {
        scrapText.text = "Scrap:"+ playerScrap.ToString();
        healthText.text ="health"+ playerHealth.ToString() + "/" + playerMaxHealth.ToString();
    }
}
