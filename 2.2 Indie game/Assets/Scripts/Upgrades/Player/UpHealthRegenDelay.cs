using UnityEngine;
using System.Collections;

public class UpHealthRegenDelay : Upgrade {
    public float regenDelay = 7.5f;  //Set through inspector!

    public override void Apply() {
        Debug.Log("Health Regen Delay Upgrade");
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.SetRegenDelay(regenDelay);
        Debug.Log("New Regen Delay = " + regenDelay);
    }
}
