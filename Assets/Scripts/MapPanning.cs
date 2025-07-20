using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapPanningZooming : MonoBehaviour
{


    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;
    // public GameObject TileWindow;

    // private float dragThreshold = 10f; // Minimum movement (in pixels) to detect dragging

    [Header("Camera Settings")]
    [SerializeField] private Camera _camera;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    private bool _isZooming = false;
    [SerializeField] private float _smoothSpeed = 0.1f; // Higher = faster zooming, lower = smoother
    [SerializeField] private float _moveThreshold = 10f; // pixels
    [SerializeField] private float _clickTimeThreshold = 0.2f; // seconds
    private Vector3 dragOrigin;

    private Vector3 _touchStartPos;
    private float _startTime;
    private bool _isDragging = false;

    [SerializeField] private float minXClamp;
    [SerializeField] private float maxXClamp;
    [SerializeField] private float minYClamp;
    [SerializeField] private float maxYClamp;

    //header
    [Header("Tilemap Outline")]
    // private TilemapOutline tilemapOutline;
    private int gridSizeX;
    private int gridSizeY;
    private float tileSizeX; // Adjust for correct scale
    private float tileSizeY; // Adjust for correct scale
    private float cellGapX; // Adjust for correct scale
    private float cellGapY; // Adjust for correct scale
    [SerializeField] private LineRenderer lineRenderer;
    // [SerializeField] private Grid grid;
    private BoundsInt bounds;

    void Start()
    {

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _getTilemapData();
        _makeLine();
        _setCameraBounds();
        // tilemap = TileManager.Instance.tilemap;


        // SetCameraZoomSpeed(4);
        _setStartingTileMode();

    }
    private void OnValidate()
    {
        //draw the outline
        _getTilemapData();
        _makeLine();
    }
    private void _getTilemapData()
    {
        tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
        // tilemapOutline = GetComponent<TilemapOutline>();
        bounds = tilemap.cellBounds;
        gridSizeX = bounds.size.x;
        gridSizeY = bounds.size.y;
        tileSizeX = tilemap.cellSize.x; // Assuming square tiles, use x for size
        tileSizeY = tilemap.cellSize.y; // Use y for isometric height adjustment
        cellGapX = tilemap.cellGap.x;
        cellGapY = tilemap.cellGap.y;
    }

    private void _setCameraBounds()
    {
        float effectiveSizeX = tileSizeX + cellGapX;
        float effectiveSizeY = tileSizeY + cellGapY;

        minXClamp = -((gridSizeX / 2) * effectiveSizeX);
        maxXClamp = (gridSizeX / 2) * effectiveSizeX;
        minYClamp = -(gridSizeY / 2 * effectiveSizeY);
        maxYClamp = (gridSizeY / 2 * effectiveSizeY);
        DebugLogger.Log($"Clamping camera to X: [{minXClamp}, {maxXClamp}], Y: [{minYClamp}, {maxYClamp}]");
    }

    void Update()
    {
        _handleInput();
    }

    private void _setStartingTileMode()
    {
        tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
    }


    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    private void SelectTile(Vector3 mouseWorldPos)
    {

        // // If clicked on UI, do nothing, use raycast to check if pointer is over UI
        // PointerEventData pointerData = new PointerEventData(EventSystem.current)
        // {
        //     position = Input.mousePosition
        // };
        // var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        // EventSystem.current.RaycastAll(pointerData, raycastResults);
        // if (raycastResults.Count > 0)
        // {
        //     // DebugLogger.Log("Pointer is over a UI element");
        //     //Debug log raycast results
        //     foreach (var result in raycastResults)
        //     {
        //         DebugLogger.Log($"Raycast hit: {result.gameObject.name} at position {result.worldPosition}");
        //     }
        //     // Pointer is over a UI element
        //     return;
        // }


        // Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
        TileBase clickedTile = tilemap.GetTile(tilePos);

        if (clickedTile != null)
        {
            DebugLogger.Log($"Clicked on tile at {tilePos}, Tile: {clickedTile.name}");
            TileManager.Instance.SelectTile(tilePos);
        }
        else
        {
            DebugLogger.Log($"Clicked on empty tile at {tilePos}");
        }
    }


    private void _handleInput()
    {
        // if (IsPointerOverUI()) return;
        // if (IsZooming) return;

        // if (Input.GetMouseButtonDown(0)) // Mouse click/touch start
        // {
        //     // if (IsPointerOverUI()) return;
        //     touchStartPos = Input.mousePosition;
        //     isDragging = false;
        //     dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        // }

        // if (Input.GetMouseButton(0)) // Mouse drag/touch move
        // {
        //     if (Vector3.Distance(touchStartPos, Input.mousePosition) > dragThreshold)
        //     {
        //         Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
        //         cam.transform.position += difference;
        //         isDragging = true;
        //     }
        // }

        // if (Input.GetMouseButtonUp(0) && !isDragging) // Mouse release, only select if not dragging
        // {
        //     // if (IsPointerOverUI()) return;
        //     SelectTile();
        //     // TileSelector ts= GetComponent<TileSelector>();
        // }


        //THIS IS USED
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            _isZooming = false; // Reset zooming state for single touch
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = touch.position;
                    _startTime = Time.time;
                    _isDragging = false;
                    // _isZooming = false;
                    dragOrigin = _camera.ScreenToWorldPoint(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (_isZooming)
                    {
                        dragOrigin = _camera.ScreenToWorldPoint(touch.position);
                        _isZooming = false;
                        _isDragging = true;
                    }

                    if (Vector2.Distance(touch.position, _touchStartPos) > _moveThreshold)
                    {
                        Vector3 difference = dragOrigin - _camera.ScreenToWorldPoint(touch.position);
                        _camera.transform.position += difference;
                        ClampCamera();
                    }
                    break;
                case TouchPhase.Ended:
                    float duration = Time.time - _startTime;
                    if (!_isDragging && duration < _clickTimeThreshold)
                    {
                        Vector2 worldPos = _camera.ScreenToWorldPoint(touch.position);
                        //CLICK ON TILE
                        SelectTile(worldPos);
                    }
                    break;
            }
        }
        else if (Input.touchCount == 2) // Mobile pinch zoom //TODO TRY >=2
        {
            _isZooming = true;
            _isDragging = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;
            float difference = prevMagnitude - currentMagnitude;

            // Vector2 midpoint = (touch0.position + touch1.position) / 2;

            // // World position before zoom
            // Vector3 worldBeforeZoom = _camera.ScreenToWorldPoint(midpoint);

            if (Mathf.Abs(difference) > 1f) // Threshold in pixels
            {
                Zoom(difference * zoomSpeed);
            }
            // Vector3 worldAfterZoom = _camera.ScreenToWorldPoint(midpoint);

            // Vector3 diff = worldBeforeZoom - worldAfterZoom;
            // _camera.transform.position += diff;
        }
        else if (Input.touchCount == 0)
        {
            _isZooming = false; // Reset when no fingers are touching
        }
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            DebugLogger.Log("BUTTON DOWN");
            _touchStartPos = Input.mousePosition;
            _startTime = Time.time;
            _isDragging = false;
            dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            _isDragging = true;

            if (Vector2.Distance((Vector2)Input.mousePosition, _touchStartPos) > _moveThreshold)
            {
                DebugLogger.Log("DRAGGING");
                Vector3 difference = dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                _camera.transform.position += difference;
                ClampCamera();
                // ClampCameraIsometric();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DebugLogger.Log("BUTTON UP");

            float duration = Time.time - _startTime;
            if (!_isDragging && duration < _clickTimeThreshold)
            {
                DebugLogger.Log("CLICK");
                Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                SelectTile(worldPos);
            }
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // PC scroll zoom
        if (scroll != 0)
        {
            _zoomPC(-scroll); // Invert scroll direction for zooming
        }
#endif
    }

    void ClampCamera()
    {
        float clamped2X = Mathf.Clamp(_camera.transform.position.x, minXClamp, maxXClamp);
        float clamped2Y = Mathf.Clamp(_camera.transform.position.y, minYClamp, maxYClamp);
        _camera.transform.position = new Vector3(clamped2X, clamped2Y, -10f); // lock z = -10
    }

    private float zoomVelocity = 0f;
    void Zoom(float increment)
    {
        // int fasterWhenFarther = (int)_camera.orthographicSize / 10;
        // increment = increment > 0 ? increment + fasterWhenFarther : increment - fasterWhenFarther;
        // float currentSize = _camera.orthographicSize;
        // float targeSize = Mathf.Clamp(_camera.orthographicSize + (increment * zoomSpeed), minZoom, maxZoom);
        // float smoothScale = Mathf.SmoothDamp(currentSize, targeSize, ref zoomVelocity, _smoothSpeed);

        // // float zoomFactor = cam.orthographicSize * 0.5f * ; // Scale zoom speed based on current zoom
        // // cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + (increment * zoomSpeed), minZoom, maxZoom);
        // _camera.orthographicSize = smoothScale;

        float scaleFactor = _camera.orthographicSize / 10f; // optional, tweak multiplier
        float baseSpeed = zoomSpeed * scaleFactor;

        float currentSize = _camera.orthographicSize;
        float targetSize = Mathf.Clamp(currentSize + increment * baseSpeed, minZoom, maxZoom);

        _camera.orthographicSize = Mathf.SmoothDamp(currentSize, targetSize, ref zoomVelocity, _smoothSpeed);

        _optimizeTilemap();
    }

    private void _zoomPC(float increment)
    {
        int fasterWhenFarther = (int)_camera.orthographicSize / 10;
        increment = increment > 0 ? increment + fasterWhenFarther : increment - fasterWhenFarther;
        // float zoomFactor = cam.orthographicSize * 0.5f * ; // Scale zoom speed based on current zoom
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + (increment * zoomSpeed), minZoom, maxZoom);
        //if size is greater than 20
        _optimizeTilemap();

    }

    private void _optimizeTilemap()
    {
        if (_camera.orthographicSize >= 20)
        {
            tilemapRenderer.mode = TilemapRenderer.Mode.SRPBatch;
        }
        else
        {
            tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
        }
        float width = 0.5f * (_camera.orthographicSize / 100);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }


    #region  "Tilemap Outline"
    private void _makeLine()
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

    public void SetLineWidth(float width)
    {
        //TODO make it to 3* the default value at max, 0.5f * (zoom/100)

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    private void UpdateOutline()
    {
        Vector3[] corners = new Vector3[5];

        corners[0] = IsoPosition(-gridSizeX / 2, -gridSizeY / 2);
        corners[1] = IsoPosition(gridSizeX / 2, -gridSizeY / 2);
        corners[2] = IsoPosition(gridSizeX / 2, gridSizeY / 2);
        corners[3] = IsoPosition(-gridSizeX / 2, gridSizeY / 2);
        corners[4] = corners[0]; // Close loop

        lineRenderer.SetPositions(corners);
    }

    private Vector3 IsoPosition(int x, int y)
    {
        float effectiveSizeX = tileSizeX + cellGapX;
        float effectiveSizeY = tileSizeY + cellGapY;
        float isoX = (x - y) * (effectiveSizeX / 2);  // Half-width for isometric
        float isoY = (x + y) * (effectiveSizeY / 2); // Adjusted height (0.58 / 2)
        // DebugLogger.Log($"IsoPosition: x={x}, y={y}, isoX={isoX} isoY={isoY}");
        return new Vector3(isoX, isoY, 0);
    }
    #endregion
}
