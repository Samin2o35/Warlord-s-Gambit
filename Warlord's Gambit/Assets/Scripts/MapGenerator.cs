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
    public float shadowSpawnFrequency = 0.2f;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        bool[,] terrainMap = new bool[mapWidth, mapHeight];
        bool[,] shadowMap = new bool[mapWidth, mapHeight];

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

                    // Randomly place rocks in water
                    if (Random.value < rockSpawnFrequency)
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

                // Hill Tile Logic (can be placed on both water and beach)
                /*if (Random.value < hillSpawnFrequency)
                {
                    // Check surroundings to determine type of hill tile
                    bool top = y + 1 < mapHeight && terrainMap[x, y + 1];
                    bool bottom = y - 1 >= 0 && terrainMap[x, y - 1];
                    bool left = x - 1 >= 0 && terrainMap[x - 1, y];
                    bool right = x + 1 < mapWidth && terrainMap[x + 1, y];

                    TileBase hillTileToPlace = hillCenter;

                    if (!top && !left) hillTileToPlace = hillTopLeftCorner;
                    else if (!top && !right) hillTileToPlace = hillTopRightCorner;
                    else if (!bottom && !left) hillTileToPlace = hillBottomLeftCorner;
                    else if (!bottom && !right) hillTileToPlace = hillBottomRightCorner;
                    else if (!top) hillTileToPlace = hillTopEdge;
                    else if (!bottom) hillTileToPlace = hillBottomEdge;
                    else if (!left) hillTileToPlace = hillLeftEdge;
                    else if (!right) hillTileToPlace = hillRightEdge;

                    hillTilemap.SetTile(tilePosition, hillTileToPlace);
                }*/
            }
        }
    }
}
