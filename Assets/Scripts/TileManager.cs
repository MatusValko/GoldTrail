using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public BoundsInt bounds;

    private Dictionary<Vector3Int, TileState> tileStates = new Dictionary<Vector3Int, TileState>();

    private string saveFilePath;

    public static TileManager Instance;
    public Color discoveredColor = Color.white;
    public Color nearColor = Color.gray;
    public Color hiddenColor = Color.black;
    public int discoveryRange = 1; // How far to mark as near tiles

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (tilemap == null)
        {
            // Find the tilemap in the scene
            tilemap = FindFirstObjectByType<Tilemap>();
        }
        bounds = tilemap.cellBounds;
    }

    private void Start()
    {
        // saveFilePath = Path.Combine(Application.persistentDataPath, "tileSaveDataTest.json");
        // _createNewGame();
        // InitializeTiles();

        // LoadProgress(); // Load saved tile data
    }
    private void _createNewGame()
    {
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && !tileStates.ContainsKey(pos))
            {

                tileStates[pos] = new TileState { visibility = TileStateVisibility.Hidden };
                // DebugLogger.Log($"Tile at {pos} initialized as Hidden.");
            }
            // UpdateTileVisual(pos);
        }
        //set disvoered color center tile
        Vector3Int centerTile = new Vector3Int(0, 0, 0);
        if (tilemap.HasTile(centerTile))
        {
            tileStates[centerTile].visibility = TileStateVisibility.Discovered;
            tilemap.SetColor(centerTile, discoveredColor);
            SetNearTiles(centerTile, 2);
        }
        DebugLogger.Log("Tile progress initialized!");
        RefreshTileColors(); // Refresh colors based on initialized data
    }

    private void InitializeTiles()
    {
        // Ensure all tiles have a state, even if not loaded from save
        // BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && !tileStates.ContainsKey(pos))
            {
                tileStates[pos] = new TileState();
            }
            // UpdateTileVisual(pos);
        }


        // RefreshTileColors();

    }

    //select tile on click
    public void SelectTile(Vector3Int tilePos)
    {

        if (tileStates.ContainsKey(tilePos))
        {
            if (tileStates[tilePos].visibility == TileStateVisibility.Hidden)
            {
                DebugLogger.Log($"Tile {tilePos} is hidden. Cannot select.");
                return;
            }
            tileStates[tilePos].clickCount++;
            if (tileStates[tilePos].visibility == TileStateVisibility.Near)
            {
                DebugLogger.Log($"Tile {tilePos} is near");
                if (_canClickOnNearTile(tilePos))
                {
                    DebugLogger.Log($"Tile {tilePos} can be discovered.");
                    DiscoverTile(tilePos);
                }
            }
            else if (tileStates[tilePos].visibility == TileStateVisibility.Discovered)
            {
                DebugLogger.Log($"Tile {tilePos} is already discovered.");
                _showTileInfoWindow(tilePos);
            }
            else
            {
                DebugLogger.Log($"Tile {tilePos} is not in a valid state for selection.");
            }

            // SaveProgress(); // Save progress after interaction
        }
    }

    private void _showTileInfoWindow(Vector3Int clickedTilePos)
    {
        UIManager.instance.ShowTileInfoWindow(clickedTilePos);
    }

    private bool _canClickOnNearTile(Vector3Int position)
    {
        // Check adjacent tiles (x+1, y+1, x-1, y-1)
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),  // x+1
            new Vector3Int(-1, 0, 0), // x-1
            new Vector3Int(0, 1, 0),  // y+1
            new Vector3Int(0, -1, 0), // y-1
        };
        foreach (Vector3Int dir in directions)
        {
            Vector3Int adjacentTile = position + dir;
            if (tileStates.ContainsKey(adjacentTile))
            {
                // If any of the adjacent tiles are discovered, return true
                if (tileStates[adjacentTile].visibility == TileStateVisibility.Discovered)
                {
                    return true;
                }
            }
        }
        // If no adjacent tiles are discovered, return false
        return false;
    }

    public void DiscoverTile(Vector3Int tilePos)
    {
        if (tileStates[tilePos].visibility != TileStateVisibility.Discovered)
        {
            tileStates[tilePos].visibility = TileStateVisibility.Discovered;
        }

        // DebugLogger.Log($"Tile {tilePos} clicked {tileStates[tilePos].clickCount} times.");

        UpdateTileVisual(tilePos);
        SetNearTiles(tilePos, discoveryRange);
    }

    private void UpdateTileVisual(Vector3Int tilePos)
    {
        // Color tileColor = tileStates[tilePos].isDiscovered ? Color.white : Color.black;
        // DebugLogger.Log($"Tile {tilePos} visual updated to {tileColor}.");
        if (tileStates[tilePos].visibility == TileStateVisibility.Hidden)
        {
            tilemap.SetColor(tilePos, hiddenColor);

        }
        else if (tileStates[tilePos].visibility == TileStateVisibility.Near)
        {
            tilemap.SetColor(tilePos, nearColor);

        }
        else if (tileStates[tilePos].visibility == TileStateVisibility.Discovered)
        {
            tilemap.SetColor(tilePos, discoveredColor);

        }

    }

    //get click count of tile
    public int GetClickCount(Vector3Int tilePos)
    {
        if (tileStates.ContainsKey(tilePos))
        {
            return tileStates[tilePos].clickCount;
        }
        return 0;
    }


    void RefreshTileColors()
    {
        foreach (var tile in tileStates)
        {
            Color colorToSet = hiddenColor; // Default to hidden (black)
            switch (tile.Value.visibility)
            {
                case TileStateVisibility.Discovered:
                    colorToSet = discoveredColor;
                    break;
                case TileStateVisibility.Near:
                    colorToSet = nearColor;
                    break;
                case TileStateVisibility.Hidden:
                    colorToSet = hiddenColor;
                    break;
            }
            tilemap.SetColor(tile.Key, colorToSet);
        }
    }

    public void SaveProgress()
    {
        TileSaveData saveData = new TileSaveData();

        foreach (var kvp in tileStates)
        {
            saveData.tilePositions.Add(new Vector3IntSerializable(kvp.Key));
            saveData.tileStates.Add(kvp.Value);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        DebugLogger.Log("Tile progress saved!");
    }

    public void LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            TileSaveData saveData = JsonUtility.FromJson<TileSaveData>(json);

            tileStates.Clear();

            for (int i = 0; i < saveData.tilePositions.Count; i++)
            {
                Vector3Int pos = saveData.tilePositions[i].ToVector3Int();
                tileStates[pos] = saveData.tileStates[i];

                // Apply loaded visual state
                // UpdateTileVisual(pos);
            }

            DebugLogger.Log("Tile progress loaded!");
            RefreshTileColors(); // Refresh colors based on initialized data
        }
        else
        {
            _createNewGame();
        }
    }

    public void SetNearTiles(Vector3Int centerTile, int range = 1)
    {
        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                Vector3Int adjacentTile = centerTile + new Vector3Int(i, j, 0);
                if (!tileStates.ContainsKey(adjacentTile)) continue;

                // int distance = Mathf.Abs(i) + Mathf.Abs(j); // Manhattan distance from center
                // int discoveryCost = Mathf.Max(1, distance); 


                // Conditions to match your cross-like pattern
                bool isCross = (i == 0 || j == 0);  // Up, down, left, right
                bool isDiagonal = (Mathf.Abs(i) == Mathf.Abs(j) && Mathf.Abs(i) < range); // Inner diagonals


                // bool isCross = (i == 0 || j == 0);

                // // Diagonal expansion (progressive difficulty)
                // bool isDiagonal = Mathf.Abs(i) == Mathf.Abs(j) && Mathf.Abs(i) <= range;
                if (isCross || isDiagonal)
                {
                    if (tileStates[adjacentTile].visibility != TileStateVisibility.Discovered)
                    {
                        tileStates[adjacentTile].visibility = TileStateVisibility.Near;
                        tilemap.SetColor(adjacentTile, nearColor);

                    }
                }
            }
        }

        // RefreshTileColors();
    }




    private void OnApplicationQuit()
    {
        // SaveProgress();
    }
}
