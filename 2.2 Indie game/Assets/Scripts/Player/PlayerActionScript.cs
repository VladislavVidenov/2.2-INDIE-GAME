using UnityEngine;
using System.Collections;

public class PlayerActionScript : MonoBehaviour {
    ToolManager toolManager;
    float maxRayDistance = 2.0f;

    [SerializeField]
    LayerMask layerMask;
    public GUISkin guiSkin;
    bool showGuiSkin = false;

    RaycastHit hit;

    void Start()
    {
        toolManager = FindObjectOfType<ToolManager>();
    }
	// Update is called once per frame
	void Update () {
        Raycasting();
	}

    void Raycasting()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask.value))
        {
            switch (hit.collider.tag)
            {
                case "VendingMachine":
                    showGuiSkin = true;
                    if (Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.isWaving)
                        GameObject.Find("SceneManager").GetComponent<SceneChangeManager>().SetState(GameState.InBuyScreen);
                    break;

                case "LootableBox":
                    LootableBoxScript lootBox = hit.collider.gameObject.GetComponent<LootableBoxScript>();
                    if (!lootBox.isLooted)
                    {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                            lootBox.Loot();
                    }
                    else {
                        showGuiSkin = false;
                    }
                    break;

                case "Spawner":
                    SpawnEventScript spawnEvent = hit.collider.gameObject.GetComponent<SpawnEventScript>();
                    if (!spawnEvent.hasSpawned && spawnEvent.allowedToSpawn)
                    {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                            spawnEvent.Spawn();
                    }
                    else {
                        showGuiSkin = false;
                    }
                    break;

                case "TutDesk":
                    //SpawnEventScript spawnEvent = hit.collider.gameObject.GetComponent<SpawnEventScript>();
                    if (GameObject.Find("TutorialManager").GetComponent<TutorialScript>().finishedTutorial)
                    {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            Application.LoadLevel("CubeWorld");
                        }
                    }
                    else {
                        showGuiSkin = false;
                    }
                    break;

                case "LevelSwitcher":
                    showGuiSkin = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.gameObject.GetComponent<SceneSwitcher>().SwitchScene();
                    }


                    break;
                case "Tool":
                    toolManager.CheckTool(hit.transform.GetComponent<Tool>());
                    break;

                case "Engine":
                    if (!hit.collider.gameObject.GetComponent<EngineScript>().activated)
                    {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            hit.collider.gameObject.GetComponent<EngineScript>().StartEngine();
                        }
                    }
                    else {
                        showGuiSkin = false;
                    }
                    break;
            }

        }
        else {
            if (toolManager.showToolGuiText) toolManager.showToolGuiText = false;
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
