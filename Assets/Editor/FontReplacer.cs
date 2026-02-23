using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ReplaceTMPFontAssetUtility : EditorWindow
{
    private TMP_FontAsset newFontAsset;

    [MenuItem("Tools/TMP/Replace All Font Assets")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceTMPFontAssetUtility>("Replace TMP Fonts");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Replace All TMP Font Assets", EditorStyles.boldLabel);
        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "New Font Asset",
            newFontAsset,
            typeof(TMP_FontAsset),
            false);

        if (GUILayout.Button("Replace Fonts in Open Scenes"))
        {
            if (newFontAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a TMP Font Asset.", "OK");
                return;
            }

            ReplaceFontsInScenes();
        }
    }

    private void ReplaceFontsInScenes()
    {
        int count = 0;

        for (int i = 0; i < EditorSceneManager.sceneCount; i++) 
        {
            var scene = EditorSceneManager.GetSceneAt(i);

            if (!scene.isLoaded) continue;

            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                var texts = root.GetComponentsInChildren<TMP_Text>(true);
                foreach (var text in texts)
                {
                    if (text.font != newFontAsset)
                    {
                        text.font = newFontAsset;
                        EditorUtility.SetDirty(text);
                        count++;
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"TMP Font replacement complete. Updated {count} components.");
    }
}
