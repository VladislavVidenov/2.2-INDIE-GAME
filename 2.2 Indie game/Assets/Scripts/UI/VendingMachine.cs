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
	Image buyMenuImage;
	[SerializeField]
	Text buyText;

	Upgrade selectedUpgrade;

	void Start(){
		FindButtons ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.I)) {
			ActivateStation ();
		}
	}

	public void ActivateStation (){
		foreach (Upgrade upgrade in allUpgrades) {

			if (upgrade.Cost > 100) {
				upgrade.buttonImage.color = Color.gray;
				//upgrade.buttonImage.GetComponent<Button>().interactable = false;

			}
		}
	}

	public void DeActivateStation () {

	}

	public void SelectUpgrade (Upgrade upgrade) {
		selectedUpgrade = upgrade;
		upgrade.buttonImage.color = Color.red;

		buyMenuImage.sprite = upgrade.upGradeImage;
		buyText.text = upgrade.text;
	}

	public void BuyUpgrade1 () {
		selectedUpgrade.Apply ();
	}

	void FindButtons () {
		foreach (Upgrade upgrade in allUpgrades) {
			upgrade.buttonImage = GameObject.Find (upgrade.name).GetComponent<Image> ();
		}
	}
}
