using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform[] spawnLocation;
    public int spawnIndex;

    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    private Camera mainCamera; // Reference to the main camera
    public float offScreenOffset = 2f; // Distance outside the screen boundaries

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; // Get the main camera reference
        UpdateSpawnLocationsOffScreen();
        GenerateWave();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawnTimer <= 0)
        {
            //spawn an enemy
            if (enemiesToSpawn.Count > 0)
            {
                // spawn first enemy in our list
                GameObject enemy = (GameObject)Instantiate(enemiesToSpawn[0], 
                    spawnLocation[spawnIndex].position, Quaternion.identity);

                // remove it from list
                enemiesToSpawn.RemoveAt(0);
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;

                if (spawnIndex + 1 <= spawnLocation.Length - 1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                // if no enemies remain, end wave
                waveTimer = 0;
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        if (waveTimer <= 0 && spawnedEnemies.Count <= 0)
        {
            currWave++;
            GenerateWave();
        }
    }

    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.

        // repeat... 

        //  -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    private void UpdateSpawnLocationsOffScreen()
    {
        // Get the camera's screen bounds
        Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Place spawn locations outside the screen
        spawnLocation[0].position = new Vector3(bottomLeft.x - offScreenOffset, Random.Range(bottomLeft.y, topRight.y), 0); // Left
        spawnLocation[1].position = new Vector3(topRight.x + offScreenOffset, Random.Range(bottomLeft.y, topRight.y), 0);  // Right
        spawnLocation[2].position = new Vector3(Random.Range(bottomLeft.x, topRight.x), bottomLeft.y - offScreenOffset, 0); // Bottom
        spawnLocation[3].position = new Vector3(Random.Range(bottomLeft.x, topRight.x), topRight.y + offScreenOffset, 0);  // Top
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere at each spawn location
        Gizmos.color = Color.yellow; // Set the color for the Gizmo
        if (spawnLocation != null)
        {
            foreach (Transform spawnPoint in spawnLocation)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.5f); // Draw a sphere with radius 0.5
                }
            }
        }
    }

}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}