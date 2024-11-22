using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform cameraFollowPoint;

    [Header("References")]
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;


    [Header("Movement Settings")]
    public float runAcceleration = 50f;
    public float runSpeed = 4f;
    public float drag = 30f;
    public float jumpHeight = 1.0f;
    public float gravityModifier = 2f;

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f; // Horizontal look sensitivity
    public float lookSenseV = 0.1f; // Vertical look sensitivity
    public float lookLimitV = 75f; // Limits how far a player can look up/down

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

    private void Awake() {
        inputManager = InputManager.Instance;
        healthHandler = GetComponent<PlayerHealth>();
        playerCamera = Camera.main;
    }

    private void Update() {
        //MoveVertical(); // MUST be called before MoveHorizontal
        //MoveHorizontal();
        //Look();
    }

    private void FixedUpdate() {
        // Rigidbody movement should be handled in FixedUpdate, not Update
        UpdateGroundedState();
        MoveVertical();
        MoveHorizontal();
        Look();
        AnimateMovement();
    }

    private void MoveHorizontal() {
        if (!groundedPlayer) {
            timeSpentInAir += Time.deltaTime;
        }
        else {
            timeSpentInAir = 0;
        }

        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;

        Vector2 movementInput = inputManager.GetPlayerMovement();
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

        Vector3 movementForce = movementDirection * runSpeed;      
        rb.AddForce(movementForce);
    }
    
    private void MoveVertical() {
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer) {
            Vector3 jumpForce = new Vector3 (0, jumpHeight, 0);
            rb.AddForce(jumpForce);

            playerAnimator.SetTrigger("Jump");
            AudioManager.Instance.PlayEffect("Jump");
        }

        ApplyGravity(); // Needed for the player to have different gravity than other rigidbodies in scene
    }

    private void ApplyGravity() {
        if (!Mathf.Approximately(gravityModifier, 1f)) {
            // Player has a special gravity value
            rb.useGravity = false;
            Vector3 gravityForce = new Vector3(0, 1f, 0) * -9.81f * gravityModifier * rb.mass;
            rb.AddForce(gravityForce);

        }
        else {
            // Player has standard gravity
            rb.useGravity = true;
        }
    }


    private void UpdateGroundedState() {
        groundedPlayer = Physics.Raycast(transform.position, -Vector3.up, 0.1f);
        playerAnimator.SetBool("Grounded", groundedPlayer);
    }

    private void Look() {
        Vector2 mouseDelta = inputManager.GetMouseDelta();
        cameraRotation.x += lookSenseH * mouseDelta.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * mouseDelta.y, -lookLimitV, lookLimitV);

        // CineMachine prevents Camera x rotation from being altered, so the follow point is changed instead
        cameraFollowPoint.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0f);
    }

    private void AnimateMovement() {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        playerAnimator.SetFloat("Velocity", horizontalVelocity.magnitude);
    }


    public void SetSpawnPoint(Transform spawnPoint) {
        this.spawnPoint = spawnPoint;
    }
    
    public void Die() {
        //Respawn();
        healthHandler.currentHealth = healthHandler.maxHealth;
        healthHandler.healthBar.SetHealth(healthHandler.currentHealth);
    }
    /*
    private void Respawn() {
        characterController.velocity.Set(0f, 0f, 0f);
        characterController.enabled = false;
        characterController.transform.position = spawnPoint.position;
        characterController.enabled = true;
    }*/

}
