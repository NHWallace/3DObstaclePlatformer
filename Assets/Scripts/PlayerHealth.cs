using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;           // Maximum health of the player
    private int currentHealth;             // Current health of the player

    public HealthBar healthBar;            // Reference to the HealthBar script
    public Transform spawnPoint;            // Reference to the spawn point

    private CharacterController characterController; // Reference to CharacterController

    private void Start()
    {
        // Initialize health and health bar
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent health from going negative

        // Update the health bar's current value
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");

        // Reset the player's health and health bar
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        // Start the respawn coroutine
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        // Wait for a short duration (e.g., 2 seconds)
        yield return new WaitForSeconds(2f);

        // Disable the CharacterController before respawning
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Move the player to the spawn point's position
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position; 
        }
        else
        {
            Debug.LogWarning("Spawn point not assigned!");
        }

        // Re-enable the CharacterController after respawning
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Debug.Log($"Player respawned at: {transform.position}");
    }
}