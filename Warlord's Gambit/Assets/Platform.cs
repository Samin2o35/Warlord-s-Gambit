using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float Width => transform.localScale.x; // Returns the width of the platform
    public Vector3 Position => transform.position; // Returns the position of the platform

    public void SetWidth(float width)
    {
        // Adjusts the platform's width
        Vector3 scale = transform.localScale;
        scale.x = width;
        transform.localScale = scale;
    }
}