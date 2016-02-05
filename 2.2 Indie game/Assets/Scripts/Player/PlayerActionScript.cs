using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerActionScript : MonoBehaviour {
    ToolManager toolManager;
    float maxRayDistance = 2.0f;

    [SerializeField]
    LayerMask layerMask;
    public GUISkin guiSkin;
    bool showGuiSkin = false;

    RaycastHit hit;

    public Image interactionImage;

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
                    if (GameObject.Find("TutorialManager").GetComponent<TutorialScript>().finishedTutorial)
                    {
                        showGuiSkin = true;
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            GameObject.Find("ScreenFader").GetComponent<FadeInout>().fade = true;
                            Invoke("LoadLevel", 4);
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
                        GameObject.Find("ScreenFader").GetComponent<FadeInout>().fade = true;
                        StartCoroutine(SwitchScene(hit));
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

    void LoadLevel() {
        Application.LoadLevel("CubeWorld");
    }

    IEnumerator SwitchScene(RaycastHit hitIn) {
        yield return new WaitForSeconds(4);
        hitIn.collider.gameObject.GetComponent<SceneSwitcher>().SwitchScene();
    }

    void OnGUI() {
        GUI.skin = guiSkin;

        if (interactionImage == null) { print("Please attach an interactionImage to the PlayerActionScript");  return; }
        
        if (showGuiSkin) {
            interactionImage.enabled = true;
        } else {
            interactionImage.enabled = false;
        }
    }

}
