using UnityEngine;
using System.Collections;

/// <summary>
/// This is the General EnemyScript.
/// </summary>
public enum AIState { Running , Attacking, FindPlayerInSight , FindCover, InCover, MovingToCover, Shooting, Charge,Flank,HuntEnemy}

public class EnemyScript : MonoBehaviour
{
	//public delegate void EnemyDied ();
	//public static event EnemyDied OnEnemyDeath;
    public AIState state;

    [Header("General")]
    public int health;
	public int damage;
    [SerializeField] 
	int bitsDropAmount = 20;

   
    public float defaultSpeed = 3.5f;
    public float defaultAcceleration = 8f;

    public NavMeshAgent agent;
	[HideInInspector]
    public Transform player; //target

    [HideInInspector]
    public Transform playerHeadPos;
    [HideInInspector]
    public Transform enemyHeadPos;

    [HideInInspector]
    public SpawningScript spawner;

    public float attackTime = 3f;

    public LayerMask layer;

	//Animation part
	[HideInInspector]
	public Animator myAnimator;
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip[] enemySounds;

    bool hasDied = false;

	public virtual void Start () {

        audioSource = GetComponent<AudioSource>();


		myAnimator = GetComponentInChildren<Animator> ();
		player = GameObject.FindGameObjectWithTag (Tags.player).transform;
        playerHeadPos = player.GetChild(0).transform;
        enemyHeadPos = agent.gameObject.transform.GetChild(0).transform;
	}

    public virtual void Dying()
    {
        print("dieieied");

        if (!hasDied)
        {
            hasDied = true;
            print("yo mama");
            AudioSource.PlayClipAtPoint(enemySounds[0], gameObject.transform.position, 0.1f);
            if (spawner != null) spawner.KillEnemy(); //Event ! -vladimir.:D
            player.GetComponent<PlayerScript>().IncreasePlayerStats(0, 0, 0, 0, bitsDropAmount); //maybe also?
            DropPackage();
            Destroy(gameObject);
        }
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Dying();
        }
    }

    void DropPackage()
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
