using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour {
    public int damageAmount = 25;
    public float pushStrength = 0.5f;

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(this.transform.parent.name + " hit " + collision.gameObject.name + " at " + collision.contacts[0].point);
        Vector3 pointOfContact = collision.contacts[0].point;
        // Check if the collided object has a PlayerHealth component
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        CharacterController characterController = collision.gameObject.GetComponent<CharacterController>();

        if (playerHealth != null) {
            // Inflict damage on the player
            playerHealth.TakeDamage(damageAmount);
        }
        if (characterController != null) {
            Push(characterController, collision.contacts[0].point, this.transform.position);
        }

    }

    private void Push(CharacterController player, Vector3 to, Vector3 from) {
        // Prevent Player from being pushed up or down.
        to.y = 0f;
        from.y = 0f;

        Vector3 pushDirection = (to - from).normalized;
        player.Move(pushDirection * pushStrength);
    }

}