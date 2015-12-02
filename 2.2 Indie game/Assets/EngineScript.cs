using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineScript : MonoBehaviour {

    [SerializeField]
    List<SpawningScript> spawners;

    public void StartEngine() {
        foreach (SpawningScript spawn in spawners) {
            spawn.spawn = true;
        }
    }
}
