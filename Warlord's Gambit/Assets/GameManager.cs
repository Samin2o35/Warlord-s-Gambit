using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject heroPrefab;
    public GameObject stickPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 firstPlatformPosition = new Vector3(0, -2, 0); // Match first platform position
        Instantiate(heroPrefab, firstPlatformPosition + Vector3.up * 1.5f, Quaternion.identity);

        Vector3 stickPosition = firstPlatformPosition + Vector3.up * 2; // Place stick near Hero
        Instantiate(stickPrefab, stickPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}