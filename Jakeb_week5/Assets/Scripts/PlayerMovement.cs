using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    private PlayerControls playerControls;
    private Rigidbody rb;
    public float moveSpeed = 10f;
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
        if (isDieing) return;
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
        moveSpeed = 10f;
    }

    void HandleMovement() {
        inputVector = playerControls.Player.Move.ReadValue<Vector2>();

        // Get the camera's forward and right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Keep the vectors flat (ignoring the y-axis)
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the direction relative to the camera
        Vector3 moveDirection = cameraForward * inputVector.y + cameraRight * inputVector.x;

        // Move the character
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        isWalking = moveDirection != Vector3.zero;
    }

    private void HandleRotation() {
        // Get the mouse position in screen space
        Vector2 mousePos = playerControls.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 pointToLook = ray.GetPoint(rayDistance);

            // Calculate the direction to look at
            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0f; // Keep rotation in the XZ plane

            // Prevent rotation if the mouse is too close to the player
            float distanceToMouse = lookDirection.magnitude;
            if (distanceToMouse > 2f) { // Adjust this threshold as needed
                                          // Apply rotation to face the mouse
                Quaternion newRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f);
            }
        }
    }

    void Jump() {
        if (isBlocking) return; // Prevent jumping while blocking
        if (isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            Debug.Log("IsJumping called: " + isJumping);
            GetComponent<Animator>().SetTrigger("IsJumping");
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
        if (!isAttacking) {
            isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine() {
        swordCollider.enabled = true; // Enable the sword collider
        yield return new WaitForSeconds(0.25f); // Adjust duration as needed for the attack animation
        isAttacking = false;
        swordCollider.enabled = false; // Disable the sword collider after the attack
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
        GameManager.instance.PlayerDied();
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