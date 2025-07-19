using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI _tileNameText;
    [SerializeField] private TextMeshProUGUI _tileDescriptionText;
    [SerializeField] private GameObject _tileInfoWindowPrefab;


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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //show the tile info window
    public void ShowTileInfoWindow(Vector3Int tilePos, string tileName = "name", string tileDescription = "desc")
    {
        TileBase clickedTile = TileManager.Instance.tilemap.GetTile(tilePos);
        // Set the position of the TileInfoWindow to the specified position
        if (clickedTile == null)
        {
            DebugLogger.Log($"No tile found at {tilePos}.");
            return;
        }

        _tileNameText.text = clickedTile.name;
        _tileDescriptionText.text = "Total Clicks: " + TileManager.Instance.GetClickCount(tilePos).ToString(); // Assuming you want to show the type of the tile

        // Set the text of the TileInfoWindow to the specified tile name and description
        // Assuming you have a TextMeshProUGUI component in your TileInfoWindow GameObject

        // Activate the TileInfoWindow GameObject
        _tileInfoWindowPrefab.SetActive(true);

    }
}
