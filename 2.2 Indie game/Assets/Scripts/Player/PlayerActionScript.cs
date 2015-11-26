using UnityEngine;
using System.Collections;

public class PlayerActionScript : MonoBehaviour {

    float maxRayDistance = 2.0f;
    [SerializeField]
    LayerMask layerMask;
    public GUISkin guiSkin;
    bool showGuiSkin = false;
    RaycastHit hit;
    bool gotInfo = false;
	// Update is called once per frame
	void Update () {
        Raycasting();
	}

    void Raycasting() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //    Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.black,2);
        if (Physics.Raycast(ray,out hit, maxRayDistance, layerMask.value)) {
            if (gotInfo) return;
            switch (hit.collider.tag) {

                case "VendingMachine":
                    showGuiSkin = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    GameObject.Find("SceneManager").GetComponent<SceneChangeManager>().SetState(GameState.InBuyScreen);
                    break;

                case "LootableBox":
                    LootableBoxScript lootBox = hit.collider.gameObject.GetComponent<LootableBoxScript>();
                    if (!lootBox.isLooted) {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                            lootBox.Loot();
                    }
                    else {
                        showGuiSkin = false;
                    }
                    break;
                case "Tool":
                    Debug.Log("TOOL TOOL TOOLL MODAFUKA");
                    gotInfo = true;
                    //get the tool script               (and show text)                 (show text)
                    //ask tool manager if he has it, if so replace the current tool , otherwise pick up the tool 
                    //give it to tool manager and its stuff.
                    break;

            }
        }
        else {
            if (gotInfo) gotInfo = false;
            showGuiSkin = false;
        }
    }

    void OnGUI() {
        GUI.skin = guiSkin;
        if (showGuiSkin) {
            Rect rect = new Rect(Screen.width - (Screen.width / 1.7f),Screen.height - (Screen.height / 1.4f), 800f, 100f);
            GUI.Label(rect, "Press key >>E<< to Use"); 
        }
    }

}
