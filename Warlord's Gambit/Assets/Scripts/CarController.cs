using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 20f;
    public float maxSpeed = 10f;
    public float steeringSensitivity = 2.5f;
    public float driftFactor = 0.95f;  // Controls how much it drifts normally
    public float handbrakeDriftFactor = 0.8f;  // More drift when handbrake is used
    public float handbrakePower = 0.5f;  // Slows car down while using handbrake

    private Rigidbody2D rb;
    private float moveInput;
    private float steerInput;
    private bool isHandbraking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get movement input
        moveInput = Input.GetAxis("Vertical");  // W/S or Up/Down Arrow keys
        steerInput = Input.GetAxis("Horizontal");  // A/D or Left/Right Arrow keys
        isHandbraking = Input.GetKey(KeyCode.Space);  // Space key for handbrake
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        SteerCar();
        ApplyDrift();
        ApplyHandbrake();
    }

    void ApplyEngineForce()
    {
        // Accelerate forward/backward
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.up * moveInput * acceleration, ForceMode2D.Force);
        }
    }

    void SteerCar()
    {
        // Steering reduces at higher speed for more control
        float speedFactor = rb.velocity.magnitude / maxSpeed;
        float steerAmount = steerInput * steeringSensitivity * speedFactor;

        // Rotate the car
        rb.rotation -= steerAmount;
    }

    void ApplyDrift()
    {
        // Control sideways drift
        float currentDriftFactor = isHandbraking ? handbrakeDriftFactor : driftFactor;
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * currentDriftFactor;
    }

    void ApplyHandbrake()
    {
        // Apply extra drag when handbraking to slow the car slightly
        if (isHandbraking)
        {
            rb.velocity *= handbrakePower;
        }
    }
}