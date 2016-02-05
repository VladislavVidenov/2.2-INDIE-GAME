using UnityEngine;
using System.Collections;

public class AmmoPickable : MonoBehaviour {
    WeaponManager wepManager;
    WeaponScript weapon;

    public enum AmmoType
    {
        Pistol, Shotgun
    }

    public AmmoType typeOfAmmo;

    public int AmmoGiveAmount;

    void Start()
    {
        wepManager = FindObjectOfType<WeaponManager>();
    }

	void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            if(typeOfAmmo == AmmoType.Pistol)
            {
                weapon = wepManager.inventory[0].GetComponent<WeaponScript>();
                weapon.IncreaseTotalAmmo(AmmoGiveAmount);
            }
            else if (typeOfAmmo == AmmoType.Shotgun)
            {
                weapon = wepManager.inventory[1].GetComponent<WeaponScript>();
                weapon.IncreaseTotalAmmo(AmmoGiveAmount);
            }

            if (weapon.gameObject.activeInHierarchy)
                weapon.UpdateHudValues();

            Destroy(gameObject);
        }
      
    }
}
