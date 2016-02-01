using UnityEngine;
using System.Collections;

public class UpIncreaseDamage : Upgrade {
    GameObject[] weapons;
    WeaponScript weaponScript;

    [SerializeField]
    int upgradeMultiplier = 2; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

        weapons = GameManager.Instance.weaponManager.weaponsOnPlayer;

        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i] != null) {
                weaponScript = weapons[i].GetComponent<WeaponScript>();
                print("Old DAM: " + weaponScript.damage);
                weaponScript.damage *= upgradeMultiplier;
                print("New DAM: " + weaponScript.damage);
            }
        }
        print(string.Format("Weapon damage multiplied by {0}", upgradeMultiplier));
    }
}
