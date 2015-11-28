using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawningScript : MonoBehaviour { 

	public enum SpawnTypes {
		Once,
		Wave,
		TimedWave
	}

	[SerializeField] List<Transform> spawnPoints;
	int spawnIndex = 0;

	//enemy prefabs
	[SerializeField] GameObject meleeEnemyPrefab;
	[SerializeField] GameObject rangedEnemyPrefab;

	//enemies created
	public int totalEnemy = 10;
	private int remainingEnemy = 0;
	private int spawnedEnemy = 0;

	private int SpawnID;

	//wave types
	private bool waveSpawn = false;
	public bool spawn = true;
	public SpawnTypes spawnType = SpawnTypes.TimedWave;
	//timed waves
	public float waveTimer = 30f;
	private float timeTillWave = 0f;
	//wave controls
	public int totalWaves = 5;
	private int numWaves = 0;

	public float timeBetweenWaves;
	private float betweenWavesTimer;


	void Start () {
		SpawnID = Random.Range (1, 500);
	}

	void Update () {

		if (spawn) {
			switch(spawnType) {

			case SpawnTypes.Once:
				if(spawnedEnemy >= totalEnemy)
				{
					//sets the spawner to false
					spawn = false;
				}
				else
				{
					// spawns an enemy
					SpawnEnemy();
				}
				break;

			case SpawnTypes.Wave:

				if(numWaves < totalWaves + 1)
				{
					if (waveSpawn)
					{
						//spawns an enemy
						SpawnEnemy();
						Debug.Log("SPWANWNIF");
					}
					if (remainingEnemy == 0)
					{
						//start the betweenWavesTimer
						betweenWavesTimer += Time.deltaTime;

						if (betweenWavesTimer >= timeBetweenWaves) {
							// enables the wave spawner
							betweenWavesTimer = 0;
							waveSpawn = true;
							//increase the number of waves
							numWaves++;
							print (numWaves);

						}
					}
					if(remainingEnemy == totalEnemy)
					{

						// disables the wave spawner
						waveSpawn = false;
					}
				}
				break;

			case SpawnTypes.TimedWave:
				// checks if the number of waves is bigger than the total waves
				if(numWaves <= totalWaves)
				{
					// Increases the timer to allow the timed waves to work
					timeTillWave += Time.deltaTime;
					if (waveSpawn)
					{
						//spawns an enemy
						SpawnEnemy();
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
					if(remainingEnemy >= totalEnemy)
					{
						// diables the wave spawner
						waveSpawn = false;
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

	private void SpawnEnemy (){
		Vector3 spawnPos = spawnPoints [spawnIndex].position;
		spawnIndex++;
		if (spawnIndex >= spawnPoints.Count) {
			spawnIndex =0;
		}
		GameObject enemy = Instantiate (meleeEnemyPrefab, spawnPos, Quaternion.identity) as GameObject;
		enemy.GetComponent<EnemyScript> ().spawner = this.GetComponent<SpawningScript> ();
	//	enemy.SendMessage ("SetID", SpawnID);

		remainingEnemy++;
		spawnedEnemy++;
	}

	public void KillEnemy () {

	
			remainingEnemy--;

	}

	public void EnableSpawner (int sID) {
		
		if (SpawnID == sID) {
			spawn = true;
		}
	}

	public void DisableSpawner (int sID) {
		
		if (SpawnID == sID) {
			spawn = false;
		}
	}

	public float TimeTillWave
	{
		get
		{
			return timeTillWave;
		}
	}

	public void EnableTrigger()
	{
		spawn = true;
	}
}
