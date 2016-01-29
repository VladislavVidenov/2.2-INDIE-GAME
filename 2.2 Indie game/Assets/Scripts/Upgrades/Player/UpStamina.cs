using UnityEngine;
using System.Collections;

public class UpStamina : Upgrade {
    public int upgradeAmount = 50; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.IncreasePlayerStats(0, 0, upgradeAmount, upgradeAmount, 0, 0);
        print(string.Format("Stamina upgraded by {0}pts.", upgradeAmount));
    }
}
