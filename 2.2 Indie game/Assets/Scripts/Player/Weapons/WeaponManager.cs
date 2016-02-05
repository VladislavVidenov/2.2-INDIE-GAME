using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

    [SerializeField]
    Rigidbody[] weaponPrefabs;
    public GameObject[] weaponsOnPlayer; //all weapons WITH hands attached on the player gameObject!!  //-- set to public to access it with upgrades.
    [SerializeField]
    Transform dropWeaponPosition;
    RaycastHit hit;
    float pickDistance = 2.0f;
    public float weaponSwitchTime = 0.5f;
    public LayerMask layerWeapons;

    bool showWeaponText = false;
    bool isWeaponOwned = false;
    bool isSlotTaken = false;
    [HideInInspector]
    public bool canSwitchWeapon = true;

    WeaponIndex currentWeapon;
    WeaponIndex previousWeapon;
    [HideInInspector]
    public WeaponIndex[] inventory;
    WeaponIndex hitWeaponIndex;

    void Start() {
        inventory = new WeaponIndex[3];
        SetSlot(1, 1);
    }
    void OnGUI()
    {
        GUI.contentColor = Color.red;
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
                    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 100), "Press <E> to replace current weapon with this one");
                }

            }
            else
            {
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 150, 100), "Press <E> to pick up !");
            }
        }
    }

    void Update()
    {
        if (canSwitchWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (inventory[0] == null) return;
                SetSlot(1, inventory[0].weaponID);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (inventory[1] == null) return;
                SetSlot(2, inventory[1].weaponID);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (inventory[2] == null) return;
                SetSlot(3, inventory[2].weaponID);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
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
            if (Input.GetKeyDown(KeyCode.E) && canSwitchWeapon)
            {
                if (isSlotTaken)
                {
                    if (isWeaponOwned)
                    {
                        //just notify the player that he already owns the weapon
                        //-->maybe replace ? to do.
                    }
                    else
                    {
                        DropWeapon(hitWeaponIndex.slotID, inventory[hitWeaponIndex.slotID - 1].weaponID);
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

                dropWeapon.AddRelativeForce(Random.Range(0, -35), Random.Range(120, 200), Random.Range(1, 10));

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
            

                temp.gameObject.SendMessage("HolsterWeapon", SendMessageOptions.DontRequireReceiver);
                //temp.gameObject.SetActive(false);
                //temp.enabled = false;
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
                //temp.gameObject.SetActive(true);
                //temp.enabled = true;

                weaponsOnPlayer[i].SetActive(true);
                weaponsOnPlayer[i].GetComponent<WeaponIndex>().enabled = true;
                weaponsOnPlayer[i].GetComponent<WeaponScript>().enabled = true;
                currentWeapon = temp;
                inventory[slot - 1] = currentWeapon;
                temp.gameObject.SendMessage("PullOutWeapon", SendMessageOptions.DontRequireReceiver);
                //   Debug.Log("ENABLE WEAPON --> " + temp.slotID + " - " + temp.weaponID);
                //  Debug.Log("------ENABLE HAPPENED ------ !");
                weaponsOnPlayer[i].GetComponent<WeaponScript>().UpdateHudValues();
                break;
            }
        }
    }




}
