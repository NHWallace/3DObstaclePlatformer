using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform cameraFollowPoint;

    [Header("References")]
    public Transform player;
    public Rigidbody rb;


    [Header("Movement Settings")]
    public float runSpeed = 4;
    public float runAcceleration = 180f;
    public float groundDrag = 10f;
    public float jumpHeight = 580;
    public float gravityModifier = 1.5f;

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f; // Horizontal look sensitivity
    public float lookSenseV = 0.1f; // Vertical look sensitivity
    public float lookLimitV = 75f; // Limits how far a player can look up/down
    public bool invertVerticalAxis;

    private InputManager inputManager;
    private Camera playerCamera;
    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;
    private bool groundedPlayer;
    private float verticalVelocity;
    private float timeSpentInAir;
    PlayerHealth healthHandler; // Grabbed from scene in Awake

    [Header("Misc Fields")]
    public Transform spawnPoint;

    private Vector2 movementInput;
    private Vector2 mouseDelta;
    private bool jumpedThisFrame;
    private float jumpCooldown = 0.25f;
    private float timeSinceLastJump = 0f;

    private void Awake() {
        inputManager = InputManager.Instance;
        healthHandler = GetComponent<PlayerHealth>();
        playerCamera = Camera.main;
    }

    private void Update() {
        GetInput();
    }

    private void GetInput() {
        movementInput = inputManager.GetPlayerMovement();

        // This validation prevents the jump flag from being consued erronously
        // when an update frame happens between when a player tries to jump
        // and the fixed update frame where the jump is supposed to happen
        if (jumpedThisFrame == false && groundedPlayer) {
            jumpedThisFrame = inputManager.PlayerJumpedThisFrame();
        }

        mouseDelta = inputManager.GetMouseDelta();
        UpdateGroundedState();
    }

    private void FixedUpdate() {
        // Rigidbody movement should be handled in FixedUpdate, not Update
        MoveHorizontal();
        MoveVertical();
        Look();
        AnimateMovement();
        timeSinceLastJump += Time.fixedDeltaTime;
    }

    private void MoveHorizontal() {
        if (groundedPlayer) {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0;
        }

        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;

        //Vector2 movementInput = inputManager.GetPlayerMovement(); // moved to GetInput();
        Vector3 movementDirection = cameraRightXZ * movementInput.x + cameraForwardXZ * movementInput.y;

        // Change the direction the player model is facing if the player input movement this frame
        if (movementDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementDirection), Time.deltaTime * 9f);

            // Play the running sound if the player is grounded
            if (groundedPlayer) {
                AudioManager.Instance.PlayRunningSound("Running");
            }
            else {
                AudioManager.Instance.PauseRunningSound();
            }
        }
        // If the player did not input movement this frame, stop the running sound
        else {
            AudioManager.Instance.PauseRunningSound();
        }

        Vector3 movementForce = movementDirection * runAcceleration;      
        rb.AddForce(movementForce);
        LimitHorizontalSpeed();

        // manually apply drag
        // note: rigidbodies move only on FixedUpdate so use fixedDeltaTime instead of deltaTime
        Vector3 movementDelta = movementDirection * runAcceleration * Time.fixedDeltaTime;
        float savedVerticalVelocity = rb.velocity.y;
        Vector3 newVelocity = rb.velocity + movementDelta;
        newVelocity.y = 0f;
        Vector3 currentDrag = newVelocity.normalized * rb.drag * Time.fixedDeltaTime;
        newVelocity = (newVelocity.magnitude > rb.drag * Time.fixedDeltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);
        newVelocity.y = savedVerticalVelocity;
        rb.velocity = newVelocity;

    }
    
    private void MoveVertical() {
        //jumpedThisFrame moved to GetInput();
        if (jumpedThisFrame && groundedPlayer && (timeSinceLastJump > jumpCooldown)) {
            Vector3 jumpForce = new Vector3 (0, jumpHeight, 0);
            rb.AddForce(jumpForce, ForceMode.VelocityChange);

            playerAnimator.SetTrigger("Jump");
            AudioManager.Instance.PlayEffect("Jump");

            // Reset booleans used in jumping to ensure jump effects do not occur more than once per jump
            jumpedThisFrame = false;
            groundedPlayer = false;
            timeSinceLastJump = 0;
        }

        ApplyGravity(); // Needed for the player to have different gravity than other rigidbodies in scene
    }

    private void ApplyGravity() {
        if (!Mathf.Approximately(gravityModifier, 1f)) {
            // Player has a special gravity value
            rb.useGravity = false;
            float deltaTimeModifier = 60; // scale up to account for how low gravity force will be after accounting for delta time
            Vector3 gravityForce = new Vector3(0, 1f, 0) * -9.81f * gravityModifier * rb.mass * Time.fixedDeltaTime * deltaTimeModifier;
            rb.AddForce(gravityForce, ForceMode.Acceleration);

        }
        else {
            // Player has standard gravity
            rb.useGravity = true;
        }
    }


    private void UpdateGroundedState() {
        groundedPlayer = Physics.Raycast(transform.position, -Vector3.up, 0.2f);
        playerAnimator.SetBool("Grounded", groundedPlayer);
    }

    private void Look() {
        // Vector2 mouseDelta = inputManager.GetMouseDelta(); // moved to GetInput()
        cameraRotation.x += lookSenseH * mouseDelta.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * mouseDelta.y, -lookLimitV, lookLimitV);

        // Update vertical inversion value
        float verticalInverterValue = invertVerticalAxis ? -1f : 1f; 

        // CineMachine prevents Camera x rotation from being altered, so the follow point is changed instead
        cameraFollowPoint.transform.rotation = Quaternion.Euler(cameraRotation.y * verticalInverterValue, cameraRotation.x, 0f);
    }

    private void AnimateMovement() {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        playerAnimator.SetFloat("Velocity", horizontalVelocity.magnitude);
    }


    public void SetSpawnPoint(Transform spawnPoint) {
        this.spawnPoint = spawnPoint;
    }
    
    public void Die() {
        Respawn();
        healthHandler.currentHealth = healthHandler.maxHealth;
        healthHandler.healthBar.SetHealth(healthHandler.currentHealth);
    }

    public void LimitHorizontalSpeed() {
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (currentVelocity.magnitude > runSpeed) {
            Vector3 newVelocity = currentVelocity.normalized * runSpeed;
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
    }
    
    private void Respawn() {
        rb.velocity = Vector3.zero;
        this.transform.position = spawnPoint.position;
    }

}
