using UnityEngine;
using System.Collections;

public class UpAmmoClipSize : Upgrade {
    GameObject[] weapons;
    WeaponScript weaponScript;

    [SerializeField] int upgradeMultiplier = 2; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

        weapons = GameManager.Instance.weaponManager.weaponsOnPlayer;

        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i] != null) {
                weaponScript = weapons[i].GetComponent<WeaponScript>();

                weaponScript.maxAmmoInClip *= upgradeMultiplier;

                if (weaponScript.gameObject.activeInHierarchy)
                    weaponScript.UpdateHudValues();
            }
        }
        print(string.Format("AmmoClipSize multiplied by {0}", upgradeMultiplier));
    }
}
