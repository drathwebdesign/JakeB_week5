using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimator : MonoBehaviour {
    private const string IS_WALKING = "IsWalking";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DIEING = "IsDieing";

    private bool hasDied = false;  // This flag prevents triggering the death animation multiple times

    private EnemyAI enemyAI;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Update() {

        if (enemyAI.IsDieing() && !hasDied) {
            // Trigger the die animation only once
            animator.SetTrigger(IS_DIEING);
            hasDied = true;
        } else if (enemyAI.IsAttacking()) {
            animator.SetTrigger(IS_ATTACKING);
        }
        animator.SetBool(IS_WALKING, enemyAI.IsWalking());
    }
}