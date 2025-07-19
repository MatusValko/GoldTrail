using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridSize : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = TileManager.Instance.bounds;

        // Calculate width and height
        int width = bounds.size.x;
        int height = bounds.size.y;

        Debug.Log($"Tilemap Size: Width = {width}, Height = {height}");
        text.text = $"W = {width}, H = {height}";
    }
}
