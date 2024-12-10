using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {
    [SerializeField] public float springForce = 40;
    private bool shouldApplyImpulse;
    private PlayerController player;
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
        if (shouldApplyImpulse) {
            Debug.Log("Impulse should be applied this fixed update frame!");
            if (player != null) {
                Debug.Log("Forced applied!");
                player.rb.AddForce(Vector3.up * springForce, ForceMode.Impulse);
                shouldApplyImpulse = false;
            }
        }
    }
}