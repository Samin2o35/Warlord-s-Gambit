using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public Tilemap waterTilemap, beachTilemap, shadowTilemap;
    public TileBase beachCenter, shadowTile; 
    public TileBase beachTopEdge, beachBottomEdge, beachLeftEdge, beachRightEdge;
    public TileBase beachTopLeftCorner, beachTopRightCorner, beachBottomLeftCorner, beachBottomRightCorner;

    public GameObject foamPrefab;
    public GameObject[] rockPrefabs; // Array for rock prefabs
    public TileBase waterTile;

    public int mapWidth;
    public int mapHeight;
    public float scale = 0.1f;
    public float rockSpawnFrequency = 0.1f;

    public int clusterProbability = 45; // Chance of a cell being a shadow tile
    public int smoothingIterations = 5; // Number of times to smooth for clustering

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        bool[,] terrainMap = new bool[mapWidth, mapHeight];
        bool[,] shadowMap = new bool[mapWidth, mapHeight];

        // Generate a noise-based terrain layout for bach and water
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * scale, y * scale);
                terrainMap[x, y] = perlinValue > 0.5f; // True for  beach, False for water
            }
        }

        // Generate a noise-based terrain layout for shadow
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                shadowMap[x, y] = Random.Range(0, 100) < clusterProbability;
            }
        }

        // Step 3: Apply cellular automata to cluster shadow tiles
        for (int i = 0; i < smoothingIterations; i++)
        {
            shadowMap = SmoothShadowMap(shadowMap);
        }

        // Place tiles based on terrain layout and surrounding tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (terrainMap[x, y])
                {
                    // Check surroundings to determine the type of grass tile
                    bool top = y + 1 < mapHeight && terrainMap[x, y + 1];
                    bool bottom = y - 1 >= 0 && terrainMap[x, y - 1];
                    bool left = x - 1 >= 0 && terrainMap[x - 1, y];
                    bool right = x + 1 < mapWidth && terrainMap[x + 1, y];

                    TileBase tileToPlace = beachCenter;

                    if (!top && !left) tileToPlace = beachTopLeftCorner;
                    else if (!top && !right) tileToPlace = beachTopRightCorner;
                    else if (!bottom && !left) tileToPlace = beachBottomLeftCorner;
                    else if (!bottom && !right) tileToPlace = beachBottomRightCorner;
                    else if (!top) tileToPlace = beachTopEdge;
                    else if (!bottom) tileToPlace = beachBottomEdge;
                    else if (!left) tileToPlace = beachLeftEdge;
                    else if (!right) tileToPlace = beachRightEdge;

                    beachTilemap.SetTile(tilePosition, tileToPlace);

                    // Check for neighboring water tiles to spawn foam
                    if (!top || !bottom || !left || !right)
                    {
                        Vector3 foamPosition = beachTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0); // Center foam sprite on tile
                        Instantiate(foamPrefab, foamPosition, Quaternion.identity);
                    }
                }
                else
                {
                    waterTilemap.SetTile(tilePosition, waterTile);

                    // Randomly place rocks in water only if the current tile is not a shadow tile
                    if (!shadowMap[x, y] && Random.value < spawnFrequency)
                    {
                        GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                        Instantiate(rockPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                }

                // Shadow tile placement from the shadowMap
                if (shadowMap[x, y])
                {
                    shadowTilemap.SetTile(tilePosition, shadowTile);
                }
            }
        }
    }

    // Smooth shadowMap using cellular automata
    bool[,] SmoothShadowMap(bool[,] shadowMap)
    {
        bool[,] newShadowMap = new bool[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int neighborCount = CountNeighbors(shadowMap, x, y);

                if (neighborCount > 4)
                {
                    newShadowMap[x, y] = true;
                }
                else if (neighborCount < 4)
                {
                    newShadowMap[x, y] = false;
                }
                else
                {
                    newShadowMap[x, y] = shadowMap[x, y];
                }
            }
        }

        return newShadowMap;
    }

    // Count neighboring shadow tiles
    int CountNeighbors(bool[,] shadowMap, int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nx = x + i;
                int ny = y + j;

                if (i == 0 && j == 0) continue; // Skip self

                if (nx >= 0 && nx < mapWidth && ny >= 0 && ny < mapHeight)
                {
                    if (shadowMap[nx, ny]) count++;
                }
            }
        }

        return count;
    }
}
