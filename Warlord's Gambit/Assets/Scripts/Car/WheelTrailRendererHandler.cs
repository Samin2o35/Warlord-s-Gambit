using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRendererHandler : MonoBehaviour
{
    // componenets
    CarController carController;
    TrailRenderer trailRenderer;
    
    void Awake()
    {
        // Get car controller
        carController = GetComponentInParent<CarController>();
        
        // Get trail renderer
        trailRenderer = GetComponent<TrailRenderer>();
        
        // Disable trail renderer at start
        trailRenderer.emitting = false;
    }

    void Update()
    {
        // if tires are screeching then emit trail
        if(carController.isTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            trailRenderer.emitting = true;
        }
        else trailRenderer.emitting = false;
    }
}
