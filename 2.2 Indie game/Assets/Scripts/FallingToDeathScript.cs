using UnityEngine;
using System.Collections;

public class FallingToDeathScript : MonoBehaviour {

    public int AllowedToDie = 3;
    public string levelName = "";
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.player)) {
            if (AllowedToDie == 0) {
                Application.LoadLevel(levelName);
            }
            else {
                AllowedToDie--;
                other.GetComponent<PlayerScript>().Respawning();
            }
        }
    }
}
