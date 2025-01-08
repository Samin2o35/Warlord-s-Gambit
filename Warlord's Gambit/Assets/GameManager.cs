using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject heroPrefab;
    public float heroSpawnHeight;

    // Start is called before the first frame update
    void Start()
    {
        SpawnHeroOnStartingPlatform();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnHeroOnStartingPlatform()
    {
        // Find the platform with the "StartingPlatform" tag
        GameObject startingPlatform = GameObject.FindGameObjectWithTag("Starting Platform");

        if (startingPlatform != null)
        {
            // Get the platform's position
            Vector3 platformPosition = startingPlatform.transform.position;

            // Spawn the Hero slightly above the platform
            Vector3 heroSpawnPosition = platformPosition + Vector3.up * heroSpawnHeight; // Adjust Y offset as needed
            Instantiate(heroPrefab, heroSpawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No platform with the 'StartingPlatform' tag found in the scene.");
        }
    }
}