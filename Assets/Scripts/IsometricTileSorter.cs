using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricTileSorter : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapRenderer tileRenderer;
    void Start()
    {
        FixTileSorting();
    }

    public void FixTileSorting()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                int sortingOrder = Mathf.RoundToInt(-pos.y * 10); // Higher Y = lower sorting order
                tilemap.SetTileFlags(pos, TileFlags.None);
                tilemap.SetColor(pos, new Color(1, 1, 1, 1)); // Force redraw (fixes sorting issues)
                // TilemapRenderer tileRenderer = GetComponent<TilemapRenderer>();
                tileRenderer.sortingOrder = sortingOrder;
            }
        }
    }
}
