using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {
    [SerializeField] public float springForce = 40;
    private bool shouldApplyImpulse;
    private PlayerController player;
    private float timeSinceLastImpulse = 0;
    private void OnCollisionEnter(Collision collision) {
        player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null) {
            // Impulse player into the air
            Debug.Log("Collided with player!");
            shouldApplyImpulse = true;
        }
    }

    private void FixedUpdate() {
        // Forces should only be applied to rigid bodies in fixed update
        timeSinceLastImpulse += Time.fixedDeltaTime;

        // Do not apply force from a spring more than once every half second
        // This should prevent players from gaining more than the intended height
        if (timeSinceLastImpulse < 0.5) {
            return;
        }

        if (shouldApplyImpulse) {
            Debug.Log("Impulse should be applied this fixed update frame!");
            if (player != null) {
                Debug.Log("Forced applied!");
                player.rb.AddForce(Vector3.up * springForce, ForceMode.VelocityChange);
                shouldApplyImpulse = false;
                timeSinceLastImpulse = 0;
            }
        }
    }
}