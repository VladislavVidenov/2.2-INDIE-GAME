using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct WaveInfo  {
	public int meleeEnemy;
	public int rangedEnemy;
    public int rangedRushEnemy;
}

public class SpawningScript : MonoBehaviour { 
	
	public enum SpawnTypes {
		Once,
		Wave,
		TimedWave
	}
	[SerializeField] WaveInfo[] waves;

	[SerializeField] List<Transform> spawnPoints;
	int spawnIndex = 0;

	//enemy prefabs
	[SerializeField] GameObject meleeEnemyPrefab;
	[SerializeField] GameObject rangedEnemyPrefab;
    [SerializeField] GameObject rangedRushEnemyPrefab;

	//enemies created
	private int remainingEnemy = 0;

	private int SpawnID;

	//wave types
	private bool waveSpawn = false;
	public bool spawn = false;
	public SpawnTypes spawnType = SpawnTypes.TimedWave;
	//timed waves
	public float waveTimer = 30f;
	private float timeTillWave = 0f;
	//wave controls
	private int totalWaves;
	private int numWaves = 0;

	[SerializeField] float timeBetweenWaves;
    private float betweenWavesTimer = 9;

	AIManager manager;

	void Start () {
		manager = GameObject.FindGameObjectWithTag (Tags.aiManager).GetComponent<AIManager>();
		totalWaves = waves.Length;
	}

	void Update () {
		if (spawn) {
			switch(spawnType) {

			case SpawnTypes.Once:

					// spawns an enemy
				for(int i = 0; i < waves[0].meleeEnemy ; i++) {
					SpawnEnemy(meleeEnemyPrefab);
				}
				for(int j = 0; j < waves[0].rangedEnemy ; j++) {
					SpawnEnemy(rangedEnemyPrefab);
				}
                for (int k = 0; k < waves[numWaves - 1].rangedRushEnemy; k++) {
                    SpawnEnemy(rangedRushEnemyPrefab);
                }
					spawn =false;
				

				break;

			case SpawnTypes.Wave:

                if (numWaves < totalWaves + 1) {
                    if (waveSpawn) {
                        GameManager.Instance.isWaving = true;
                        for (int i = 0; i < waves[numWaves - 1].meleeEnemy; i++) {
                            SpawnEnemy(meleeEnemyPrefab);
                        }
                        for (int j = 0; j < waves[numWaves - 1].rangedEnemy; j++) {
                            SpawnEnemy(rangedEnemyPrefab);
                        }
                        for (int k = 0; k < waves[numWaves - 1].rangedRushEnemy; k++) {
                            SpawnEnemy(rangedRushEnemyPrefab);
                        }
                        //spawns an enemy
                        waveSpawn = false;
                    }

                    if (remainingEnemy == 0) {
                        GameManager.Instance.isWaving = false;
                        //start the betweenWavesTimer
                        betweenWavesTimer += Time.deltaTime;

                        if (betweenWavesTimer >= timeBetweenWaves) {

                            // enables the wave spawner
                            betweenWavesTimer = 0;
                            waveSpawn = true;
                            //increase the number of waves
                            numWaves++;

                        }
                    }
				}else{spawn = false;}
				break;

			case SpawnTypes.TimedWave:
				// checks if the number of waves is bigger than the total waves
				if(numWaves <= totalWaves)
				{
					// Increases the timer to allow the timed waves to work
					timeTillWave += Time.deltaTime;
					if (waveSpawn)
					{
						for(int i = 0; i < waves[numWaves -1].meleeEnemy ; i++) {
							SpawnEnemy(meleeEnemyPrefab);
						}
						for(int j = 0; j < waves[numWaves -1].rangedEnemy ; j++) {
							SpawnEnemy(rangedEnemyPrefab);
						}
                        for (int k = 0; k < waves[numWaves - 1].rangedRushEnemy; k++) {
                            SpawnEnemy(rangedRushEnemyPrefab);
                        }
						//spawns an enemy
						waveSpawn = false;
					}
					// checks if the time is equal to the time required for a new wave
					if (timeTillWave >= waveTimer)
					{
						// enables the wave spawner
						waveSpawn = true;
						// sets the time back to zero
						timeTillWave = 0.0f;
						// increases the number of waves
						numWaves++;
						// A hack to get it to spawn the same number of enemies regardless of how many have been killed
						remainingEnemy = 0;
					}
				}
				else
				{
					spawn = false;
				}
				break;
			}
		}
	}

	private void SpawnEnemy (GameObject pEnemy){
		Vector3 spawnPos = spawnPoints [spawnIndex].position;
		spawnIndex++;
		if (spawnIndex >= spawnPoints.Count) {
			spawnIndex = 0;
		}
		GameObject enemy = Instantiate (pEnemy, spawnPos, Quaternion.identity) as GameObject;

		//enemy.GetComponent<EnemyScript> ().spawner = this.GetComponent<SpawningScript> ();

		if (enemy.GetComponent<MeleeEnemyScript> () != null) {
			manager.meleeEnemies.Add (enemy.GetComponent<MeleeEnemyScript>());
		} else if (enemy.GetComponent<RangedEnemyScript> () != null) {
			manager.rangedEnemies.Add (enemy.GetComponent<RangedEnemyScript>());
		} else {
			manager.rangedRushEnemies.Add (enemy.GetComponent<RangedRushEnemy>());
		}

		remainingEnemy++;
	}

    void KillEnemy () {
			remainingEnemy--;
	}

	void OnEnable(){
		EnemyScript.OnEnemyDeath += KillEnemy;
	}
	void OnDisable(){
		EnemyScript.OnEnemyDeath -= KillEnemy;
	}

}
