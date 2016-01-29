using UnityEngine;
using System.Collections;

public class UpHealthRegenDelay : Upgrade {
    public float regenDelay = 7.5f;  //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.SetRegenDelay(regenDelay);
        print(string.Format("New health regeneration delay: '{0}'", regenDelay));
    }
}
