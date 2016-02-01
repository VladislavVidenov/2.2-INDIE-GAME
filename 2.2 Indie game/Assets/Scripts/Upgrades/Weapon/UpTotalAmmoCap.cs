using UnityEngine;
using System.Collections;

/// <summary>
/// Upgrades the Total Ammo Cap for each weapon.
/// </summary>

public class UpTotalAmmoCap : Upgrade {

    GameObject[] weapons;
    WeaponScript weaponScript;

    [SerializeField]
    float upgradeMultiplier = 2; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

        weapons = GameManager.Instance.weaponManager.weaponsOnPlayer;

        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i] != null) {
                weaponScript = weapons[i].GetComponent<WeaponScript>();

                weaponScript.maxTotalAmmo = Mathf.FloorToInt(weaponScript.maxTotalAmmo * upgradeMultiplier);

                //Only do this for the weapon you are currently holding. Changed for upgrades to prevent the wrong info showing.
                
                if (weaponScript.gameObject.activeInHierarchy)
                    weaponScript.UpdateHudValues();
            }
        }
        print(string.Format("Total ammocap multiplied by {0}", upgradeMultiplier));
    }
}
