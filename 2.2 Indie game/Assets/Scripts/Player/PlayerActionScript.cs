using UnityEngine;
using System.Collections;

public class PlayerActionScript : MonoBehaviour {

    float maxRayDistance = 2.0f;
    [SerializeField]
    LayerMask layerMask;
    public GUISkin guiSkin;
    bool showGuiSkin = false;
    RaycastHit hit;

	// Update is called once per frame
	void Update () {
        Raycasting();
	}

    void Raycasting() {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green);
        if (Physics.Raycast(ray,out hit, maxRayDistance, layerMask.value)) { 
            
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
            }
        }
        else {
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
