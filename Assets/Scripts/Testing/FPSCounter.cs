using TMPro;
using UnityEngine;
using UnityEngine.UI; // Use TMPro if using TextMeshPro

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Assign a UI Text in the Inspector
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }
}
