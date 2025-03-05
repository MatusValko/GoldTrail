using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapSorting : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
}
