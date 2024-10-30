using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap baseTilemap;
    public Tilemap decorationTilemap;

    // Tile references
    public TileBase waterTile;
    public TileBase grassTile;
    public TileBase hillTile;

    // Map setup
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float noiseScale = 0.1f;

    // Animated decorations
    public GameObject foamPrefab;
    public GameObject[] rockPrefabs;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Generate noise value for this tile
                float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                // Set base tile based on noise value
                if (noiseValue < 0.3f)
                {
                    baseTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);

                    // Randomly place rocks in water
                    if (Random.value < 0.1f)
                    {
                        GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                        Instantiate(rockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    }
                }
                else if (noiseValue < 0.6f)
                {
                    baseTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }
                else
                {
                    baseTilemap.SetTile(new Vector3Int(x, y, 0), hillTile);
                }

                // Add water foam between water and grass/hill tiles
                AddWaterFoam(x, y, noiseValue);
            }
        }
    }

    void AddWaterFoam(int x, int y, float noiseValue)
    {
        // Check if water tile has a neighboring grass or hill tile
        if (noiseValue < 0.3f)
        {
            bool hasLandNeighbor = false;

            // Check the 4 neighbors (left, right, top, bottom)
            foreach (Vector2Int offset in new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down })
            {
                float neighborNoiseValue = Mathf.PerlinNoise((x + offset.x) * noiseScale, (y + offset.y) * noiseScale);
                if (neighborNoiseValue >= 0.3f) // If neighbor is grass or hill
                {
                    hasLandNeighbor = true;
                    break;
                }
            }

            if (hasLandNeighbor)
            {
                Instantiate(foamPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
}
