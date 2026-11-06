using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Menu: Tools > Casino > Apply Excalifont
// 1. Builds a TextMesh Pro font asset from Assets/Fonts/Excalifont-Regular.ttf (once).
// 2. Makes it the default font for any new text.
// 3. Swaps it onto every TMP text in the scenes and prefabs listed below, then saves them.
public static class ExcalifontSetup
{
    const string FontPath = "Assets/Fonts/Excalifont-Regular.ttf";
    const string AssetPath = "Assets/Fonts/Excalifont SDF.asset";

    // Add "Assets/Scenes/SceneTwo.unity" here if you also want your class scene changed.
    static readonly string[] Scenes =
    {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/CasinoOpen.unity",
        "Assets/Scenes/CasinoClosed.unity",
        "Assets/Scenes/GamblingMinigame.unity",
    };

    static readonly string[] Prefabs =
    {
        "Assets/Dealer.prefab",
    };

    [MenuItem("Tools/Casino/Apply Excalifont")]
    static void ApplyExcalifont()
    {
        // Opening scenes below would discard unsaved work, so ask first.
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        TMP_FontAsset fontAsset = GetOrCreateFontAsset();
        if (fontAsset == null) return;

        SetDefaultFont(fontAsset);

        string startScene = SceneManager.GetActiveScene().path;
        int total = 0;

        foreach (string path in Scenes)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            int count = 0;
            TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TMP_Text t in texts) count += ApplyTo(t, fontAsset);
            if (count > 0) EditorSceneManager.SaveScene(scene);
            Debug.Log("Excalifont: " + path + " changed " + count + " text objects");
            total += count;
        }

        foreach (string path in Prefabs)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            int count = 0;
            foreach (TMP_Text t in root.GetComponentsInChildren<TMP_Text>(true)) count += ApplyTo(t, fontAsset);
            if (count > 0) PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("Excalifont: " + path + " changed " + count + " text objects");
            total += count;
        }

        AssetDatabase.SaveAssets();
        if (!string.IsNullOrEmpty(startScene)) EditorSceneManager.OpenScene(startScene);
        Debug.Log("Excalifont: done, " + total + " text objects changed in total");
    }

    static int ApplyTo(TMP_Text t, TMP_FontAsset fontAsset)
    {
        // Text inside a prefab instance picks the font up from the prefab itself.
        if (PrefabUtility.IsPartOfPrefabInstance(t)) return 0;
        if (t.font == fontAsset) return 0;
        t.font = fontAsset;
        t.fontSharedMaterial = fontAsset.material;
        EditorUtility.SetDirty(t);
        return 1;
    }

    static TMP_FontAsset GetOrCreateFontAsset()
    {
        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetPath);
        if (existing != null) return existing;

        Font font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
        if (font == null)
        {
            Debug.LogError("Excalifont: could not find the font at " + FontPath);
            return null;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);
        if (fontAsset == null)
        {
            Debug.LogError("Excalifont: TextMesh Pro could not create a font asset from " + FontPath);
            return null;
        }

        // Pre-load the basic keyboard characters so the atlas is ready before first use.
        fontAsset.TryAddCharacters(AsciiCharacters());

        AssetDatabase.CreateAsset(fontAsset, AssetPath);
        fontAsset.material.name = "Excalifont SDF Material";
        AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        foreach (Texture2D atlas in fontAsset.atlasTextures)
        {
            atlas.name = "Excalifont SDF Atlas";
            AssetDatabase.AddObjectToAsset(atlas, fontAsset);
        }
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        return fontAsset;
    }

    static void SetDefaultFont(TMP_FontAsset fontAsset)
    {
        TMP_Settings settings = TMP_Settings.instance;
        if (settings == null) return;
        SerializedObject so = new SerializedObject(settings);
        SerializedProperty prop = so.FindProperty("m_defaultFontAsset");
        if (prop == null)
        {
            Debug.LogWarning("Excalifont: could not set the default font, set it in Project Settings > TextMesh Pro.");
            return;
        }
        prop.objectReferenceValue = fontAsset;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(settings);
    }

    static string AsciiCharacters()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (char c = ' '; c <= '~'; c++) sb.Append(c);
        return sb.ToString();
    }
}
