using UnityEngine;
using System.Collections;

public class MeleeEnemyScript : EnemyScript {

	float attackTimer;

    [SerializeField]
    float chargeSpeed;
    [SerializeField]
    float chargeAcceleration;

	override public void Start () {
		base.Start ();
		state = AIState.Running;
	}
	
	// Update is called once per frame
	void Update () {
	  switch (state) {
		case AIState.Attacking:
			Attacking ();
			break;

		case AIState.Running:
			agent.SetDestination (player.position);
			break;

		case AIState.Charge:
			agent.SetDestination (player.position);
			if (Vector3.Distance (agent.transform.position, player.transform.position) < 10) {
				Charge();
			}
			break;
		}
	}

	void Attacking () {
		attackTimer += Time.deltaTime;

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), 5);

        if (attackTimer > attackTime) {
			MeleeAttack ();
			attackTimer = 0;
		}
	}

	void MeleeAttack () {
		//play anim
		player.GetComponent<PlayerScript> ().TakeDamage (10);
	}

	void Charge () {
		agent.acceleration = chargeAcceleration;
		agent.speed = chargeSpeed;
	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag (Tags.player)) {
            agent.acceleration = defaultAcceleration;
            agent.speed = defaultSpeed;
			state = AIState.Attacking;
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.CompareTag (Tags.player)) {
			state = AIState.Running;
		}
	}


}
