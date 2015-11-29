using UnityEngine;
using System.Collections;

public class RangedEnemyScript : EnemyScript {
    Vector3 fightingPosition;
    bool relocating;
    float attackTimer;
    [SerializeField] float attackTime = 2f;
    [SerializeField] float attackRotationSpeed = 5f;

	CoverSpotScript coverSpot;


    void Start() {
		base.Start ();
        state = AIState.FindPlayerInSight;
    }

    void Update() {
		switch (state) {
		case AIState.FindPlayerInSight:
			FindPlayerInSight();
			break;
		case AIState.Shooting:
			Shooting ();
			break;
		case AIState.FindCover:
			FindCover ();
			break;
		case AIState.InCover:
			InCover ();
			break;
		}
	}

	void Shooting(){
		agent.Stop();
	}

	void FindCover(){
		
	}
	void InCover(){
		
	}

	void FindPlayerInSight(){
		agent.SetDestination (player.transform.position);
		Vector3 dir = player.transform.position - agent.transform.position;
		RaycastHit hit;
		Debug.DrawRay (agent.transform.position, dir * 100, Color.cyan);
		if (Physics.Raycast (agent.transform.position,dir,out hit,100f)) {
			if(hit.collider.CompareTag("Player")){
				state = AIState.Shooting;
			}
		}
	}

    void Attacking() {
        if (!relocating) FindFightingPosition();
        if (agent.remainingDistance <= agent.stoppingDistance) relocating = false;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);

        if (Time.time - attackTimer > attackTime) {
            Shoot();
            attackTimer = Time.time;
        }
    }

    void Shoot() {
        //magic
    }


    void FindFightingPosition() {
        if (Random.Range(0, 50) == 4) {
            agent.Resume();
            agent.updateRotation = false;

            Vector3 delta = (this.transform.position - player.transform.position);

            fightingPosition = player.transform.position + new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
            Vector3 dirToPoint = fightingPosition - player.transform.position;
            if (Vector3.Dot(delta, dirToPoint) > 0) {
                RaycastHit hit;
                Vector3 dir = (player.transform.position - fightingPosition);

                if (Physics.Raycast(fightingPosition, dir, out hit, 11f)) {
                    Debug.DrawRay(fightingPosition, dir, Color.black, 5f);
                    if (hit.collider.CompareTag(Tags.player)) {
                        Debug.Log("I FOUND IT");
                        agent.SetDestination(fightingPosition);
                        relocating = true;
                    }
                }
            }
        }
        else {
            agent.Stop();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.player)) {
            print("helooo");
        }
    }
}
