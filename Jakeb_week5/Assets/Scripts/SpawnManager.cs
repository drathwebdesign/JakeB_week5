using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int enemyCount;
    [SerializeField] float spawnRadius = 10f;
    [SerializeField] int baseEnemiesPerWave = 3;
    public int waveNumber = 1;
    float waveCooldown = 5f;
    bool waveSpawning = false;

    private List<Transform> enemySpawnPoints = new List<Transform>();

    void Start()
    {
        // Find all spawn points
        GameObject[] points = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
        foreach (GameObject point in points) {
            enemySpawnPoints.Add(point.transform);
        }

        //make caroutine
        StartCoroutine(SpawnEnemyWave(waveNumber));
    }


    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0 && !waveSpawning) {
            //make caroutine
            StartCoroutine(SpawnEnemyWave(waveNumber));
        }
    }

    //make ienumerator
    IEnumerator SpawnEnemyWave(int waveNumber) {
        waveSpawning = true;
        yield return new WaitForSeconds(waveCooldown);
        int enemiesPerSpawn = baseEnemiesPerWave + (waveNumber - 1);

        foreach (Transform spawnPoint in enemySpawnPoints) {
            //j is inner loop which runs after outer loop 
            for (int j = 0; j < enemiesPerSpawn; j++) {
                // Randomly choose an enemy from the array
                int randomIndex = Random.Range(0, enemyPrefabs.Length);

                // Calculate a random position within the spawn radius
                Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);

                // Spawn the enemy at the calculated position
                Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
            }
        }
        waveSpawning = false;
    }
}

/*    void SpawnEnemies() {
        int enemiesPerSpawn = 5;

        for (int i = 0; i < enemySpawnPoints.Count; i++) {
            //J is inner loop which runs after outer loop 
            for (int j = 0; j < enemiesPerSpawn; j++) {
                //spawn random enemy
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomIndex], enemySpawnPoints[i].position, Quaternion.identity);
            }
        }
    }*/