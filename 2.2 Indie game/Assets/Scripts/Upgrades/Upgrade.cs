using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UpgradeType {
    Player, Tools, Weapon
}

public class Upgrade :MonoBehaviour {
    public string upgradeName = "NULL";

	public Sprite descriptionImage;
	public string text;

    public Sprite buttonSprite;

	public int ScrapCost;
    public int ElectronicsCost;

    public  int tier;
    public UpgradeType upgradeType;

	//public Image buttonImage;
    public virtual void Start() {
    }

	public virtual void Apply (){

	}
}
