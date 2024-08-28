using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour {
    public EnemyStats enemyStats;
    private int currentHealth;
    private Rigidbody rb;
    private Transform playerTransform;

    //attacking
    public Collider hitCollider;

    public GameObject[] itemPrefabs;

    //Animations
    private bool isWalking;
    private bool isDieing;
    private bool isAttacking;

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        currentHealth = enemyStats.maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (isDieing) return;
        findPlayer();
    }

    //Attacking
    void Attack() {
        isAttacking = true;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        hitCollider.enabled = true; // Enable the hit collider
        yield return new WaitForSeconds(1f); // Adjust duration as needed for the attack animation
        hitCollider.enabled = false; // Disable the hit collider
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other) {
        if (isAttacking && other.CompareTag("Player")) {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null) {
                playerMovement.TakeDamage(enemyStats.damage); // Deal damage based on stats
            }
        }
    }

    // Method to take damage
    public void TakeDamage(int damage, Vector3 knockbackDirection, float knockbackForce) {
        if (isDieing) return; // Do nothing if the is already Dieing

        currentHealth -= damage;
        //knockback
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);

        if (currentHealth <= 0) {
            Die();
        }
    }

    void findPlayer() {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If player is within attack range
        if (distanceToPlayer <= enemyStats.attackRange) {  // Use attackRange from SO
            if (!isAttacking) {
                Attack();
            }
        } else {
            // Move towards the player
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, enemyStats.moveSpeed * Time.deltaTime);
            isWalking = true;
        }
    }


    public void Die() {
        if (isDieing) return; // Stop Die from being called multiple times

        isDieing = true;
        isAttacking = false;
        // Check if collider type exists and disable them
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null) {
            capsuleCollider.enabled = false;
        }
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null) {
            sphereCollider.enabled = false;
        }

        DropItems();
        Destroy(gameObject, 1f);
    }

    private void DropItems() {
        float heightOffset = 1.0f; // Adjust this value as needed
        Vector3 spawnPosition = transform.position + new Vector3(0, heightOffset, 0);

        foreach (GameObject itemPrefab in itemPrefabs) {
            Instantiate(itemPrefab, spawnPosition, itemPrefab.transform.rotation);
        }
    }


    //Animation fields
    public bool IsWalking() {
        return isWalking;
    }
    public bool IsDieing() {
        return isDieing;
    }
    public bool IsAttacking() {
        return isAttacking;
    }
}