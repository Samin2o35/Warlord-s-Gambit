using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float accelerationFactor = 30.0f;
    public float driftFactor = 0.95f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20f;

    // local variables
    float accelerationInput = 0;
    float steeringInput = 0;
    float velocityVsUp = 0;
    float rotationAngle = 0;

    // componenets
    Rigidbody2D carRb;

    void Awake()
    {
        carRb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthagonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        // Calculate how much forward card is going in terms of velocity
        velocityVsUp = Vector2.Dot(transform.up, carRb.velocity);

        // Limit so player cannot go faster than max speed in forward direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }

        // Limit so player cannot go faster than half of max speed in reverse direction
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
        {
            return;
        }

        // Limit so player cannot go faster while accelerating in any direction
        if (carRb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
        {
            return;
        }

        // Apply drag if there is no accelerationInput given by player
        if (accelerationInput == 0)
        {
            carRb.drag = Mathf.Lerp(carRb.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else 
        {
            carRb.drag = 0;
        }
        
        // create force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        // apply force and push car forward
        carRb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        // Limit car turning when moving slow
        float minSpeedBeforeAllowTurningFactor = (carRb.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        // update rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        // apply steering by rotating the car object
        carRb.MoveRotation(rotationAngle);
    }

    void KillOrthagonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRb.velocity, transform.right);

        carRb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity()
    {
        // return how fast car is moving sideways
        return Vector2.Dot(transform.right, carRb.velocity);
    }
    
    public bool isTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        // Tires screech if player is hitting brakes when moving forward
        if(accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        // if alot of side movement then tires shuld be screeching
        if(Mathf.Abs(GetLateralVelocity()) > 4.0f)
        {
            return true;
        }
        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}