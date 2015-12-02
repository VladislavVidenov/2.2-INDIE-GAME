using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// </summary>
public enum AIState { Running , Attacking, FindPlayerInSight , FindCover, InCover, MovingToCover, Shooting, Charge,Flank}

public class EnemyScript : MonoBehaviour
{

    public AIState state;

    [Header("General")]
    [SerializeField] int health;
    [SerializeField] int creditsDropAmount = 5;
	

    public NavMeshAgent agent;
	[HideInInspector]
    public Transform player; //target
    public Transform playerHead;

	[HideInInspector]
	public SpawningScript spawner;

	public virtual void Start () {
		player = GameObject.FindGameObjectWithTag (Tags.player).transform;
        playerHead = player.GetChild(0).transform;
        print(playerHead.name);
	}

    public virtual void Dying()
    {
        //DIE PLEASE!
		spawner.KillEnemy ();
        player.GetComponent<PlayerScript>().IncreasePlayerStats(0, 0, 20, 0);
        DropCredits();
        Destroy(gameObject);
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Dying();
        }
    }

    void DropCredits()
    {
        if (Random.Range(0, 2) == 1)
        {
            if (Random.Range(0, 2) == 1)
            {
                Instantiate(Resources.Load("Pickable-Ammo"), agent.transform.position + (-transform.up), Quaternion.identity);
            }
            else
            {
                Instantiate(Resources.Load("Pickable-Armor"), agent.transform.position + (-transform.up) , Quaternion.identity);
            }
        }
    }
}
