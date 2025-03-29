using System.Diagnostics;
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

    [SerializeField] private TileManager tileManager;
    private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;


    public GameObject TileWindow;

    // public GameObject highlightPrefab;   // Assign a highlight sprite in Unity
    // public GameObject infoPanel;         // Assign a UI Panel
    // public TextMeshProUGUI tileInfoText; // Assign a TextMeshPro element

    private Vector3 touchStartPos;
    private bool isDragging = false;
    private float dragThreshold = 10f; // Minimum movement (in pixels) to detect dragging

    void Start()
    {

        if (cam == null)
        {
            cam = Camera.main;
        }
        tilemap = TileManager.instance.tilemap;
        tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();


        SetCameraZoomSpeed(3);
    }

    void Update()
    {
        HandlePanning();
        HandleZooming();
    }

    [Conditional("UNITY_EDITOR")]
    public void SetCameraZoomSpeed(float speed)
    {
        zoomSpeed = speed;
    }

    private void _setStartingTileMode()
    {
        tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
    }



    void HandlePanning()
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
                TileManager.instance.SelectTile(tilePos);
            }
            else
            {
                DebugLogger.Log($"Clicked on empty tile at {tilePos}");
            }
        }
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
        //if size is greater than 20
        if (cam.orthographicSize > 25)
        {
            tilemapRenderer.mode = TilemapRenderer.Mode.SRPBatch;
        }
        else
        {
            tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
        }
    }
}
