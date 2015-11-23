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
    float timer = 0;
    float waitTime = 5f;
    float pickDistance = 2.0f;
    float weaponSwitchTime = 0.5f;
    public LayerMask layerWeapons;

    bool showWeaponText = false;

    bool isWeaponOwned = false;
    bool isSlotTaken = false;
    [HideInInspector]
    public bool canSwitchWeapon = true;

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
        if(inventory[0] !=null && inventory[1] != null && inventory[2] != null)
        {
            //Debug.Log("S1->  " + inventory[0].weaponID + "  S2->  " + inventory[1].weaponID + "  S3->  " + inventory[2].weaponID);
        }
        if (canSwitchWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (inventory[0] == null) return;
             //   canSwitchWeapon = false;
                SetSlot(1, inventory[0].weaponID);
                Debug.Log("set wep 1 ");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (inventory[1] == null) return;
          //      canSwitchWeapon = false;
                SetSlot(2, inventory[1].weaponID);
                Debug.Log("set wep 2");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (inventory[2] == null) return;
            //    canSwitchWeapon = false;
                SetSlot(3, inventory[2].weaponID);
                Debug.Log("set wep 3");
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                canSwitchWeapon = false;
                lastWeaponUsed();
            }
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, pickDistance, layerWeapons) && canSwitchWeapon)
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

         //   Debug.Log("IS SLOT / WEAPON TAKEN ??" + isSlotTaken + " - " + isWeaponOwned);
            if (Input.GetKeyDown(KeyCode.F) && canSwitchWeapon)
            {
                if (isSlotTaken)
                {
                    if (isWeaponOwned)
                    {
                        // Debug.Log("shit happens brah.");
                        //just notify the player that he already owns the weapon
                        //-->maybe replace ? to do.

                    }
                    else
                    {
                        DropWeapon(hitWeaponIndex.slotID, inventory[hitWeaponIndex.slotID - 1].weaponID);
                       // inventory[hitWeaponIndex.slotID - 1] = hitWeaponIndex;
                        SetSlot(hitWeaponIndex.slotID, hitWeaponIndex.weaponID, true);
                        Destroy(hit.transform.gameObject);
                    }
                }
                else
                {
                  //  inventory[hitWeaponIndex.slotID - 1] = hitWeaponIndex;
                    SetSlot(hitWeaponIndex.slotID, hitWeaponIndex.weaponID); //enable the weapon
                    Destroy(hit.transform.gameObject);


                }
            }
        }
        else
        {
            showWeaponText = false;
        }
      //  Debug.Log("UPDATE HAPPENED ~!");


    }


    void lastWeaponUsed()
    {   //---------
        if (previousWeapon == null) return;
        if  (previousWeapon.slotID == 0 || currentWeapon.slotID == previousWeapon.slotID) { Debug.Log("No prev weapon , not switching !!"); return; }
        int previousSlotIdCopy = previousWeapon.slotID;
        int previousWwepIdcopy = previousWeapon.weaponID;
        SetSlot(previousSlotIdCopy, previousWwepIdcopy);
      //  Debug.Log("Weapon Switched !");
    }

    void SetSlot(int newSlotID, int newWeaponID, bool swapWeapons = false)
    {
        if (currentWeapon != null && newSlotID == currentWeapon.slotID && !swapWeapons)
        {
            // Debug.Log("Weapon already selected !!!!");
            return;
        }
        canSwitchWeapon = false;
        StartCoroutine(SwitchWeaponWait(newSlotID, newWeaponID, weaponSwitchTime));
        
    }

    IEnumerator SwitchWeaponWait(int newSlotID, int newWeaponID, float weaponSwitchTime)
    {
        if (currentWeapon != null) DisableWeapon(currentWeapon.slotID, currentWeapon.weaponID);
        yield return new WaitForSeconds(weaponSwitchTime);
        EnableWeapon(newSlotID, newWeaponID);
        yield return new WaitForSeconds(weaponSwitchTime);
        canSwitchWeapon = true;

    }
    void DropWeapon(int slotIndex, int wepIndex)
    {
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            WeaponIndex temp = weaponPrefabs[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slotIndex && temp.weaponID == wepIndex)
            {
                Rigidbody dropWeapon = Instantiate(weaponPrefabs[i], dropWeaponPosition.position, Quaternion.identity) as Rigidbody;
               // Debug.Log("DROPPED WEAPON ----> " + dropWeapon.gameObject.GetComponent<WeaponIndex>().slotID + " - " + dropWeapon.gameObject.GetComponent<WeaponIndex>().weaponID);
                dropWeapon.AddRelativeForce(0, 50, Random.Range(50, 150));
            }
        }
    }
    void DisableWeapon(int slot, int wepId)
    {
        canSwitchWeapon = false;
        for (int i = 0; i < weaponsOnPlayer.Length; i++)
        {
            WeaponIndex temp = weaponsOnPlayer[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slot && temp.weaponID == wepId)
            {
                previousWeapon = temp;
            

                weaponsOnPlayer[i].gameObject.SendMessage("HolsterWeapon", SendMessageOptions.DontRequireReceiver);
                weaponsOnPlayer[i].SetActive(false);
                weaponsOnPlayer[i].GetComponent<WeaponIndex>().enabled = false;
                weaponsOnPlayer[i].GetComponent<WeaponScript>().enabled = false;
              //  Debug.Log("DISABLED WEAPON --> " + temp.slotID + " - " + temp.weaponID);
                break;
            }
        }
    }
    void EnableWeapon(int slot, int wepId)
    {
        for (int i = 0; i < weaponsOnPlayer.Length; i++)
        {
            WeaponIndex temp = weaponsOnPlayer[i].GetComponent<WeaponIndex>();
            if (temp.slotID == slot && temp.weaponID == wepId)
            {
                weaponsOnPlayer[i].SetActive(true);
                weaponsOnPlayer[i].GetComponent<WeaponIndex>().enabled = true;
                weaponsOnPlayer[i].GetComponent<WeaponScript>().enabled = true;
                currentWeapon = temp;
                inventory[slot - 1] = currentWeapon;
                weaponsOnPlayer[i].gameObject.SendMessage("PullOutWeapon", SendMessageOptions.DontRequireReceiver);
             //   Debug.Log("ENABLE WEAPON --> " + temp.slotID + " - " + temp.weaponID);
                //  Debug.Log("------ENABLE HAPPENED ------ !");
                break;
            }
        }
    }




}
