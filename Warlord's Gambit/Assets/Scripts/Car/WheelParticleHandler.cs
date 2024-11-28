using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelParticleHandler : MonoBehaviour
{
    // local variables
    float particleEmissionRate = 0;

    // components
    CarController carController;

    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;
    
    void Awake()
    {
        // get car controller
        carController = GetComponentInParent<CarController>();

        // get particle system
        particleSystemSmoke = GetComponent<ParticleSystem>();

        // get emission componenet
        particleSystemEmissionModule = particleSystemSmoke.emission;

        // set to zero emission
        particleSystemEmissionModule.rateOverTime = 0;
    }

    void Update()
    {
        // reduce particles over time
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (carController.isTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            // emit smoke if tire screeching. emit more smoke for braking
            if (isBraking)
            {
                particleEmissionRate = 30;
            }

            // emit smoke based on player drifting
            else
            {
                particleEmissionRate = Mathf.Abs(lateralVelocity) * 2;
            }
        }
    }
}