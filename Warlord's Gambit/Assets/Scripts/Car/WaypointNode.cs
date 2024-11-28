using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    // max speed allowed when passing this waypoint
    [Header("Waypoint Speed")]
    public float maxSpeed = 0;
    
    [Header("Next Waypoint")]
    public float minDistanceToReachWaypoint = 5;
    
    public WaypointNode[] nextWaypointNode;
}
