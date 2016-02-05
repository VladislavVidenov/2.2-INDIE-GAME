using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineScript : MonoBehaviour {

    [SerializeField]
    List<SpawningScript> spawners;

    public bool activated = false;

    public void StartEngine() {
        foreach (SpawningScript spawn in spawners) {
            if (spawn != null) spawn.spawn = true;
        }
    }
}
