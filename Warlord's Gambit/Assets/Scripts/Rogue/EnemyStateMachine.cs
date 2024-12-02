using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack
    }

    public EnemyState currentState;

    public float walkSpeed = 2f;
    public float attackRange = 2f;
    public float idleTime = 2f;

    public GameObject fireballPrefab; 
    public Transform spawnPoint;
    public float fireballSpeed = 5f;
    public float fireballLifetime = 2f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 walkDirection;
    private bool isAttacking = false;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Idle;

        StartCoroutine(StateMachine());
    }

    void Update()
    {
        AnimateState();
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

    // This function is called by the Animation Event
    public void SpawnFireball()
    {
        if (fireballPrefab != null && spawnPoint != null)
        {
            // Instantiate the attack effect
            GameObject fireballShot = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);

            // Determine the direction based on enemy's facing direction
            Vector3 direction = transform.localScale.x > 0 ? Vector3.left : Vector3.right;

            // Set the movement for the effect
            Rigidbody2D rb = fireballShot.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * fireballSpeed;
            }

            // Destroy the effect after a set time
            Destroy(fireballShot, fireballLifetime);
        }
    }
}