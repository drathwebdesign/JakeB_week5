using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    private PlayerControls playerControls;
    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce;
    private Vector2 inputVector;
    private Vector2 lookInput;

    //groundCheck
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    //Gravity 
    [SerializeField] private float baseGravity = 5f;
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField] private float fallSpeedMultiplier = 5f;

    //Double Jump
    private int doubleJumpsUsed = 0;
    private int maxDoubleJumps = 1;

    //health & invulnerability
    public int maxHealth = 100;
    private int currentHealth;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 0.5f;
    public Slider healthBarSlider;

    //attacking
    public Collider swordCollider;

    //Powerups Affecting Damage
    private WeaponHandler weaponHandler;
    private bool isDamageDoubled = false;
    public ParticleSystem doubleDamageEffect;

    //Blocking
    public ParticleSystem blockEffect;

    private bool isGrounded;
    //animation fields
    private bool isWalking;
    private bool isJumping;
    private bool isDoubleJumping;
    private bool isBlocking;
    private bool isAttacking;
    private bool isDieing;

    private void Awake() {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        playerControls.Player.Jump.performed += ctx => Jump();
        playerControls.Player.Block.performed += ctx => Block();
        playerControls.Player.Block.canceled += ctx => StopBlocking();
        playerControls.Player.Attack.performed += ctx => Attack();

        swordCollider.enabled = false;
        weaponHandler = GetComponentInChildren<WeaponHandler>();
    }

    void Start() {
        UpdateHealthUI();
        blockEffect.Stop();
        doubleDamageEffect.Stop();
    }

    void Update() {
        GroundCheck();
        Gravity();
        HandleMovement();
        HandleRotation();
    }

    // Blocking
    void Block() {
        if (isBlocking) return;

        isBlocking = true;

        // Activate the block particle effect
        if (blockEffect != null) {
            blockEffect.Play();
        }

        // Stop movement and disable jumping
        rb.velocity = Vector3.zero; // Stop all movement
        moveSpeed = 0f;
    }

    void StopBlocking() {
        isBlocking = false;

        // Deactivate the block particle effect
        if (blockEffect != null) {
            blockEffect.Stop();
        }

        // Restore movement speed 
        moveSpeed = 5f;
    }

    void HandleMovement() {
        inputVector = playerControls.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        isWalking = moveDir != Vector3.zero;
    }

    private void HandleRotation() {
        // Get the mouse position in screen space
        lookInput = playerControls.Player.Look.ReadValue<Vector2>();

        // Convert screen position to world position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(lookInput.x, lookInput.y, Camera.main.transform.position.y));

        // Calculate the direction to look at
        Vector3 lookDirection = mouseWorldPosition - transform.position;
        lookDirection.y = 0f; // Keep rotation in the XZ plane

        // Apply rotation to face the mouse
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void Jump() {
        if (isBlocking) return; // Prevent jumping while blocking
        if (isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            isGrounded = false;
        } else if (!isGrounded && doubleJumpsUsed < maxDoubleJumps) {
            DoubleJump();
        }
    }

    void GroundCheck() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (isGrounded) {
            isJumping = false;
            isDoubleJumping = false;
            doubleJumpsUsed = 0; // Reset double jump count when grounded
        }
    }

    void Gravity() {
        if (rb.velocity.y < 0) { // Falling
            rb.velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !isJumping) {
            rb.velocity += Vector3.up * Physics.gravity.y * (baseGravity - 1) * Time.deltaTime;
        }

        // Cap the fall speed
        if (rb.velocity.y < -maxFallSpeed) {
            rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, rb.velocity.z);
        }
    }

    void DoubleJump() {
        if (!isGrounded && doubleJumpsUsed < maxDoubleJumps) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset vertical velocity before double jumping
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isDoubleJumping = true;
            // Increase amount of jumps used
            doubleJumpsUsed++;
        }
    }

    //Damage Powerups
    public void DoubleDamage() {
            isDamageDoubled = true;
            weaponHandler.weaponStats.damage *= 2; // Double the weapon damage
            doubleDamageEffect.Play();
            StartCoroutine(DoubleDamageDuration());
        }

    private IEnumerator DoubleDamageDuration() {
        yield return new WaitForSeconds(10f); // Duration
        weaponHandler.weaponStats.damage /= 2; // Reset the weapon damage
        isDamageDoubled = false;
        doubleDamageEffect.Stop();
    }

    //Attacking
    void Attack() {
        isAttacking = true;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        swordCollider.enabled = true; // Enable the sword collider
        yield return new WaitForSeconds(1f); // Adjust duration as needed for the attack animation
        swordCollider.enabled = false; // Disable the sword collider after the attack
        isAttacking = false; // Reset attacking state
    }

    public void TakeDamage(int damage) {
        if (!isInvulnerable) {
            if (isBlocking) {
                // No damage taken while blocking
                return;
            }

            currentHealth -= damage;

            if (currentHealth <= 0) {
                Die();
            } else {
                StartCoroutine(InvulnerabilityCoroutine());
                UpdateHealthUI();
            }
        }
    }

    public void Heal(int amount) {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI() {
        if (healthBarSlider != null) {
            // Update the fill amount of the health bar
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
    }

    private void Die() {
        Debug.Log("Player has died!");
        if (isDieing) return; // Stop Die from being called multiple times

        isDieing = true;
        playerControls.Player.Disable();
        //GameManager.instance.PlayerDied();
    }

    private IEnumerator InvulnerabilityCoroutine() {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    //Animation Fields
    public bool IsGrounded() {
        return isGrounded;
    }
    public bool IsWalking() {
        return isWalking;
    }
    public bool IsJumping() {
        return isJumping;
    }
    public bool IsDoubleJumping() {
        return isDoubleJumping;
    }
    public bool IsBlocking() {
        return isBlocking;
    }
    public bool IsAttacking() {
        return isAttacking;
    }
    public bool IsDieing() {
        return isDieing;
    }
}