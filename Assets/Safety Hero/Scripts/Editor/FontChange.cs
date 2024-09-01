using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FontChange : EditorWindow
{
    private Font targetFont;  // ¹Ù²Ü ÆùÆ®

    [MenuItem("Tools/Change All Text Fonts")]
    public static void ShowWindow()
    {
        GetWindow<FontChange>("Change All Text Fonts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select the Font to Apply", EditorStyles.boldLabel);
        targetFont = (Font)EditorGUILayout.ObjectField("Font", targetFont, typeof(Font), false);

        if (GUILayout.Button("Apply Font"))
        {
            ChangeFonts();
        }
    }

    private void ChangeFonts()
    {
        if (targetFont == null)
        {
            Debug.LogWarning("No font selected. Please select a font to apply.");
            return;
        }

        Text[] allUIText = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text text in allUIText)
        {
            if (text != null && text.font != targetFont)
            {
                Undo.RecordObject(text, "Change Font");
                text.font = targetFont;
                EditorUtility.SetDirty(text);
            }
        }

        Debug.Log("Fonts have been updated successfully.");
    }
}
