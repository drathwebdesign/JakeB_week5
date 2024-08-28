using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {
    public WeaponStats weaponStats;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null) {
                // Calculate the knockback direction (away from the player)
                Vector3 knockbackDirection = other.transform.position - transform.position;

                // Apply damage and knockback
                enemyAI.TakeDamage(weaponStats.damage, knockbackDirection, weaponStats.knockBack);
            }
        }
    }
}