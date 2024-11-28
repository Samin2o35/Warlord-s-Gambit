using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Attack
    }

    public PlayerState currentState;

    public float moveSpeed = 5f;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Idle; // Initial state
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

        // Attack input
        if (Input.GetKeyDown(KeyCode.Space)) // Example attack key
        {
            currentState = PlayerState.Attack;
        }
        else if (movement != Vector2.zero)
        {
            currentState = PlayerState.Move;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    void UpdateState()
    {
        // Example attack logic: return to Idle after attacking
        if (currentState == PlayerState.Attack)
        {
            // Simulate attack cooldown duration
            Invoke("EndAttack", 0.5f); // Attack lasts for 0.5 seconds
        }
    }

    void EndAttack()
    {
        if (currentState == PlayerState.Attack)
        {
            currentState = PlayerState.Idle;
        }
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
}
