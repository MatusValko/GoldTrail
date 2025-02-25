using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Tilemap tilemap;

    public GameObject TileWindow;

    void Update()
    {
        _clickOnTile();
    }

    private void _clickOnTile()
    {
        if (Input.GetMouseButtonUp(0)) // Left click
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = tilemap.GetTile(tilePos);

            if (clickedTile != null)
            {
                DebugLogger.Log($"Clicked on tile at {tilePos}, Tile: {clickedTile.name}");
                _showTileInfoWindow(clickedTile);
            }
            else
            {
                DebugLogger.Log($"Clicked on empty tile at {tilePos}");
            }
        }
    }

    private void _showTileInfoWindow(TileBase clickedTile)
    {
        DebugLogger.Log("TILE: " + clickedTile.name);
        DebugLogger.Log("Instance ID: " + clickedTile.GetInstanceID());

        // Show a window with tile info


        // Example: Show a window with the tile name
        TileWindow.SetActive(true);
        TileWindow.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = clickedTile.name;
    }

    //set actite off the popup window
    public void CloseWindow()
    {
        TileWindow.SetActive(false);
    }
}
