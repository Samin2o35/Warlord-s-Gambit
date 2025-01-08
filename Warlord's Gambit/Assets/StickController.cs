using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    public float growthSpeed = 2f; // How fast the stick grows
    public float rotationSpeed = 100f; // Rotation speed in degrees per second

    private bool isGrowing = false;
    private bool isRotating = false;

    void Update()
    {
        if (isGrowing)
        {
            GrowStick();
        }
        else if (isRotating)
        {
            RotateStick();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            StartGrowing();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopGrowingAndStartRotating();
        }
    }

    void StartGrowing()
    {
        isGrowing = true;
    }

    void StopGrowingAndStartRotating()
    {
        isGrowing = false;
        isRotating = true;
    }

    void GrowStick()
    {
        transform.localScale += new Vector3(0, growthSpeed * Time.deltaTime, 0);
    }

    void RotateStick()
    {
        transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

        // Stop rotating after 90 degrees
        if (transform.eulerAngles.z <= 270 && transform.eulerAngles.z > 180)
        {
            isRotating = false;
        }
    }
}