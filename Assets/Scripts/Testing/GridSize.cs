using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSize : MonoBehaviour
{
    public Tilemap tilemap;
    public TextMeshProUGUI text;

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Calculate width and height
        int width = bounds.size.x;
        int height = bounds.size.y;

        Debug.Log($"Tilemap Size: Width = {width}, Height = {height}");
        text.text = $"W = {width}, H = {height}";
    }
}
