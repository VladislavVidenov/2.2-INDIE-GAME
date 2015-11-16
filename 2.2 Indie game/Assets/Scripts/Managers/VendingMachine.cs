using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour {

	//functionality
	[SerializeField]
	List<Upgrade> allUpgrades;
	List<Upgrade> ownedUpgrades;

	//menuVisuals
    [SerializeField]
    GameObject buyscreen;
	[SerializeField]
	Image buyMenuImage;
	[SerializeField]
	Text buyText;
    [SerializeField]
    Button buyButton;

	Upgrade selectedUpgrade;

	void Start(){
		FindButtons ();
        DisableBuyScreen();
	}

	public void ActivateStation (){
        buyscreen.SetActive(true);
       
	}

	public void DeActivateStation () {
        DisableBuyScreen();
	}

	public void SelectUpgrade (Upgrade upgrade) {
        if (selectedUpgrade != null) {
            selectedUpgrade.buttonImage.color = Color.white;
        }

		selectedUpgrade = upgrade;
		upgrade.buttonImage.color = Color.red;

		buyMenuImage.sprite = upgrade.upGradeImage;
		buyText.text = upgrade.text;
        CheckUpgrade();
	}

	public void BuyUpgrade () {
		selectedUpgrade.Apply ();
	}

    void CheckUpgrade() {
        if (selectedUpgrade.Cost> 100){
            buyButton.GetComponentInChildren<Text>().text = "no Money";
        }
        else {
            buyButton.GetComponentInChildren<Text>().text = "Buy Upgrade";
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
}
