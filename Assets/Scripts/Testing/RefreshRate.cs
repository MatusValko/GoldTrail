using TMPro;
using UnityEngine;

public class RefreshRate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI RefreshText;
    public TextMeshProUGUI CameraSizeText;

    void Start()
    {
        DebugLogger.Log("Device refresh rate: " + Screen.currentResolution.refreshRateRatio.value);
        RefreshText.text = "Device Refresh Rate: " + Screen.currentResolution.refreshRateRatio.value;
    }

    // Update is called once per frame
    void Update()
    {
        CameraSizeText.text = "Camera Size: " + Mathf.Round(Camera.main.orthographicSize * 100f) / 100f; ;

    }
}
