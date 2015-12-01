using UnityEngine;
using System.Collections;

public class LootableBoxScript : MonoBehaviour {

    [SerializeField]
    int scrap;
    [SerializeField]
    int electronics;

    public bool isLooted = false;

    public void Loot() {
        isLooted = true;
        GameManager.Instance.Player.GetComponent<PlayerScript>().IncreasePlayerStats(0, 0, scrap,electronics);
    }
}
