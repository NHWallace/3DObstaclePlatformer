using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;           // Maximum health of the player
    public int currentHealth;             // Current health of the player

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
            characterController.gameObject.GetComponent<PlayerController>().Die();
        }
    }
}