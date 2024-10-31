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

    public TileBase hillCenter;
    public TileBase hillTopEdge, hillBottomEdge, hillLeftEdge, hillRightEdge;
    public TileBase hillTopLeftCorner, hillTopRightCorner, hillBottomLeftCorner, hillBottomRightCorner;
    public TileBase cliffTile;

    public int mapWidth;
    public int mapHeight;
    public float scale = 0.1f;
    public float rockSpawnFrequency;

    private bool[,] shadowMap;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        bool[,] terrainMap = new bool[mapWidth, mapHeight];
        shadowMap = new bool[mapWidth, mapHeight];

        // Generate a noise-based terrain layout
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * scale, y * scale);
                terrainMap[x, y] = perlinValue > 0.5f; // True for beach, False for water
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
                    // Determine the type of beach tile based on surroundings
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

                    // Randomly designate some beach tiles as shadow tiles for future hill placement
                    if (Random.value < 0.2f)
                    {
                        shadowMap[x, y] = true;
                    }
                }
                else
                {
                    waterTilemap.SetTile(tilePosition, waterTile);

                    // Randomly place rocks in water, but not on shadow tiles
                    if (!shadowMap[x, y] && Random.value < rockSpawnFrequency)
                    {
                        GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                        Vector3 rockPosition = waterTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0);
                        Instantiate(rockPrefab, rockPosition, Quaternion.identity);
                    }
                }
            }
        }

        // Hill Placement on Shadow Tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (shadowMap[x, y])
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);

                    // Determine hill tile type based on neighbors
                    bool top = y + 1 < mapHeight && shadowMap[x, y + 1];
                    bool bottom = y - 1 >= 0 && shadowMap[x, y - 1];
                    bool left = x - 1 >= 0 && shadowMap[x - 1, y];
                    bool right = x + 1 < mapWidth && shadowMap[x + 1, y];

                    TileBase hillTileToPlace = hillCenter;

                    if (!top && !left) hillTileToPlace = hillTopLeftCorner;
                    else if (!top && !right) hillTileToPlace = hillTopRightCorner;
                    else if (!bottom && !left) hillTileToPlace = hillBottomLeftCorner;
                    else if (!bottom && !right) hillTileToPlace = hillBottomRightCorner;
                    else if (!top) hillTileToPlace = hillTopEdge;
                    else if (!bottom) hillTileToPlace = hillBottomEdge;
                    else if (!left) hillTileToPlace = hillLeftEdge;
                    else if (!right) hillTileToPlace = hillRightEdge;

                    beachTilemap.SetTile(tilePosition, hillTileToPlace);

                    // Add cliff tiles below bottom edges and corners
                    if (hillTileToPlace == hillBottomEdge || hillTileToPlace == hillBottomLeftCorner || hillTileToPlace == hillBottomRightCorner)
                    {
                        int cliffDepth = Random.Range(1, 3); // Randomize cliff height
                        for (int cliffLevel = 1; cliffLevel <= cliffDepth; cliffLevel++)
                        {
                            Vector3Int cliffPosition = new Vector3Int(x, y - cliffLevel, 0);

                            // Stop if out of bounds or another shadow tile is in the way
                            if (cliffPosition.y < 0 || shadowMap[x, y - cliffLevel]) break;

                            beachTilemap.SetTile(cliffPosition, cliffTile);
                        }
                    }
                }
            }
        }
    }
}
