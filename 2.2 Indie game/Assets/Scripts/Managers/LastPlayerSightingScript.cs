using UnityEngine;
using System.Collections;

public class LastPlayerSightingScript : MonoBehaviour {
    public Vector3 position = new Vector3(1000f, 1000f, 1000f);         // The last global sighting of the player.
    public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);    // The default position if the player is not in sight.

    //This script can be widely expanded... 
    //Take a look at this: https://unity3d.com/learn/tutorials/projects/stealth/game-controller?playlist=17168
}