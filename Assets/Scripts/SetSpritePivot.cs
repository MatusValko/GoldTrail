#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SetSpritePivot : EditorWindow
{
    private Vector2 newPivot = new Vector2(0.5f, 0f); // Center Bottom Pivot

    [MenuItem("Tools/Set Pivot for Selected Sprites")]
    static void ShowWindow()
    {
        GetWindow<SetSpritePivot>("Set Sprite Pivot");
    }

    void OnGUI()
    {
        GUILayout.Label("New Pivot Position", EditorStyles.boldLabel);
        newPivot = EditorGUILayout.Vector2Field("Pivot", newPivot);

        if (GUILayout.Button("Apply to Selected Sprites"))
        {
            ApplyPivot();
        }
    }

    void ApplyPivot()
    {
        foreach (Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType == TextureImporterType.Sprite)
            {
                importer.isReadable = true; // Enable texture editing
                importer.spritePivot = newPivot;
                // importer.spritePivot = PivotMode; // Center Bottom Pivot
                DebugLogger.Log($"Pivot set to {newPivot} for {obj.name}.");
                importer.SaveAndReimport();
            }
        }
        // Debug.Log($"Pivot set to {newPivot} for all selected sprites.");
    }
}
#endif