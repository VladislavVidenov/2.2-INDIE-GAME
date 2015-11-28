using UnityEngine;
using System.Collections;

public class MeleeEnemyScript : EnemyScript {
	
	// Use this for initialization
	void Start () {
		state = AIState.Running;
	}
	
	// Update is called once per frame
	void Update () {
	  switch (state) {
		case AIState.Attacking:
			Attacking();
			break;

		case AIState.Running:
			agent.SetDestination(player.position);
			break;
		}
	}

	void Attacking () {

	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag (Tags.player)) {
			print ("helooo");
		}
	}
}
