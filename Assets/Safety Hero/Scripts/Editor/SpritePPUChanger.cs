using UnityEngine;
using UnityEditor;

public class SpritePPUChanger : EditorWindow
{
    [MenuItem("Tools/Change Pixels Per Unit")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpritePPUChanger));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Change Pixels Per Unit to 18"))
        {
            ChangePPU();
        }
    }

    private static void ChangePPU()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.spritePixelsPerUnit = 18;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        Debug.Log("Changed Pixels Per Unit to 18 for all sprites.");
    }
}
