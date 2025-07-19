using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Assign the Tilemap in the Inspector
                            // public TileBase[] tiles; // Assign the tile palette in the Inspector

    [SerializeField] private TilesSO tilesSO; // Assign the tile palette in the Inspector
    [SerializeField] private int width = 300;
    [SerializeField] private int height = 300;


#if UNITY_EDITOR
    [ContextMenu("Generate Tilemap")]
    private void GenerateTilemapFromInspector()
    {
        GenerateTilemap();
    }
#endif
    // void Start()
    // {
    //     GenerateTilemap();
    // }

    void GenerateTilemap()
    {
        //how to access the tiles from TilesSO



        TileBase[] tiles = tilesSO.Tiles;

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
                TileBase tileToSet;
                // Check if (x, y) is the center tile
                if (x == 0 && y == 0)
                {
                    tileToSet = tiles[7]; // center tile
                }
                // Check if (x, y) is within the center 3x3 area (excluding center)
                else if (Mathf.Abs(x) <= 3 && Mathf.Abs(y) <= 3)
                {
                    tileToSet = tiles[5]; // zero tile (first tile in array)
                }
                else
                {
                    tileToSet = tiles[Random.Range(0, tiles.Length)];
                }
                tilemap.SetTile(new Vector3Int(x, y, 0), tileToSet);
            }
        }
    }
}
