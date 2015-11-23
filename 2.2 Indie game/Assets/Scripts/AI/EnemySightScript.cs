using UnityEngine;
using System.Collections;

public class EnemySightScript : MonoBehaviour {

    [SerializeField]
    float fieldOfViewAngle = 110f;
    public bool playerInSight;
    public Vector3 personalLastSighting;

    NavMeshAgent navMeshAgent;
    SphereCollider sphereCollider;
    LastPlayerSightingScript lastPlayerSighting;

    GameObject player;

    Vector3 previousSighting;

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        lastPlayerSighting = GameManager.Instance.GetComponent<LastPlayerSightingScript>();
        player = GameObject.FindGameObjectWithTag(Tags.player);

        // Set the personal sighting and the previous sighting to the reset position.
        personalLastSighting = lastPlayerSighting.resetPosition;
        previousSighting = lastPlayerSighting.resetPosition;
    }

    void Update() {
        // If the last global sighting of the player has changed...
        if (lastPlayerSighting.position != previousSighting)
            // ... then update the personal sighting to be the same as the global sighting.
            personalLastSighting = lastPlayerSighting.position;

        // Set the previous sighting to the be the sighting from this frame.
        previousSighting = lastPlayerSighting.position;
    }

    void OnTriggerStay(Collider other) {
        // If the player has entered the trigger sphere...
        if (other.gameObject == player) {
            // By default the player is not in sight.
            playerInSight = false;

            // Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // If the angle between forward and where the player is, is less than half the angle of view...
            if (angle < fieldOfViewAngle * 0.5f) {
                RaycastHit hit;

                // ... and if a raycast towards the player hits something...
                if (Physics.Raycast(transform.position + transform.up/3, direction.normalized, out hit, sphereCollider.radius)) {
                  //  Debug.DrawRay(transform.position + transform.up/3, direction, Color.red);
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject == player) {
                        // ... the player is in sight.
                        playerInSight = true;

                        // Set the last global sighting is the players current position.
                        lastPlayerSighting.position = player.transform.position;
                    }
                }
            }

            if (!player.GetComponent<PlayerMovement>().isCrouching) {
                personalLastSighting = player.transform.position;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        // If the player leaves the trigger zone...
        if (other.gameObject == player)
            // ... the player is not in sight.
            playerInSight = false;
    }
}
