using UnityEngine;
using System.Collections;

public class ArmorPickable : MonoBehaviour {
    PlayerScript player;
    void Start() {
        player = GameManager.Instance.playerScript;
    }

	void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player")) {
            player.IncreasePlayerStats(15, 0, 0, 0, 0);

            Destroy(gameObject);
        }
      
    }
}
