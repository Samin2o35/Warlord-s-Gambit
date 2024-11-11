using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("Next Waypoint")]
    public float minDistanceToReachWaypoint = 5;
    
    public WaypointNode[] nextWaypointNode;
}
