using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// </summary>

public class EnemyScript : MonoBehaviour {

    enum AIState { patrolling, charging, chasing, searching, guarding, attacking }
    AIState state;
    
    [SerializeField] int health;
    [SerializeField] int creditsDropAmount = 5;

    [SerializeField] float defaultRange = 10;
    [SerializeField] float alertedRange = 20;

    //Patrolling
    public Transform[] patrolWaypoints;
    [SerializeField] float patrolSpeed = 2.0f;
    [SerializeField] float patrolWaitTime = 1.0f;
    Vector3 currentDestination;
    float patrolTimer;
    int waypointIndex;
    int waypointRounds = 0;

    //Charging
    public Transform chargingSpot;

    //Guarding
    public Transform guardingSpot;

    //Attacking
    //[SerializeField] float attackDistance = 3.0f;
    //[SerializeField] float attackRate = 1.0f;
    [SerializeField] float attackRotationSpeed = 1f;

    //Chasing
    [SerializeField] float chaseSpeed = 3.0f;
    [SerializeField] float chaseWaitTime = 5f;
    float chaseTimer;

    //float distanceToTarget;

    NavMeshAgent agent;
    
    Transform target; //player

    EnemySightScript enemySight;
    LastPlayerSightingScript lastPlayerSighting;

    Vector3 rayDirection;

    float time;
    int rotated = 0;
    bool relocating = false;
    public float maxDistance = 7f;

    Vector3 point;

    void Awake () {
        time = Time.time;
        //Getting the references
        agent = GetComponent<NavMeshAgent>();
        enemySight = GetComponent<EnemySightScript>();
        lastPlayerSighting = GameManager.Instance.GetComponent<LastPlayerSightingScript>();
        
        if (target == null) target = GameObject.FindWithTag(Tags.player).transform;

        InvokeRepeating("StateLogic", 0, 0.01f);
        StartCoroutine("StateMachine");
	}

    IEnumerator StateMachine() {
        while (true) {
            switch (state) {
                case AIState.patrolling:
                    Patrolling();
                    break;
                case AIState.chasing:
                    agent.updateRotation = true;
                    Chasing();
                    Debug.Log("Chasing");
                    break;
                case AIState.charging:
                    Charging();
                    Debug.Log("Charging");
                    break;
                case AIState.guarding:
                    //magic
                    Debug.Log("Guarding");
                    break;
                case AIState.searching:
                    //magic
                    Debug.Log("Searching");
                    break;
                case AIState.attacking:
                    Attacking();
                    Debug.Log("Attacking");
                    break;
            }
            yield return null;
        }
    }

    void StateLogic() {
        if (enemySight.playerInSight /*and player == alive*/) {
            enemySight.sphereCollider.radius = alertedRange;
            state = AIState.attacking;
        }
        // If player has been sighted by any of the enemies and isnt dead.
        else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition /*And player == alive*/) {
            enemySight.sphereCollider.radius = alertedRange;
            state = AIState.chasing;
        }
        //else {
        //    enemySight.sphereCollider.radius = defaultRange;
        //    state = AIState.patrolling;
        //}
    }

    void Attacking() {
        if (!relocating) FindFightingPosition(); 
        if (agent.remainingDistance <= agent.stoppingDistance) relocating = false;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(target.transform.position - this.transform.position), attackRotationSpeed);

        if (Time.time - time > 2f) {
            Shoot();
            time = Time.time;
        }
    }

    void Shoot() {
        Vector3 direction = target.transform.position - this.transform.position;
        direction += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        StartCoroutine(ShootRaycast(direction));
    }

    void FindFightingPosition() {
        if (Random.Range(0, 50) == 4) {
            agent.Resume();
            agent.updateRotation = false;

            Vector3 delta = (this.transform.position - target.transform.position);

            point = target.transform.position + new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
            Vector3 dirToPoint = point - target.transform.position;
            if (Vector3.Dot(delta, dirToPoint) > 0) {
                RaycastHit hit;
                Vector3 dir = (target.transform.position -point);
           
                if (Physics.Raycast(point, dir, out hit, 11f)) {
                    Debug.DrawRay(point, dir, Color.black,5f);
                    if (hit.collider.CompareTag(Tags.player)) {
                        Debug.Log("I FOUND IT");
                        agent.SetDestination(point);
                        relocating = true;
                    }
                }
            }
        }
        else {
            agent.Stop();
        }
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
            LookAround();
            chaseTimer += Time.deltaTime;

            // If the timer exceeds the wait time...
            if (chaseTimer >= chaseWaitTime) {
                // ... reset last global sighting, the last personal sighting and the timer.
                lastPlayerSighting.position = lastPlayerSighting.resetPosition;
                enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
                chaseTimer = 0f;

                rotated = 0; // reset rotation amount 
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
                if (waypointIndex == patrolWaypoints.Length - 1) {
                    waypointIndex = 0;
                    waypointRounds++;
                    print("waypointRounds: " + waypointRounds);
                    if (waypointRounds == 2) {
                        state = AIState.charging;
                        waypointRounds = 0;
                    }
                }
                else {
                    waypointIndex++;
                }
                // Reset the timer.
                patrolTimer = 0;
            }
        }
        else
            // If not near a destination, reset the timer.
            patrolTimer = 0;

        // Set the destination to the patrolWayPoint.
        agent.destination = patrolWaypoints[waypointIndex].position;
    }

    void Charging() {
        agent.SetDestination(chargingSpot.position);
        if (agent.remainingDistance <= agent.stoppingDistance) {
            Transform spotchild = chargingSpot.GetChild(0);

            Quaternion rotation = Quaternion.LookRotation(spotchild.position - agent.transform.position);
            rotation.x = 0;
            rotation.z = 0;
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rotation, Time.deltaTime * 2);
            
        }
    }

    void LookAround() { //improve this for guarding?
        if (!(rotated > 270)) {
            this.transform.Rotate(0, 1, 0);
        } 
        rotated++;
    }

    IEnumerator ShootRaycast(Vector3 direction) {
        yield return new WaitForSeconds(0.1f);
        RaycastHit hit;

        if (Physics.Raycast(transform.position + transform.up / 3, direction.normalized, out hit, 100f)) {
            Debug.DrawRay(transform.position + transform.up / 3, direction, Color.red);
            if (hit.collider.CompareTag(Tags.player)) {
                Debug.Log("i shot u");
            }
            else {
                Debug.Log("i missed u");
            }
        }
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
