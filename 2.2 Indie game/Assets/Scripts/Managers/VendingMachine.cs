using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour {

	[SerializeField]
	GameManagerScript gameManager;

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
    Button buyButton;



	void Start(){
		ownedUpgrades = new List<Upgrade> ();
		FindButtons ();
        DisableBuyScreen();
	}

	public void ActivateStation (){
        buyscreen.SetActive(true);
       
	}

	public void DeActivateStation () {
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
		allUpgrades.Remove (selectedUpgrade);
		ownedUpgrades.Add (selectedUpgrade);
		ChangeBuyButton("Allready have",false);
	}

    void CheckUpgrade() {
		if (ownedUpgrades.Contains (selectedUpgrade)) {
			ChangeBuyButton("Allready have",false);
		} else {
			if (selectedUpgrade.Cost > 100) {
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
}
