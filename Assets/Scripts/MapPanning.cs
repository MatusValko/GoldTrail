using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapPanningZooming : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 15f;

    private Vector3 dragOrigin;


    public Tilemap tilemap;

    public GameObject TileWindow;

    // public GameObject highlightPrefab;   // Assign a highlight sprite in Unity
    // public GameObject infoPanel;         // Assign a UI Panel
    // public TextMeshProUGUI tileInfoText; // Assign a TextMeshPro element

    private Vector3 touchStartPos;
    private bool isDragging = false;
    private float dragThreshold = 10f; // Minimum movement (in pixels) to detect dragging

    void Update()
    {
        HandlePanning2();
        HandleZooming();
    }

    void HandlePanning()
    {
        if (Input.touchCount == 1) // Mobile drag
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(touch.position);
                cam.transform.position += difference;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Mouse drag
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += difference;
        }
    }

    void HandlePanning2()
    {
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonDown(0)) // Mouse click/touch start
        {
            // if (IsPointerOverUI()) return;
            touchStartPos = Input.mousePosition;
            isDragging = false;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetMouseButton(0)) // Mouse drag/touch move
        {
            if (Vector3.Distance(touchStartPos, Input.mousePosition) > dragThreshold)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                cam.transform.position += difference;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && !isDragging) // Mouse release, only select if not dragging
        {
            // if (IsPointerOverUI()) return;
            SelectTile();
            // TileSelector ts= GetComponent<TileSelector>();
        }

    }
    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    private void SelectTile()
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
        // DebugLogger.Log("TILE: " + clickedTile.name);
        // DebugLogger.Log("Instance ID: " + clickedTile.GetInstanceID());

        // Show a window with tile info


        // Example: Show a window with the tile name
        TileWindow.SetActive(true);
        TileWindow.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = clickedTile.name;
    }

    void HandleZooming()
    {
        if (Input.touchCount == 2) // Mobile pinch zoom
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = prevMagnitude - currentMagnitude;
            Zoom(difference * 0.05f);

        }

        float scroll = Input.GetAxis("Mouse ScrollWheel"); // PC scroll zoom
        if (scroll != 0)
        {
            Zoom(-scroll);
        }
    }

    void Zoom(float increment)
    {
        int fasterWhenFarther = (int)cam.orthographicSize / 10;
        increment = increment > 0 ? increment + fasterWhenFarther : increment - fasterWhenFarther;
        // float zoomFactor = cam.orthographicSize * 0.5f * ; // Scale zoom speed based on current zoom
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + (increment * zoomSpeed), minZoom, maxZoom);
    }
}
