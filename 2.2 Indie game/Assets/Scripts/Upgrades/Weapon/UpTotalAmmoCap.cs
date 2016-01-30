using UnityEngine;
using System.Collections;

/// <summary>
/// Upgrades the Total Ammo Cap for each weapon.
/// </summary>

public class UpTotalAmmoCap : Upgrade {

    GameObject[] weapons;

    [SerializeField] int upgradeMultiplier = 2; //Set through inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

        weapons = GameManager.Instance.weaponManager.weaponsOnPlayer;

        for (int i = 0; i < weapons.Length; i++) {
            if(weapons[i] != null) weapons[i].GetComponent<WeaponScript>().maxTotalAmmo *= upgradeMultiplier;
        }

        print(string.Format("Total ammocap multiplied by {0}", upgradeMultiplier));
    }
    }
