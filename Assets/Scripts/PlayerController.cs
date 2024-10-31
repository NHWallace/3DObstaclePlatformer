using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basis of controller provided by example by Unity at https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
// Example has been modified to use the new input system

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag = 0.1f;

    private InputManager inputManager;

    private void Awake() {
        inputManager = InputManager.Instance;
        characterController = gameObject.GetComponent<CharacterController>();
    }

    private void Update() {
        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;

        Vector2 movementInput = inputManager.GetPlayerMovement();
        Vector3 movementDirection = cameraRightXZ * movementInput.x + cameraForwardXZ * movementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        characterController.Move(newVelocity * Time.deltaTime);
    }
}