using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// This is going to be extended by the different enemy types.
/// </summary>


public class EnemyScript : MonoBehaviour {

    enum AIState { idle, patrolling, lowAlert, mediumAlert, highAlert, chasing, attacking }
    AIState state;

    [SerializeField] int health;
    [SerializeField] int creditsDropAmount = 5;

    //Patrolling
    [SerializeField] float patrolSpeed = 2.0f;
    [SerializeField] float patrolWaitTime = 1.0f;
    Vector3 currentDestination;
    float patrolTimer;
    int waypointIndex;

    //Attacking
    [SerializeField] float attackDistance = 3.0f;
    [SerializeField] float attackRate = 1.0f;

    //Chasing
    [SerializeField] float chaseDistance = 10.0f;
    [SerializeField] float chaseSpeed = 3.0f;
    [SerializeField] float chaseRotationSpeed = 5.0f;
    [SerializeField] float chaseWaitTime = 5f;
    float chaseTimer;

    float distanceToTarget;

    NavMeshAgent agent;
    public Transform[] patrolWaypoints;
    Transform target; //player

    //Used for RotateToward
    Quaternion rotation;

    EnemySightScript enemySight;
    LastPlayerSightingScript lastPlayerSighting;

    Vector3 rayDirection;


    void Awake () {
        //Getting the references
        agent = GetComponent<NavMeshAgent>();
        enemySight = GetComponent<EnemySightScript>();
        lastPlayerSighting = GameManager.Instance.GetComponent<LastPlayerSightingScript>();

        if (target == null) target = GameObject.FindWithTag(Tags.player).transform;
        InvokeRepeating("StateLogic", 0, 0.01f);

        StartCoroutine("StateMachine");
	}

    //debug update
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            TakeDamage(10);
        }
    }
    //------------


    IEnumerator StateMachine() {
        while (true) {
            switch (state) {
                case AIState.idle:
                    Debug.Log("Idle");
                    break;
                case AIState.patrolling:
                    Patrolling();
                    break;
                case AIState.lowAlert:
                    //magic
                    Debug.Log("Low Alert");
                    break;
                case AIState.mediumAlert:
                    //magic
                    Debug.Log("Medium Alert");
                    break;
                case AIState.highAlert:
                    //magic
                    Debug.Log("High Alert");
                    break;
                case AIState.chasing:
                    //magic
                    Chasing();
                    Debug.Log("Chasing");
                    break;
                case AIState.attacking:
                    Shooting();
                    Debug.Log("Attacking");
                    break;
            }
            yield return null;
        }
    }

    void StateLogic() {
        distanceToTarget = (target.position - transform.position).sqrMagnitude;

        if (enemySight.playerInSight /*and player == alive*/) {
            state = AIState.attacking;
        }
        // If player has been sighted by any of the enemies and isnt dead.
        else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition /*And player == alive*/) {
            state = AIState.chasing;
        }
        else {
            state = AIState.patrolling;
        }
    }

    void Shooting() {
        agent.Stop(); // Let the agent stand still to shoot.
    }

    void Chasing() {
        agent.Resume(); //Resume movement
        // Create a vector from the enemy to the last sighting of the player.
        Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;

        // If the the last personal sighting of the player is not close...
        if (sightingDeltaPos.sqrMagnitude > 4f)
            // ... set the destination for the NavMeshAgent to the last personal sighting of the player.
            agent.destination = enemySight.personalLastSighting;

        // Set the appropriate speed for the NavMeshAgent.
        agent.speed = chaseSpeed;

        // If near the last personal sighting...
        if (agent.remainingDistance <= agent.stoppingDistance) {
            // ... increment the timer.
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (chaseTimer >= chaseWaitTime) {
                // ... reset last global sighting, the last personal sighting and the timer.
                lastPlayerSighting.position = lastPlayerSighting.resetPosition;
                enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
                chaseTimer = 0f;
                Debug.Log("GOT RESET");
            }
        }
        else
            // If not near the last sighting personal sighting of the player, reset the timer.
            chaseTimer = 0f;
    }

    void Patrolling() {
        // Set an appropriate speed for the NavMeshAgent.
        agent.speed = patrolSpeed;

        // If near the next waypoint or there is no destination...
        if (agent.destination == lastPlayerSighting.resetPosition || agent.remainingDistance <= agent.stoppingDistance) {
            // ... increment the timer.
            patrolTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (patrolTimer >= patrolWaitTime) {
                // ... increment the wayPointIndex.
                if (waypointIndex == patrolWaypoints.Length - 1)
                    waypointIndex = 0;
                else
                    waypointIndex++;

                // Reset the timer.
                patrolTimer = 0;
            }
        }
        else
            // If not near a destination, reset the timer.
            patrolTimer = 0;

        // Set the destination to the patrolWayPoint.
        agent.destination = patrolWaypoints[waypointIndex].position;
        Debug.Log("ok");
    }

    void Die() {
        DropCredits(creditsDropAmount);
        GameObject.Destroy(this.gameObject);  //debug
    }

    void TakeDamage(int amount) {
        health -= amount;

        if (health <= 0) {
            Die();
        }
    }

    void DropCredits(int amount) {
        //instantiate creditsdropprefab?
    }
}
