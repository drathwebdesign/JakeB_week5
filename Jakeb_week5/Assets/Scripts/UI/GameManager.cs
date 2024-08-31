using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance; // Singleton instance

    public GameObject gameOverCanvas;
    private bool isGameOver = false;

    void Awake() {
        // Set up the singleton instance
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        gameOverCanvas.SetActive(false);
    }


    void Update() {
        if (isGameOver) return;
    }

    public void PlayerDied() {
        isGameOver = true;
        gameOverCanvas.SetActive(true);
        //stop spawning new monsters
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        {
            spawnManager.CancelInvoke("SpawnEnemyWave");
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}