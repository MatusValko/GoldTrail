using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Assign the Tilemap in the Inspector
    public TileBase[] tiles; // Assign the tile palette in the Inspector

    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    void Start()
    {
        GenerateTilemap();
    }

    void GenerateTilemap()
    {
        if (tilemap == null || tiles.Length == 0)
        {
            Debug.LogError("Tilemap or Tiles array is not set!");
            return;
        }
        tilemap.ClearAllTiles();

        int centerX = width / 2;
        int centerY = height / 2;

        for (int x = -centerX; x < centerX; x++)
        {
            for (int y = -centerY; y < centerY; y++)
            {
                TileBase randomTile = tiles[Random.Range(0, tiles.Length)];
                tilemap.SetTile(new Vector3Int(x, y, 0), randomTile);
            }
        }
    }
}
