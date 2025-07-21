// Option 1: Simple on-screen debug text (easiest)
using UnityEngine;
using UnityEngine.UI;

public class MobileDebugger : MonoBehaviour
{
    public static Text debugText; // Assign a UI Text component in inspector
    private static string debugLog = "";

    void Start()
    {
        debugText = GetComponent<Text>();
        // Create debug text if not assigned
        if (debugText == null)
        {
            GameObject canvas = new GameObject("DebugCanvas2");
            Canvas c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            GameObject textGO = new GameObject("DebugText");
            textGO.transform.SetParent(canvas.transform);
            debugText = textGO.AddComponent<Text>();
            debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            debugText.fontSize = 24;
            debugText.color = Color.white;

            RectTransform rt = debugText.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(10, -200);
            rt.offsetMax = new Vector2(-10, -10);
        }
    }

    public static void LogDebug(string message)
    {
        debugLog += message + "\n";

        // Keep only last 10 lines
        string[] lines = debugLog.Split('\n');
        if (lines.Length > 20)
        {
            debugLog = string.Join("\n", lines, lines.Length - 10, 10);
        }

        if (debugText != null)
            debugText.text = debugLog;
    }
}