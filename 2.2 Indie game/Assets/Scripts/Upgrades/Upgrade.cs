using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum UpgradeType {
    Player, Weapon
}

public class Upgrade :MonoBehaviour {
    public string upgradeName = "NULL";

	public Sprite descriptionImage;
    public Text descriptionTextObj;

	public string text;

    public Sprite buttonSprite;

	public int BitsCost;

    public  int tier;
    public UpgradeType upgradeType;

	//public Image buttonImage;
    public virtual void Start() {
        descriptionTextObj.text = text;
    }

	public virtual void Apply (){

	}
}
