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

    public static TileManager instance;


    public Color discoveredColor = Color.white;
    public Color nearColor = Color.gray;
    public Color hiddenColor = Color.black;
    public int discoveryRange = 1; // How far to mark as near tiles
    public int plusSize = 3; // How far to mark as near tiles in plus shape

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
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
        saveFilePath = Path.Combine(Application.persistentDataPath, "tileSaveDataTest.json");

        LoadProgress(); // Load saved tile data
        // InitializeTiles();
        RefreshTileColors(); // Refresh colors based on loaded data
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



    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector3Int tilePos = tilemap.WorldToCell(worldPos);

        //     if (tileStates.ContainsKey(tilePos))
        //     {
        //         DiscoverTile(tilePos);
        //         // SaveProgress(); // Save progress after interaction
        //     }
        // }
    }

    //select tile on click
    public void SelectTile(Vector3Int tilePos)
    {
        tileStates[tilePos].clickCount++;
        if (tileStates.ContainsKey(tilePos))
        {
            if (tileStates[tilePos].visibility == TileStateVisibility.Hidden)
            {
                DebugLogger.Log($"Tile {tilePos} is hidden. Cannot select.");
                return;
            }
            else if (tileStates[tilePos].visibility == TileStateVisibility.Near)
            {
                DebugLogger.Log($"Tile {tilePos} is near");
                if (CanClickOnNearTile(tilePos))
                {
                    DebugLogger.Log($"Tile {tilePos} can be discovered.");
                    DiscoverTile(tilePos);
                }
            }
            else if (tileStates[tilePos].visibility == TileStateVisibility.Discovered)
            {
                DebugLogger.Log($"Tile {tilePos} is already discovered.");
            }
            else
            {
                DebugLogger.Log($"Tile {tilePos} is not in a valid state for selection.");
            }


            // DiscoverTile(tilePos);
            // SaveProgress(); // Save progress after interaction
        }
    }

    bool CanClickOnNearTile(Vector3Int position)
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
        // DebugLogger.Log($"Tile {tilePos} discovered: {tileStates[tilePos].isDiscovered}.");

        // if (!tileStates[tilePos].isDiscovered)
        // {
        //     tileStates[tilePos].isDiscovered = true;
        // }

        if (tileStates[tilePos].visibility != TileStateVisibility.Discovered)
        {
            tileStates[tilePos].visibility = TileStateVisibility.Discovered;
        }


        tileStates[tilePos].clickCount++;
        DebugLogger.Log($"Tile {tilePos} clicked {tileStates[tilePos].clickCount} times.");

        UpdateTileVisual(tilePos);
        SetNearTiles(tilePos, 1);
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
        }
        else
        {
            DebugLogger.Log("No save data found!.");
            // Initialize tiles if no save data exists
            // InitializeTiles(); // Uncomment if you want to initialize tiles when no save data is found
            // Or you can set a default state for all tiles
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos) && !tileStates.ContainsKey(pos))
                {
                    tileStates[pos] = new TileState { visibility = TileStateVisibility.Hidden };
                }
                // UpdateTileVisual(pos);
            }
            //set disvoered color center tile
            Vector3Int centerTile = new Vector3Int(0, 0, 0);
            if (tilemap.HasTile(centerTile))
            {
                tileStates[centerTile].visibility = TileStateVisibility.Discovered;
                tilemap.SetColor(centerTile, discoveredColor);
                SetNearTiles(centerTile);
            }


            DebugLogger.Log("Tile progress initialized!");
        }

        RefreshTileColors(); // Refresh colors based on initialized data

    }

    // public void SetNearTiles(Vector3Int centerTile)
    // {
    //     int squareSize = 3; // Size of the square around the center tile
    //     // Set a square of "near" tiles around the center tile
    //     for (int dx = -squareSize; dx <= squareSize; dx++)
    //     {
    //         for (int dy = -squareSize; dy <= squareSize; dy++)
    //         {
    //             Vector3Int adjacentTile = centerTile + new Vector3Int(dx, dy, 0);

    //             if (tileStates.ContainsKey(adjacentTile))
    //             {
    //                 // Skip the center tile itself if it's already discovered
    //                 if (adjacentTile != centerTile)
    //                 {
    //                     //if tile is not discovered 
    //                     if (tileStates[adjacentTile].visibility != TileStateVisibility.Discovered)
    //                     {
    //                         tileStates[adjacentTile].visibility = TileStateVisibility.Near;
    //                         UpdateTileVisual(adjacentTile);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
    public void SetNearTiles(Vector3Int centerTile, int plusSize = 3)
    {
        // Iterate in a plus shape (up, down, left, right)
        for (int i = -plusSize; i <= plusSize; i++)
        {
            Vector3Int[] checkTiles =
            {
            centerTile + new Vector3Int(0, i, 0),  // Vertical (up/down)
            centerTile + new Vector3Int(0, -i, 0),
            centerTile + new Vector3Int(i, 0, 0),  // Horizontal (left/right)
            centerTile + new Vector3Int(-i, 0, 0)
        };

            foreach (Vector3Int adjacentTile in checkTiles)
            {
                // Check if the tile exists and is NOT already discovered
                if (tileStates.ContainsKey(adjacentTile) && tileStates[adjacentTile].visibility != TileStateVisibility.Discovered)
                {
                    tileStates[adjacentTile].visibility = TileStateVisibility.Near;
                    UpdateTileVisual(adjacentTile);
                }
            }
        }

        // RefreshTileColors();
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }
}
