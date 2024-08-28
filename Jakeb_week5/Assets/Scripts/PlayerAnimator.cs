using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private const string IS_GROUNDED = "IsGrounded";
    private const string IS_WALKING = "IsWalking";
    private const string IS_JUMPING = "IsJumping";
    private const string IS_DOUBLE_JUMPING = "IsDoubleJumping";
    private const string IS_BLOCKING = "IsBlocking";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DIEING = "IsDieing";

    private bool hasDied = false;  // This flag prevents triggering the death animation multiple times

    private PlayerMovement playerMovement;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() {

        if (playerMovement.IsDieing() && !hasDied) {
            // Trigger the die animation only once
            animator.SetTrigger(IS_DIEING);
            hasDied = true;
        } else if (playerMovement.IsJumping()) {
            animator.SetTrigger(IS_JUMPING);
        } else if (playerMovement.IsDoubleJumping()) {
            animator.SetTrigger(IS_DOUBLE_JUMPING);
        } else if (playerMovement.IsAttacking()) {
            animator.SetTrigger(IS_ATTACKING);
        }

        animator.SetBool(IS_WALKING, playerMovement.IsWalking());
        animator.SetBool(IS_GROUNDED, playerMovement.IsGrounded());
        animator.SetBool(IS_BLOCKING, playerMovement.IsBlocking());
    }
}