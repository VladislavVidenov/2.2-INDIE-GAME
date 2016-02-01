﻿using UnityEngine;
using System.Collections;

public class LootableBoxScript : MonoBehaviour {

    [SerializeField]
    int bits;

    public bool isLooted = false;

    public void Loot() {
        isLooted = true;
        GameManager.Instance.Player.GetComponent<PlayerScript>().IncreasePlayerStats(0, 0, 0, 0, bits);
    }
}
