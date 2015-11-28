using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// </summary>
public enum AIState { Running , Attacking}

public class EnemyScript : MonoBehaviour
{

    public AIState state;

    [Header("General")]
    [SerializeField] int health;
    [SerializeField] int creditsDropAmount = 5;
	

    public NavMeshAgent agent;
    public Transform player; //target

	[HideInInspector]
	public SpawningScript spawner;
	


//    void Attacking()
//    {
//        if (!relocating) FindFightingPosition();
//        if (agent.remainingDistance <= agent.stoppingDistance) relocating = false;
//        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);
//
//        if (Time.time - attackTimer > attackTime)
//        {
//            Shoot();
//            attackTimer = Time.time;
//        }
//    }

//    void Shoot()
//    {
//        Vector3 direction = player.transform.position - this.transform.position;
//        direction += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
//        StartCoroutine(ShootRaycast(direction));
//    }

//    void FindFightingPosition()
//    {
//        if (Random.Range(0, 50) == 4)
//        {
//            agent.Resume();
//            agent.updateRotation = false;
//
//            Vector3 delta = (this.transform.position - player.transform.position);
//
//            fightingPosition = player.transform.position + new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
//            Vector3 dirToPoint = fightingPosition - player.transform.position;
//            if (Vector3.Dot(delta, dirToPoint) > 0)
//            {
//                RaycastHit hit;
//                Vector3 dir = (player.transform.position - fightingPosition);
//
//                if (Physics.Raycast(fightingPosition, dir, out hit, 11f))
//                {
//                    Debug.DrawRay(fightingPosition, dir, Color.black, 5f);
//                    if (hit.collider.CompareTag(Tags.player))
//                    {
//                        Debug.Log("I FOUND IT");
//                        agent.SetDestination(fightingPosition);
//                        relocating = true;
//                    }
//                }
//            }
//        }
//        else
//        {
//            agent.Stop();
//        }
//    }

//    void Chasing()
//    {
//        agent.Resume(); //Resume movement
//        // Create a vector from the enemy to the last sighting of the player.
//        Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
//
//        // If the the last personal sighting of the player is not close...
//        if (sightingDeltaPos.sqrMagnitude > 4f)
//            // ... set the destination for the NavMeshAgent to the last personal sighting of the player.
//            agent.destination = enemySight.personalLastSighting;
//
//        // Set the appropriate speed for the NavMeshAgent.
//        agent.speed = chaseSpeed;
//
//        // If near the last personal sighting...
//        if (agent.remainingDistance <= agent.stoppingDistance)
//        {
//            // ... increment the timer.
//            LookAround();
//            chaseTimer += Time.deltaTime;
//
//            // If the timer exceeds the wait time...
//            if (chaseTimer >= chaseWaitTime)
//            {
//                // ... reset last global sighting, the last personal sighting and the timer.
//                lastPlayerSighting.position = lastPlayerSighting.resetPosition;
//                enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
//                chaseTimer = 0f;
//
//                chaseRotated = 0; // reset rotation amount 
//            }
//        }
//        else
//            // If not near the last sighting personal sighting of the player, reset the timer.
//            chaseTimer = 0f;
//    }
	
//    IEnumerator ShootRaycast(Vector3 direction)
//    {
//        yield return new WaitForSeconds(0.1f);
//        RaycastHit hit;
//
//        if (Physics.Raycast(transform.position + transform.up / 3, direction.normalized, out hit, 100f))
//        {
//            Debug.DrawRay(transform.position + transform.up / 3, direction, Color.red);
//            if (hit.collider.CompareTag(Tags.player))
//            {
//                Debug.Log("i shot u");
//            }
//            else
//            {
//                Debug.Log("i missed u");
//            }
//        }
//    }

    void Dying()
    {
	
        //DIE PLEASE!
		spawner.KillEnemy ();
        //DropCredits(creditsDropAmount);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Dying();
        }
    }

    void DropCredits(int amount)
    {
        //instantiate creditsdropprefab?
    }
}
