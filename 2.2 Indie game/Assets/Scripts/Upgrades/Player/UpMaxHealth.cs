using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpMaxHealth : Upgrade {
    [SerializeField] int upgradeAmount = 20;  //Set through inspector!

    public override void Apply ()
	{
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.IncreasePlayerStats(upgradeAmount, upgradeAmount, 0, 0, 0, 0);
        print(string.Format("MaxHealth upgraded by {0}pts", upgradeAmount));
    }
}
