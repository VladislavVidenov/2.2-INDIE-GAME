using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// </summary>
public enum AIState { Running , Attacking, FindPlayerInSight , FindCover, InCover, MovingToCover, Shooting, Charge}

public class EnemyScript : MonoBehaviour
{

    public AIState state;

    [Header("General")]
    [SerializeField] int health;
    [SerializeField] int creditsDropAmount = 5;
	

    public NavMeshAgent agent;
	[HideInInspector]
    public Transform player; //target

	[HideInInspector]
	public SpawningScript spawner;

	public void Start () {
		player = GameObject.FindGameObjectWithTag (Tags.player).transform;
	}

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
