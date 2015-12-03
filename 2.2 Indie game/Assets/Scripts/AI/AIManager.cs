﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	public List<MeleeEnemyScript> meleeEnemies;
	public List<RangedEnemyScript> rangedEnemies;
	public List<RangedRushEnemy> rangedRushEnemies;

	int meleeEnemyRunning = 0;
	int meleeEnemyAttacking = 0;
	int meleeEnemyCharging = 0;
	int rangedEnemyShooting = 0;
	int rangedEnemyInCover = 0;
	int rangedRushEnemyShooting = 0;
	int rangedRushEnemyInCover = 0;

    PlayerMovement playerMovement;
	// Use this for initialization
	void Start () {
        playerMovement = GameObject.FindWithTag(Tags.player).GetComponent<PlayerMovement>();
		meleeEnemies = new List<MeleeEnemyScript> ();
		rangedEnemies = new List<RangedEnemyScript> ();
		rangedRushEnemies = new List<RangedRushEnemy> ();

		InvokeRepeating ("CheckEnemiesStates", 3, 3);
	}

	void CheckEnemiesStates () {

		meleeEnemyRunning = 0;
		meleeEnemyAttacking = 0;
		meleeEnemyCharging = 0;
		rangedEnemyShooting = 0;
		rangedEnemyInCover = 0;
		rangedRushEnemyShooting = 0;
		rangedRushEnemyInCover = 0;
        if (meleeEnemies.Count <= 0) return;

		foreach (MeleeEnemyScript enemy in meleeEnemies) {
	
			switch (enemy.state) {
			case AIState.Running:
				meleeEnemyRunning++;
				break;
			case AIState.Attacking:
				meleeEnemyAttacking++;
				break;

			case AIState.Charge:
				meleeEnemyCharging++;
				break;
			
			}
		}

		foreach (RangedEnemyScript enemy in rangedEnemies) {
			switch (enemy.state) {
			case AIState.Shooting:
				rangedEnemyShooting++;
				break;
			
			case AIState.InCover:
				rangedEnemyInCover++;
				break;
			}
		}

		foreach (RangedRushEnemy enemy in rangedRushEnemies) {
			switch (enemy.state) {
			case AIState.Shooting:
				rangedRushEnemyShooting++;
				break;
				
			case AIState.InCover:
				rangedRushEnemyInCover++;
				break;
			}
		}

		if (meleeEnemyAttacking == 0 && meleeEnemyCharging == 0) {
			meleeEnemies[Random.Range(0,meleeEnemies.Count - 1)].state = AIState.Charge;
		}

        if (playerMovement.isCrouching) {
            rangedRushEnemies[Random.Range(0, rangedRushEnemies.Count - 1)].state = AIState.Flank;
        }
	}
}







