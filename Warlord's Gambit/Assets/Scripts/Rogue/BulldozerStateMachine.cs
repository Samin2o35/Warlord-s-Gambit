using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozerStateMachine : MonoBehaviour
{
    #region Variables
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    public EnemyState currentState;

    public float walkSpeed = 2f;
    public float attackRange = 2f;
    public float idleTime = 2f;
    public float health = 10f;

    public int aoeDamage = 5; // Amount of damage to deal

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 walkDirection;
    private SpriteRenderer spriteRenderer;

    private bool isAttacking = false;
    private bool isDead = false;

    public Animator animator;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Idle;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(StateMachine());
    }

    void Update()
    {
        if (isDead) return; // Prevent updates when dead

        AnimateState();

        // Adjust the sorting order dynamically
        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        // Use the negative Y-coordinate to determine sorting order.lower Y means displayed top of others
        if (spriteRenderer != null)
        {
            // Primary criterion: Y-coordinate
            int baseOrder = Mathf.RoundToInt(-transform.position.y * 100);

            // Secondary criterion: InstanceID ensures unique sorting for overlapping Y-values
            int tieBreaker = GetInstanceID() % 1000;

            // Combine primary and secondary criteria
            spriteRenderer.sortingOrder = baseOrder * 1000 + tieBreaker;
        }
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    yield return StartCoroutine(IdleState());
                    break;
                case EnemyState.Walk:
                    yield return StartCoroutine(WalkState());
                    break;
                case EnemyState.Attack:
                    yield return StartCoroutine(AttackState());
                    break;
            }
        }
    }

    IEnumerator IdleState()
    {
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);

        // Transition to Walk state
        currentState = EnemyState.Walk;
    }

    IEnumerator WalkState()
    {
        animator.SetBool("isWalking", true);

        float walkDuration = Random.Range(1f, 3f);

        for (float t = 0; t < walkDuration; t += Time.deltaTime)
        {
            // Null check for player
            if (player == null)
            {
                yield break; // Exit if player is no longer available
            }

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                currentState = EnemyState.Attack;
                yield break;
            }

            // Move towards player
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * walkSpeed;

            // Flip the enemy to face the player
            FlipTowardsPlayer(directionToPlayer.x);

            yield return null;
        }

        rb.velocity = Vector2.zero;

        // Transition to Idle state
        currentState = EnemyState.Idle;
    }

    IEnumerator AttackState()
    {
        rb.velocity = Vector2.zero; // Stop moving

        isAttacking = true;
        animator.SetTrigger("Attack");

        // Execute AoE attack
        // AoEAttack();

        // Simulate attack duration
        yield return new WaitForSeconds(1f);

        isAttacking = false;

        // Return to Idle or Walk state
        currentState = EnemyState.Idle;
    }

    void AnimateState()
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    void FlipTowardsPlayer(float directionX)
    {
        Vector3 currentScale = transform.localScale;

        if (directionX > 0) // Player is to the right
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        else if (directionX < 0) // Player is to the left
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }

        transform.localScale = currentScale;
    }

    void AoEAttack()
    {
        // Find all colliders within the attack range
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D collider in hitColliders)
        {
            // Check if the collider belongs to the player or other attackable objects
            if (collider.CompareTag("Player"))
            {
                PlayerStateMachine player = collider.GetComponent<PlayerStateMachine>();
                if (player != null)
                {
                    player.TakeDamageFromAoE(aoeDamage);
                }
            }
        }
    }

    public void TakeDamageFromPlayer(float attackDamage)
    {
        // Implement health reduction or destruction logic
        if (isDead) return; // Prevent updates when dead

        health -= attackDamage;
        if (health <= 0)
        {
            StartCoroutine(HandleEnemyDeath());
        }
        Debug.Log("Bulldozer took " + attackDamage + " damage!");
    }

    private IEnumerator HandleEnemyDeath()
    {
        currentState = EnemyState.Death;
        isDead = true;

        // Trigger death animation
        animator.SetTrigger("Death");

        // Disable further input
        rb.velocity = Vector2.zero;

        // Wait for animation to complete (if required)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Perform any cleanup or respawn logic here
        Destroy(gameObject); // Destroy the player object
    }

    // Draw the debug sphere in the scene view to visualize the enemy's range
    private void OnDrawGizmos()
    {
        // Set the color for the range gizmo (red in this case)
        Gizmos.color = Color.red;

        // Draw a sphere at the enemy's position with a radius equal to attackRange
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}