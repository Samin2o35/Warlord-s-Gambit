using UnityEngine;
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

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isAttacking = false;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Idle; // Initial state

        StartCoroutine(AutoAttack());
    }

    void Update()
    {
        HandleInput(); // Handles movement and attack input
        UpdateState(); // Updates FSM states
        AnimateState(); // Handles animation transitions
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.Move)
        {
            rb.velocity = movement * moveSpeed; // Apply movement
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop moving in non-move states
        }
    }

    void HandleInput()
    {
        // Movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Change state based on move input recieved
        if (movement != Vector2.zero)
        {
            currentState = PlayerState.Move;
        }
        else if (!isAttacking) 
        {
            currentState = PlayerState.Idle;
        }
    }

    void UpdateState()
    {
        
    }

    void AnimateState()
    {
        // Set animator parameters based on the state
        switch (currentState)
        {
            case PlayerState.Idle:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                break;

            case PlayerState.Move:
                animator.SetBool("isMoving", true);
                animator.SetBool("isAttacking", false);
                break;

            case PlayerState.Attack:
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", true);
                break;
        }
    }

    IEnumerator AutoAttack()
    {
        while (true) 
        {
            // Trigger attack
            isAttacking = true;
            currentState = PlayerState.Attack;

            // Damage Enemies, Play effects logic

            // Wait for attack animation to finish
            yield return new WaitForSeconds(0.5f);

            // Return to Idle or Move state
            isAttacking = false;
            currentState = movement !=Vector2.zero? PlayerState.Move : PlayerState.Idle;

            // Wait for next attack interval
            yield return new WaitForSeconds(attackInterval - 0.5f);
        }
    }
}
