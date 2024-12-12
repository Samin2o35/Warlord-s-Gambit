using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveStateMachine : MonoBehaviour
{
    #region Variables
    public enum MicrowaveState
    {
        Idle,
        Walk,
        Attack,
        Hurt,
        Death
    }

    public MicrowaveState currentState;

    public float walkSpeed = 2f;
    public float attackRange = 2f;
    public float idleTime = 2f;

    public GameObject fireballPrefab; 
    public Transform spawnPoint;
    public float fireballSpeed = 5f;
    public float fireballLifetime = 2f;
    public float health = 100f;             // Total health of the Microwave
    public float hurtDuration = 0.5f;       // Time spent in hurt state
    public float deathDelay = 1f;           // Time before destruction after death

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 walkDirection;
    private SpriteRenderer spriteRenderer;

    private bool isDead = false;            // To prevent further state changes after death
    private bool isAttacking = false;

    public Animator animator;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = MicrowaveState.Idle;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(StateMachine());
    }

    void Update()
    {
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
                case MicrowaveState.Idle:
                    yield return StartCoroutine(IdleState());
                    break;
                case MicrowaveState.Walk:
                    yield return StartCoroutine(WalkState());
                    break;
                case MicrowaveState.Attack:
                    yield return StartCoroutine(AttackState());
                    break;
                case MicrowaveState.Hurt:
                    yield return StartCoroutine(HurtState());
                    break;
                case MicrowaveState.Death:
                    yield return StartCoroutine(DeathState());
                    break;
            }
        }
    }

    IEnumerator IdleState()
    {
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);

        // Transition to Walk state
        currentState = MicrowaveState.Walk;
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
                currentState = MicrowaveState.Attack;
                yield break;
            }

            // Move towards player
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.velocity = directionToPlayer * walkSpeed;

            // Flip the Microwave to face the player
            FlipTowardsPlayer(directionToPlayer.x);

            yield return null;
        }

        rb.velocity = Vector2.zero;

        // Transition to Idle state
        currentState = MicrowaveState.Idle;
    }

    IEnumerator AttackState()
    {
        rb.velocity = Vector2.zero; // Stop moving

        isAttacking = true;
        animator.SetTrigger("Attack");

        // Simulate attack duration
        yield return new WaitForSeconds(1f);

        isAttacking = false;

        // Return to Idle or Walk state
        currentState = MicrowaveState.Idle;
    }

    void AnimateState()
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    IEnumerator HurtState()
    {
        isAttacking = false;

        // Stop movement during hurt
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Hurt");

        // Wait for the hurt animation to complete
        yield return new WaitForSeconds(hurtDuration);

        // Transition to Idle or Walk if still alive
        if (!isDead)
        {
            currentState = MicrowaveState.Idle;
        }
    }

    IEnumerator DeathState()
    {
        // Prevent further damage or state changes
        isDead = true;

        // Stop movement
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");

        // Wait for the death animation to complete
        yield return new WaitForSeconds(deathDelay);

        // Destroy the Microwave object
        Destroy(gameObject);
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

    // This function is called by the Animation Event
    public void SpawnFireball()
    {
        if (fireballPrefab != null && spawnPoint != null)
        {
            // Instantiate the attack effect
            GameObject fireballShot = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);

            // Determine the direction based on Microwave's facing direction
            Vector3 direction = transform.localScale.x > 0 ? Vector3.left : Vector3.right;

            // Set the movement for the effect
            Rigidbody2D rb = fireballShot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * fireballSpeed;
            }

            // Flip the fireball to match the Microwave's facing direction
            fireballShot.transform.localScale = new Vector3(-direction.x, 
                fireballShot.transform.localScale.y, fireballShot.transform.localScale.z);

            // Destroy the effect after a set time
            Destroy(fireballShot, fireballLifetime);
        }
    }

    public void TakeDamageFromPlayer(float attackDamage)
    {
        if (isDead) return; // Prevent taking damage after death

        health -= attackDamage;
        Debug.Log("Microwave took " + attackDamage + " damage!");

        if (health <= 0)
        {
            currentState = MicrowaveState.Death;
        }
        else
        {
            currentState = MicrowaveState.Hurt;
        }
    }

    // Draw the debug sphere in the scene view to visualize the Microwave's range
    private void OnDrawGizmos()
    {
        // Set the color for the range gizmo (red in this case)
        Gizmos.color = Color.red;

        // Draw a sphere at the Microwave's position with a radius equal to attackRange
        if (spawnPoint != null)
        {
            Gizmos.DrawWireSphere(spawnPoint.position, attackRange);
        }
    }
}