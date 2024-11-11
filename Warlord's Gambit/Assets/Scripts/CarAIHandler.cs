using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, folllowWaypoints };

    [Header("AI Settings")]
    public AIMode aiMode;

    // local variables
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;

    // waypoints
    WaypointNode currentWaypoint = null;
    WaypointNode[] allWayPoints;

    // componenets
    CarController carController;

    void Awake()
    {
        carController = GetComponent<CarController>();
        allWayPoints = FindObjectsOfType<WaypointNode>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame and is frame dependent
    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch (aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;

            case AIMode.folllowWaypoints:
                FollowWaypoints();
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
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
        }
    }
    void FollowWaypoints()
    {
        // pick closest waypont if not set
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWaypoint();
        }

        // set target on waypoint position
        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            // store how close we are to target
            float distanceToWaypoint = (targetPosition - transform.position).magnitude;

            // check if we are close enough to consider we have reached waypoint
            if (distanceToWaypoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
                // if close enough then follow next waypoint, choose random between multiple waypoints
                currentWaypoint = currentWaypoint.nextWaypointNode[Random.Range(0, currentWaypoint.nextWaypointNode.Length)];
            }
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



    // find closest waypoint to AI
    WaypointNode FindClosestWaypoint()
    {
        return allWayPoints
            .OrderBy(tag => Vector3.Distance(transform.position, tag.transform.position))
            .FirstOrDefault();
    }
}
