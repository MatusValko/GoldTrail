using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileState
{

    public TileStateVisibility visibility;
    public int clickCount;
    //variable to track how many clicks it needs to be discovered
    public int clicksToDiscover; // How many clicks to discover the tile
}
public enum TileStateVisibility
{
    Hidden,     // Far away (black)
    Near,       // Can be discovered (grey)
    Discovered  // Fully visible
}


[Serializable]
public class TileSaveData
{
    public List<Vector3IntSerializable> tilePositions = new List<Vector3IntSerializable>();
    public List<TileState> tileStates = new List<TileState>();
}

[Serializable]
public struct Vector3IntSerializable
{
    public int x, y, z;

    public Vector3IntSerializable(Vector3Int vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);
}
