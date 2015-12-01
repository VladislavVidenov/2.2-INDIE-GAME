using UnityEngine;
using System.Collections;

public class AmmoPickable : MonoBehaviour {
    WeaponManager wepManager;
    public enum AmmoType
    {
        Pistol, Shotgun
    }

    public AmmoType typeOfAmmo;

    public int AmmoGiveAmount;

    void Start()
    {
        wepManager = FindObjectOfType<WeaponManager>();
        gameObject.name += " - " + typeOfAmmo.ToString();
    }

	void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {
            if(typeOfAmmo == AmmoType.Pistol)
            {
                wepManager.inventory[0].GetComponent<WeaponScript>().IncreaseTotalAmmo(AmmoGiveAmount);
            }
            else if (typeOfAmmo == AmmoType.Shotgun)
            {
                wepManager.inventory[1].GetComponent<WeaponScript>().totalAmmo += AmmoGiveAmount;
            }
        }
        Destroy(gameObject);
    }
}
