using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab; // Assign the platform prefab here
    public int initialPlatformCount = 5; // Number of platforms to generate at the start
    public float minPlatformWidth = 2f; // Minimum width of a platform
    public float maxPlatformWidth = 5f; // Maximum width of a platform
    public float minGap = 1.5f; // Minimum gap between platforms
    public float maxGap = 3f; // Maximum gap between platforms

    public Vector3 startPosition = new Vector3(0, -2, 0); // Custom starting position
    private Vector3 nextPosition = Vector3.zero;

    void Start()
    {
        // Initialize nextPosition to startPosition
        nextPosition = startPosition;

        GenerateInitialPlatforms();
    }

    void GenerateInitialPlatforms()
    {
        for (int i = 0; i < initialPlatformCount; i++)
        {
            float width = Random.Range(minPlatformWidth, maxPlatformWidth);
            GameObject platform = Instantiate(platformPrefab, nextPosition, Quaternion.identity);
            platform.GetComponent<Platform>().SetWidth(width);

            // Update next position
            float gap = Random.Range(minGap, maxGap);
            nextPosition.x += width + gap;
        }
    }
}