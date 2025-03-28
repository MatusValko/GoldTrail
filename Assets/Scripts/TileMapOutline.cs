using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapOutline : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeY;
    public float tileSize = 1f; // Adjust for correct scale
    public float tileSizeY = 0.58f; // Adjust for correct scale
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private BoundsInt bounds;


    private void Start()
    {
        bounds = TileManager.instance.bounds;
        gridSizeX = bounds.size.x;
        gridSizeY = bounds.size.y;
        MakeLine();
    }

    public void MakeLine()
    {
        // lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5; // 4 corners + closing point
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Assign a visible material
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.sortingOrder = 50;

        UpdateOutline();
    }

    //set line width
    public void SetLineWidth(float width)
    {
        //TODO make it to 3* the default value at max, 0.5f * (zoom/100)

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    private void UpdateOutline()
    {
        Vector3[] corners = new Vector3[5];

        // Centering the 0,0 position
        Vector3 center = IsoPosition(0, 0);

        corners[0] = IsoPosition(-gridSizeX / 2, -gridSizeY / 2);
        corners[1] = IsoPosition(gridSizeX / 2, -gridSizeY / 2);
        corners[2] = IsoPosition(gridSizeX / 2, gridSizeY / 2);
        corners[3] = IsoPosition(-gridSizeX / 2, gridSizeY / 2);
        corners[4] = corners[0]; // Close loop

        lineRenderer.SetPositions(corners);

        // Vector3[] corners = new Vector3[5]; // 4 corners + close loop
        // corners[0] = IsoPosition(0, 0);
        // corners[1] = IsoPosition(gridSizeX, 0);
        // corners[2] = IsoPosition(gridSizeX, gridSizeY);
        // corners[3] = IsoPosition(0, gridSizeY);
        // corners[4] = corners[0]; // Close loop

        // lineRenderer.SetPositions(corners);
    }

    private Vector3 IsoPosition(int x, int y)
    {
        float isoX = (x - y) * 0.5f;  // Half-width for isometric
        float isoY = (x + y) * (tileSizeY / 2); // Adjusted height (0.58 / 2)
        return new Vector3(isoX, isoY, 0);
    }
}
