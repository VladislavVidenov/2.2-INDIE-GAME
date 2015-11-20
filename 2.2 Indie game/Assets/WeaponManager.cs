using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

   // [SerializeField]
   // GameObject[] weaponsOnGround; //JUST weapons available for picking up
    [SerializeField]
    Rigidbody[] weaponPrefabs;
    [SerializeField]
    Transform dropWeaponPosition;

    //GameObject currentWeapon;
   // int currentSlot;
   


    RaycastHit hit;
    float pickDistance = 2.0f;
    public LayerMask layerWeapons;
    bool showWeaponText = false;

    bool isWeaponOwned = false;
    bool isSlotTaken = false;


    //int hitSlotIndex;
    //int hitWepIndex;
    WeaponIndex currentWeapon;
    WeaponIndex previousWeapon;
    WeaponIndex[] inventory;
    WeaponIndex hitWeaponIndex;
    [SerializeField]
    GameObject[] weaponsOnPlayer; //all weapons WITH hands attached on the player gameObject!!
    void Start()
    {
        inventory = new WeaponIndex[3];
        
    }

    void OnGUI()
    {

        if (showWeaponText)
        {
            if (isSlotTaken)
            {
                if (isWeaponOwned)
                {   //if the slot is taken and the weapon is owned.
                    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 100), "You already own this weapon!");
                }
                else
                    //if the slot is taken but the weapon is NOT owned.
                {
                    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 100), "Press <F> to replace current weapon with this one");
                }
                
            }
            else
            {
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 100), "Press <F> to pick up !");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory[0] == null) return;
            SetSlot(1,inventory[0].weaponID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory[1] == null) return;
            SetSlot(2,inventory[1].weaponID);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory[2] == null) return;
            SetSlot(3,inventory[2].weaponID);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            lastWeaponUsed();
        }

        //Debug.Log("Previous weapon -> " + previousWeaponIndex);
        //Debug.Log("Current weapon  -> " + currentWeaponIndex);

        Vector3 position = transform.parent.position;
        Vector3 direction = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(position, direction, out hit, pickDistance, layerWeapons))
        {
            hitWeaponIndex = hit.transform.GetComponent<WeaponIndex>();
            showWeaponText = true;
            if (inventory[hitWeaponIndex.slotID - 1] != null)
            {
                isSlotTaken = true;
                if (inventory[hitWeaponIndex.slotID - 1].weaponID == hitWeaponIndex.weaponID)
                    isWeaponOwned = true;
                else
                    isWeaponOwned = false;
            }
            else
            {
                isSlotTaken = false;
                isWeaponOwned = false;
            }
            Debug.Log("Weapon owned ?" + isWeaponOwned + " Slot taken ? " + isSlotTaken);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isSlotTaken)
                {
                    if (isWeaponOwned)
                    {
                        Debug.Log("shit happens brah.");
                       //just notify the player that he already owns the weapon
                       //-->maybe replace ? to do.
                    }
                    else
                    {
                      //  not tested if working.
                     
                        DropWeapon(hitWeaponIndex.slotID,inventory[hitWeaponIndex.slotID - 1].weaponID);
                        inventory[hitWeaponIndex.slotID - 1] = hitWeaponIndex;
                        SetSlot(hitWeaponIndex.slotID, hitWeaponIndex.weaponID,true);
                        inventory[hitWeaponIndex.slotID - 1] = currentWeapon;
                        Destroy(hit.transform.gameObject);

                    }

                }
                else
                {
                    // assign the weapon index to the Inventory
                    inventory[hitWeaponIndex.slotID - 1] = hitWeaponIndex;
                    SetSlot(hitWeaponIndex.slotID,hitWeaponIndex.weaponID); //enable the weapon
                    inventory[hitWeaponIndex.slotID - 1] = currentWeapon;
                    Debug.Log("DESTROOOYING -------> " + hit.transform.gameObject.name);
                    Destroy(hit.transform.gameObject); //destroy the picked item/weapon.
                    /// ^^^problem.
                }
            }

        }
        else
        {
            showWeaponText = false;
        }
    }


    void lastWeaponUsed()
    {   //---------
        if(previousWeapon != null && previousWeapon.slotID == 0 || currentWeapon.slotID == previousWeapon.slotID) { Debug.Log("No prev weapon , not switching !!"); return; }
        int previousSlotIdCopy = previousWeapon.slotID;
        int previousWwepIdcopy = previousWeapon.weaponID;
        DisableWeapon(currentWeapon.slotID,currentWeapon.weaponID);
        EnableWeapon(previousSlotIdCopy,previousWwepIdcopy);
       
    }

    void SetSlot(int newSlotID, int newWeaponID = 1,bool swapWeapons = false)
    {

            if (currentWeapon != null && newSlotID == currentWeapon.slotID && !swapWeapons)
            {
                Debug.Log("Slot weapon already selected - return !");
                return;
            }
            //if it is the right slot.
            if (inventory[newSlotID - 1] != null && inventory[newSlotID - 1].slotID == newSlotID && inventory[newSlotID - 1].weaponID == newWeaponID)
            {
                if(currentWeapon != null)DisableWeapon(currentWeapon.slotID, currentWeapon.weaponID);
                EnableWeapon(newSlotID, newWeaponID);

            }
        
    }

    void DropWeapon(int slotIndex,int wepIndex)

    {
        Debug.Log("Slot ind" + slotIndex + "Wep ind" + wepIndex);
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            WeaponIndex temp = weaponPrefabs[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slotIndex && temp.weaponID == wepIndex)
            {
                Rigidbody dropWeapon = Instantiate(weaponPrefabs[i], dropWeaponPosition.position, Quaternion.identity) as Rigidbody;
                Debug.Log("DROPPED WEAPON ----> " + dropWeapon.gameObject.name);
                dropWeapon.AddRelativeForce(0, 50, Random.Range(50, 150));
            }
        }
    }
    void DisableWeapon(int slot, int wepId)
    {
        
        for (int i = 0; i < weaponsOnPlayer.Length; i++)
        {
            WeaponIndex temp = weaponsOnPlayer[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slot && temp.weaponID == wepId)
            {
                previousWeapon = temp;
                Debug.Log("DISABLED WEAPON --> " + temp.gameObject.name);
                weaponsOnPlayer[i].SetActive(false);

                break;
            }
        }
    }
    void EnableWeapon(int slot, int wepId = 1)
    {
        for (int i = 0; i < weaponsOnPlayer.Length; i++)
        {
            WeaponIndex temp = weaponsOnPlayer[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slot && temp.weaponID == wepId)
            {
         
               
                currentWeapon = temp;
                Debug.Log("ENABLED WEAPON --> " + temp.gameObject.name);
                weaponsOnPlayer[i].SetActive(true);
                break;
            }
        }
    }
   
}
