using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;

    // local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    // componenets
    RigidBody2D carRb;

    void Awake()
    {
        carRb = GetComponent<RigidBody2D>();
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

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        // create force for the engine
        vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        // apply force and push car forward
        carRb.AddForce(engineForceVector, ForceMode2D.Force);
    }
}