using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to deal

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball hit the player
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerStateMachine component from the player
            PlayerStateMachine player = collision.GetComponent<PlayerStateMachine>();

            if (player != null)
            {
                // Call the TakeDamage function on the player
                player.TakeDamageFromFireball(damageAmount);
            }

            // Destroy the fireball after it hits the player
            Destroy(gameObject);
        }
    }
}
