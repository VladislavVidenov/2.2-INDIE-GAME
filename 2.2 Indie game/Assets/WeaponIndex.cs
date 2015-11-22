using UnityEngine;
using System.Collections;

public class WeaponIndex : MonoBehaviour {

    public int slotID;
    public int weaponID;
    [HideInInspector]
    public string weaponType = "";
    void Start()
    {
        if (slotID == 3)
        {
            if (weaponID == 1) weaponType = "Screwdriver";
            else if (weaponID == 2) weaponType = "Wrench";
            else if (weaponID == 3) weaponType = "Hammer";
        }
        else if (slotID == 1) weaponType = "Handgun";
        else if (slotID == 2) weaponType = "Shotgun";
    }

    void FakeDisable()
    {
        GetComponent<Collider>().enabled = false;
        gameObject.SetActive(false);
    }
}
