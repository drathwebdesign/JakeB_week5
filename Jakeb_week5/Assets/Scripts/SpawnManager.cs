using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public GameObject[] enemyPrefabs;
    public int enemyCount;
    [SerializeField] float spawnRadius = 10f;
    [SerializeField] int baseEnemiesPerWave = 3;
    public int waveNumber = 1;
    [SerializeField] float waveCooldown = 5f;
    bool waveSpawning = false;

    private List<Transform> enemySpawnPoints = new List<Transform>();

    void Start() {
        // Find all spawn points
        GameObject[] points = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
        foreach (GameObject point in points) {
            enemySpawnPoints.Add(point.transform);
        }

        // Start the first wave
        StartCoroutine(SpawnEnemyWave(waveNumber));
    }

    void Update() {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0 && !waveSpawning) {
            // Increment the wave number and start the next wave
            waveNumber++;
            StartCoroutine(SpawnEnemyWave(waveNumber));
        }
    }

    IEnumerator SpawnEnemyWave(int waveNumber) {
        waveSpawning = true;
        yield return new WaitForSeconds(waveCooldown);

        if (waveNumber % 5 == 0) {
            // Boss wave: Spawn the last enemy prefab as the boss
            int bossCount = waveNumber / 5;  // 1 boss on wave 5, 2 bosses on wave 10, etc.

            foreach (Transform spawnPoint in enemySpawnPoints) {
                for (int j = 0; j < bossCount; j++) {
                    Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
                    Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                    Instantiate(enemyPrefabs[enemyPrefabs.Length - 1], spawnPosition, Quaternion.identity); // Spawn boss
                }
            }
        } else {
            // Regular wave
            int enemiesPerSpawn = baseEnemiesPerWave + (waveNumber - 1);

            foreach (Transform spawnPoint in enemySpawnPoints) {
                for (int j = 0; j < enemiesPerSpawn; j++) {
                    int randomIndex = Random.Range(0, enemyPrefabs.Length - 1);  // Exclude the boss enemy

                    Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
                    Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                    Instantiate(enemyPrefabs[randomIndex], spawnPosition, Quaternion.identity);
                }
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