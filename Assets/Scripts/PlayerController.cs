using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform cameraFollowPoint;

    [Header("Movement Settings")]
    public float runAcceleration = 50f;
    public float runSpeed = 4f;
    public float drag = 30f;
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f; // Horizontal look sensitivity
    public float lookSenseV = 0.1f; // Vertical look sensitivity
    public float lookLimitV = 75f; // Limits how far a player can look up/down

    private CharacterController characterController;
    private InputManager inputManager;
    private Camera playerCamera;
    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;
    private bool groundedPlayer;
    private float verticalVelocity;

    [Header("Misc Fields")]
    public Transform spawnPoint;

    private void Awake() {
        inputManager = InputManager.Instance;
        characterController = gameObject.GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    private void Update() {
        MoveVertical(); // MUST be called before MoveHorizontal
        MoveHorizontal();
        Look();
    }

    private void MoveHorizontal() {

        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;

        Vector2 movementInput = inputManager.GetPlayerMovement();
        Vector3 movementDirection = cameraRightXZ * movementInput.x + cameraForwardXZ * movementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;
        newVelocity.y = 0f; // Workaround to prevent jumping from slowing the player down

        // Drag is 0 if the player is in the air - keeps forward momentum
        Vector3 currentDrag = (groundedPlayer)? newVelocity.normalized * drag * Time.deltaTime : Vector3.zero;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        newVelocity.y += verticalVelocity;
        characterController.Move(newVelocity * Time.deltaTime);
        Vector3 horizontalVelocity = new Vector3(newVelocity.x, 0f, newVelocity.z);
        playerAnimator.SetFloat("Velocity", horizontalVelocity.magnitude);
    }

    private void MoveVertical() {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && verticalVelocity < 0) {
            verticalVelocity = 0f;
        }

        verticalVelocity += gravityValue * Time.deltaTime;

        playerAnimator.SetBool("Grounded", groundedPlayer);

        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer) {
            verticalVelocity += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            playerAnimator.SetTrigger("Jump");
        }
    }

    private void LateUpdate() {
        Look();
    }

    private void Look() {
        Vector2 mouseDelta = inputManager.GetMouseDelta();
        cameraRotation.x += lookSenseH * mouseDelta.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * mouseDelta.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * mouseDelta.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);

        // CineMachine prevents Camera x rotation from being altered, so the follow point is changed instead
        cameraFollowPoint.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0f);
    }

    public void Die() {
        Respawn();
    }

    private void Respawn() {
        characterController.velocity.Set(0f, 0f, 0f);
        characterController.enabled = false;
        characterController.transform.position = spawnPoint.position;
        characterController.enabled = true;
    }

    public void SetSpawnPoint(Transform spawnPoint) {
        this.spawnPoint = spawnPoint;
    }
}