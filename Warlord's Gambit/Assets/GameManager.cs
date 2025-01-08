using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject playerPrefab; // Drag the player prefab here in the Inspector
    public Vector3 spawnOffset = Vector3.zero; // Adjust this in the Inspector for the desired offset

    void Start()
    {
        // Find the starting platform
        GameObject startingPlatform = GameObject.FindGameObjectWithTag("StartingPlatform");

        if (startingPlatform != null)
        {
            // Get the platform's position and apply the offset
            Vector3 spawnPosition = startingPlatform.transform.position + spawnOffset;

            // Instantiate the player prefab at the adjusted spawn position
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No GameObject with the tag 'StartingPlatform' found in the scene.");
        }
    }
}
