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
        Vector3 direction = gameObject.transform.TransformDirection(Vector3.forward);
        Vector3 position = transform.position;
        if (Physics.Raycast(position, direction, out hit, maxRayDistance, layerMask.value)) {
            showGuiSkin = true;
            if (Input.GetKeyDown(KeyCode.E)) {
                GameObject target = hit.collider.gameObject;
                if (hit.collider.CompareTag("VendingMachine")) {
                    GameObject.Find("SceneManager").GetComponent<SceneChangeManager>().ChangeState(GameState.InBuyScreen);
                }
                //do action
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
