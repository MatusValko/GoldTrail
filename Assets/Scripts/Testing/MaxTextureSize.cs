using TMPro;
using UnityEngine;

public class MaxTextureSize : MonoBehaviour
{
    public TextMeshProUGUI MaxTextureSizeText;

    void Start()
    {
        int maxSize = SystemInfo.maxTextureSize;
        MaxTextureSizeText.text = $"Max texture size: <b>{maxSize}</b>";
        DebugLogger.Log($"Max supported texture size: {maxSize}");
    }
}
