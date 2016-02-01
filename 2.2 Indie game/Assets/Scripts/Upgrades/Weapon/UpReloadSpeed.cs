using UnityEngine;
using System.Collections;

public class UpReloadSpeed : Upgrade {
    GameObject[] weapons;
    WeaponScript weaponScript;

    [SerializeField]
    float upgradeMultiplier = 0.5f; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

        weapons = GameManager.Instance.weaponManager.weaponsOnPlayer;

        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i] != null) {
                weaponScript = weapons[i].GetComponent<WeaponScript>();
                print("Old RLDS: " + weaponScript.reloadTime);
                weaponScript.reloadTime *= upgradeMultiplier;
                print("New RLDS: " + weaponScript.reloadTime);
            }
        }
        print(string.Format("ReloadSpeed multiplied by {0}", upgradeMultiplier));
    }
}
