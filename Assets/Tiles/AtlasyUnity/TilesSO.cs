using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilesSO", menuName = "Scriptable Objects/TilesSO")]
public class TilesSO : ScriptableObject
{
    // public Tile[] Tiles;

    // [System.Serializable]
    // public class Tile
    // {
    //     public string Name;
    //     public Sprite Sprite;
    //     public int Cost;
    //     public string Description;
    // }

    public TileBase[] Tiles;
}
