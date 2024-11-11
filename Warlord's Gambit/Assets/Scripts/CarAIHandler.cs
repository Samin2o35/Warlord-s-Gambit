using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, folllowWaypoints };

    [Header("AI Settings")]
    public AIMode aiMode;

    // local variables
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;
    
    // componenets
    CarController carController;
    
    void Awake()
    {
        carController = GetComponent<CarController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame and is frame dependent
    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch(aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;
        }

        inputVector.x = TurnTowardsPlayer();
        inputVector.y = 1.0f;
        
        // do donuts
        //inputVector.x = 1.0f;
        //inputVector.y = 1.0f;

        // send input to car controller
        carController.SetInputVector(inputVector);
    }

    void FollowPlayer()
    {
        if(targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(targetTransform != null)
        {
            targetPosition = targetTransform.position;
        }
    }

    float TurnTowardsPlayer()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        // calculate angle towards target
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        //turn car as much as possible if angle greater than 45 degrees and smooth out if angle smaller
        float steerAmount = angleToTarget / 45.0f;

        // clamp steering between 1 and -1
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        return steerAmount;
    }
}
