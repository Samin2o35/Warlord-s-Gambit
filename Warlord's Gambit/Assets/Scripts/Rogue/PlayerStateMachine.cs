using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;
using System.Collections;

public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Attack
    }

    public PlayerState currentState;

    public float moveSpeed = 5f;        // Player move speed
    public float attackInterval = 2f;   // Player attack speed
    public float attackDamage = 10f;    // Player attack damage

    private Rigidbody2D rb;
    private Vector2 movement;

    private bool isAttacking = false;
    private bool isFacingRight = true;

    public Animator animator;
    public Transform attackRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Idle; // Initial state

        StartCoroutine(AutoAttack());
    }

    void Update()
    {
        HandleInput(); // Handles movement and attack input

        if (!isAttacking) // Only handle state if not attacking
        {
            UpdateState(); // Updates FSM states
        }
        AnimateState(); // Handles animation transitions

        FlipSprite();   // Flip sprite based on movement direction
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed; // Apply movement
    }

    void HandleInput()
    {
        // Movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void UpdateState()
    {
        if (movement != Vector2.zero)
        {
            currentState = PlayerState.Move;
        }
        else 
        {
            currentState = PlayerState.Idle;
        }
    }

    void AnimateState()
    {
        // Set animator parameters based on the state
        animator.SetBool("isMoving", movement != Vector2.zero && !isAttacking);
        animator.SetBool("isAttacking", isAttacking);
    }

    void FlipSprite()
    {
        if (movement.x < 0 && isFacingRight)
        {
            // Flip to face left
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
        else if (movement.x > 0 && !isFacingRight)
        {
            // Flip to face right
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false; // Allow state changes again

        // Transition back to Idle or Move state based on movement
        if (movement != Vector2.zero)
        {
            currentState = PlayerState.Move;
        }
        else
        {
            currentState = PlayerState.Idle;
        }

        AnimateState();
    }

    IEnumerator AutoAttack()
    {
        while (true) 
        {
            // Trigger attack
            isAttacking = true;

            // Damage Enemies, Play effects logic
            DamageEnemiesInRange();

            currentState = PlayerState.Attack;
            AnimateState();

            // Wait for next attack interval
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void DamageEnemiesInRange()
    {
        // Find all colliders within the attack range
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackRange.position, 
            attackRange.GetComponent<CircleCollider2D>().radius * 2);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy")) // Ensure only enemies are affected
            {
                EnemyStateMachine enemy = collider.GetComponent<EnemyStateMachine>();
                if (enemy != null)
                {
                    enemy.TakeDamageFromPlayer(attackDamage);
                }
            }
        }
    }

    public void TakeDamageFromFireball(float damage)
    {
        // Implement health reduction or destruction logic
        Debug.Log("Player took " + damage + " damage!");
    }
}
