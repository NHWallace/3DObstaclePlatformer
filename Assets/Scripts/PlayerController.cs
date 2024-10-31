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

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        newVelocity.y += verticalVelocity;
        characterController.Move(newVelocity * Time.deltaTime);
        playerAnimator.SetFloat("Velocity", newVelocity.magnitude);
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
}