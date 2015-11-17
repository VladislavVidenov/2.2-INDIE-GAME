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
    [SerializeField] float visionRange;
    [SerializeField] float visionWidth;

    //Patrolling
    [SerializeField] float patrolSpeed = 5.0f;
                     Vector3 currentDestination;

    //Attacking
    [SerializeField] float attackDistance = 3.0f;
    [SerializeField] float attackRate = 1.0f;

    //Chasing
    [SerializeField] float chaseDistance = 10.0f;
    [SerializeField] float chaseSpeed = 3.0f;
    [SerializeField] float chaseRotationSpeed = 5.0f;

    float distanceToTarget;

    NavMeshAgent agent;
    public Transform[] waypoints;
    Transform target; //player

    //Used for RotateToward
    Quaternion rotation;


    void Start () {
        agent = GetComponent<NavMeshAgent>();

        if (target == null) target = GameObject.FindWithTag("Player").transform;
        InvokeRepeating("StateLogic", 0, 0.1f);
        StartCoroutine("StateMachine");
	}

    //debug update
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            agent.SetDestination(waypoints[1].position);
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
                    Debug.Log("Patrolling");
                    //RotateToward()
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
                    Debug.Log("Chasing");
                    break;
                case AIState.attacking:
                    //magic
                    Debug.Log("Attacking");
                    break;
            }
            yield return null;
        }
    }

    void StateLogic() {
        distanceToTarget = (target.position - transform.position).sqrMagnitude;

        if (distanceToTarget <= attackDistance * attackDistance) {
            state = AIState.attacking;
        }
        else if (distanceToTarget <= chaseDistance * chaseDistance) {
            state = AIState.chasing;
        }
        else {
            state = AIState.patrolling;
        }
    }

    void RotateToward(Vector3 targetPosition, float rotationSpeed) {
        targetPosition.y = transform.position.y; // set Y to be equal to its own to prevent some weird stuff.
        rotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    void MoveForward(float moveSpeed) {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
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
