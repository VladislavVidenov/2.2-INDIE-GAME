using UnityEngine;
using System.Collections;

public class UpAimDownSightsSpeed : Upgrade {
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
                print("OLD aimDamp: " + weaponScript.aimingDamp + "   fovDamp: " + weaponScript.fovDamp);
                weaponScript.aimingDamp *= upgradeMultiplier;
                weaponScript.fovDamp *= (upgradeMultiplier / 2);
                print("NEW aimDamp: " + weaponScript.aimingDamp + "   fovDamp: " + weaponScript.fovDamp);
            }
        }
        print(string.Format("AimDownSightSpeed multiplied by {0}", upgradeMultiplier));
    }
}
