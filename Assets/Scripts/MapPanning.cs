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

    [SerializeField] private float _prevTouchDeltaMag = 0f;
    private Vector3 dragOrigin;

    private Vector3 _touchStartPos;
    private float _startTime;
    private bool _isDragging = false;
    private bool _pinchStarted = false;
    private float lastPinchDistance = 0f;

    [SerializeField] private float minXClamp;
    [SerializeField] private float maxXClamp;
    [SerializeField] private float minYClamp;
    [SerializeField] private float maxYClamp;

    [SerializeField] private float effectiveSizeX;
    [SerializeField] private float effectiveSizeY;




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
        effectiveSizeX = tileSizeX + cellGapX;
        effectiveSizeY = tileSizeY + cellGapY;

        minXClamp = -((gridSizeX / 2) * effectiveSizeX);
        maxXClamp = (gridSizeX / 2) * effectiveSizeX;
        minYClamp = -(gridSizeY / 2 * effectiveSizeY);
        maxYClamp = (gridSizeY / 2 * effectiveSizeY);
        // DebugLogger.Log($"Clamping camera to X: [{minXClamp}, {maxXClamp}], Y: [{minYClamp}, {maxYClamp}]");
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
        //THIS IS USED
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            // _isZooming = false; // Reset zooming state for single touch
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = touch.position;
                    _startTime = Time.time;
                    _isDragging = false;
                    // dragOrigin = _camera.ScreenToWorldPoint(touch.position);
                    //from claude
                    if (!_isZooming)
                    {
                        dragOrigin = _camera.ScreenToWorldPoint(touch.position);
                    }
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
                        ClampCameraToDiamond();
                        _isDragging = true; //claude
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
                    _isDragging = false;//claude
                    _isZooming = false; //claude
                    break;
            }
        }
        else if (Input.touchCount == 2) // Mobile pinch zoom //TODO TRY >=2
        {
            _isDragging = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Calculate current distance between fingers
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            if (!_isZooming)
            {
                // First frame of pinch - just record the distance, don't zoom
                _isZooming = true;
                lastPinchDistance = currentDistance;
                DebugLogger.Log($"Pinch started - distance: {currentDistance}");
            }
            else
            {
                // Calculate how much the distance changed
                float deltaDistance = currentDistance - lastPinchDistance;
                lastPinchDistance = currentDistance;

                DebugLogger.Log($"Delta: {deltaDistance}, Current: {currentDistance}");

                // Only zoom if change is significant
                float currentSize = _camera.orthographicSize;
                float zoomSpeedMultiplier = currentSize / 10f; // Adjust divisor as needed

                // Positive deltaDistance = fingers moving apart = zoom out
                // Negative deltaDistance = fingers moving together = zoom in
                float zoomAmount = -deltaDistance * 0.01f * zoomSpeedMultiplier;


                float newSize = Mathf.Clamp(currentSize + zoomAmount, minZoom, maxZoom);
                _camera.orthographicSize = newSize;

                _optimizeTilemap();
            }



            // _isZooming = true;
            // _isDragging = false;

            // Touch touch0 = Input.GetTouch(0);
            // Touch touch1 = Input.GetTouch(1);
            // Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            // Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            // float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            // float currentMagnitude = (touch0.position - touch1.position).magnitude;
            // float deltaMag = _prevTouchDeltaMag - currentMagnitude;
            // _prevTouchDeltaMag = currentMagnitude;

            // if (Mathf.Abs(deltaMag) > 1f) // Threshold in pixels
            // {
            //     Zoom(deltaMag * zoomSpeed);
            // }
        }
        else if (Input.touchCount < 2 && _isZooming)
        {
            // End pinch gesture
            _isZooming = false;
            lastPinchDistance = 0f;
            DebugLogger.Log("Pinch ended");
        }
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            // DebugLogger.Log("BUTTON DOWN");
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
                // DebugLogger.Log("DRAGGING");
                Vector3 difference = dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                _camera.transform.position += difference;
                ClampCameraToDiamond();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // DebugLogger.Log("BUTTON UP");

            float duration = Time.time - _startTime;
            if (!_isDragging && duration < _clickTimeThreshold)
            {
                // DebugLogger.Log("CLICK");
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

    public void ClampCameraToDiamond()
    {
        Vector3 pos = _camera.transform.position;

        // Clamp to rectangle first (safe fallback)
        pos.x = Mathf.Clamp(pos.x, minXClamp, maxXClamp);
        pos.y = Mathf.Clamp(pos.y, minYClamp, maxYClamp);

        // Now calculate diagonal (diamond) limits
        float halfWidth = (maxXClamp - minXClamp) / 2f;
        float halfHeight = (maxYClamp - minYClamp) / 2f;
        Vector2 center = new Vector2((minXClamp + maxXClamp) / 2f, (minYClamp + maxYClamp) / 2f);

        // Convert position to relative
        float dx = pos.x - center.x;
        float dy = pos.y - center.y;

        // Check if outside diamond bounds
        if (Mathf.Abs(dx / halfWidth) + Mathf.Abs(dy / halfHeight) > 1f)
        {
            float t = 1f / (Mathf.Abs(dx / halfWidth) + Mathf.Abs(dy / halfHeight));
            dx *= t;
            dy *= t;
        }

        // Set final clamped position
        _camera.transform.position = new Vector3(center.x + dx, center.y + dy, -10f);
    }

    private float zoomVelocity = 0f;
    void Zoom(float increment)
    {
        float currentSize = _camera.orthographicSize;
        float zoomFactor = 1f + (increment * zoomSpeed * 0.02f); // Convert to percentage
        float targetSize = Mathf.Clamp(currentSize * zoomFactor, minZoom, maxZoom);

        _camera.orthographicSize = Mathf.SmoothDamp(currentSize, targetSize, ref zoomVelocity, _smoothSpeed);

        // float scaleFactor = _camera.orthographicSize / 10f; // optional, tweak multiplier
        // float baseSpeed = zoomSpeed * scaleFactor;

        // float currentSize = _camera.orthographicSize;
        // float targetSize = Mathf.Clamp(currentSize + increment * baseSpeed, minZoom, maxZoom);

        // _camera.orthographicSize = Mathf.SmoothDamp(currentSize, targetSize, ref zoomVelocity, _smoothSpeed);

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
        float width = 0.5f * (_camera.orthographicSize / 35);
        SetLineWidth(width);
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
        lineRenderer.sortingOrder = -50;

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
