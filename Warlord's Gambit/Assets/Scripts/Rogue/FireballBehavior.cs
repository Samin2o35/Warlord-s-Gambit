using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to deal
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the fireball hit the player
        if (collision.CompareTag("Player"))
        {
            // Stop the fireball's movement
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Stop movement
                rb.isKinematic = true; // Prevent any external physics interaction
            }

            if (animator != null)
            {
                // play explode animation on hit player
                animator.SetTrigger("Hit");
            }

            // Get the PlayerStateMachine component from the player
            PlayerStateMachine player = collision.GetComponent<PlayerStateMachine>();
            if (player != null) 
            {
                // Call the TakeDamage function on the player
                player.TakeDamageFromFireball(damageAmount);
            }
        }
    }

    public void DestroyFireball()
    {
        // Destroy the fireball after it hits the player
        Destroy(gameObject);
    }
}
