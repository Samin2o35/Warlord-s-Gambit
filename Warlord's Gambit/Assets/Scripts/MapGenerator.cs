using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public Tilemap waterTilemap, beachTilemap;
    public TileBase beachCenter;
    public TileBase beachTopEdge, beachBottomEdge, beachLeftEdge, beachRightEdge;
    public TileBase beachTopLeftCorner, beachTopRightCorner, beachBottomLeftCorner, beachBottomRightCorner;

    public GameObject foamPrefab;
    public GameObject[] rockPrefabs; // Array for rock prefabs
    public TileBase waterTile;

    public int mapWidth;
    public int mapHeight;
    public float scale = 0.1f;
    public float spawnFrequency;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        bool[,] terrainMap = new bool[mapWidth, mapHeight];

        // First, generate a noise-based terrain layout
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * scale, y * scale);
                terrainMap[x, y] = perlinValue > 0.5f; // True for  beach, False for water
            }
        }

        // Place tiles based on terrain layout and surrounding tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
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

                    beachTilemap.SetTile(new Vector3Int(x, y, 0), tileToPlace);

                    // Check for neighboring water tiles to spawn foam
                    if (!top || !bottom || !left || !right)
                    {
                        Vector3Int tilePosition = new Vector3Int(x, y, 0);
                        Vector3 foamPosition = beachTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0); // Center foam sprite on tile
                        //Instantiate(foamPrefab, foamPosition, Quaternion.identity, tilemap.transform);
                        Instantiate(foamPrefab, foamPosition, Quaternion.identity);
                    }
                }
                else
                {
                    waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);

                    // Randomly place rocks in water
                    if (Random.value < spawnFrequency)
                    {
                        GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                        Instantiate(rockPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                }
            }
        }
    }
}
