using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has a PlayerHealth component
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            // Inflict damage on the player
            playerHealth.TakeDamage(damageAmount);
            AudioManager.Instance.PlayEffect("Damage");
        }
    }
}