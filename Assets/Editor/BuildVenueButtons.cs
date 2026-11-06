using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Menu: Tools > Casino > Build Venue Buttons
// Builds the four venue buttons and the Start Day button inside VenueStrip.
// Safe to run once; it stops if StartDayButton already exists.
public static class BuildVenueButtons
{
    const string SpritePath = "Assets/Sprites/RoundedBox.png";
    const string FontPath = "Assets/Fonts/Excalifont SDF.asset";

    [MenuItem("Tools/Casino/Build Venue Buttons")]
    static void Build()
    {
        GameObject strip = GameObject.Find("VenueStrip");
        if (strip == null) { Debug.LogError("Venues: no object named VenueStrip in the open scene"); return; }
        if (strip.transform.Find("StartDayButton") != null) { Debug.Log("Venues: already built, nothing done"); return; }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (sprite == null) Debug.LogWarning("Venues: RoundedBox sprite not found, buttons will be plain squares");

        // anchor boxes are fractions of the strip: minX, minY, maxX, maxY
        Make(strip, "TonyGameButton", "Tony Private Game - $300\n45 points go to Tony", "#FFE98E", 14, sprite, font, 0.015f, 0.52f, 0.345f, 0.94f);
        Make(strip, "AlleyButton", "Run Games in Alley $0", "#FFE98E", 14, sprite, font, 0.355f, 0.52f, 0.685f, 0.94f);
        Make(strip, "KnightsHallButton", "rent knights of columbus hall - $1000/day", "#FFE98E", 14, sprite, font, 0.015f, 0.06f, 0.345f, 0.48f);
        Make(strip, "BasementButton", "\"Grandma's\" Basement - $100", "#FFE98E", 14, sprite, font, 0.355f, 0.06f, 0.685f, 0.48f);
        Make(strip, "StartDayButton", "Start Day", "#A9F0B3", 28, sprite, font, 0.705f, 0.06f, 0.985f, 0.94f);

        EditorSceneManager.MarkSceneDirty(strip.scene);
        Debug.Log("Venues: built 5 buttons. Press Cmd+S to save.");
    }

    static void Make(GameObject parent, string name, string label, string hex, float size,
                     Sprite sprite, TMP_FontAsset font, float x0, float y0, float x1, float y1)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        Undo.RegisterCreatedObjectUndo(go, "Build Venue Buttons");
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent.transform, false);
        rt.anchorMin = new Vector2(x0, y0);
        rt.anchorMax = new Vector2(x1, y1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        Color c; ColorUtility.TryParseHtmlString(hex, out c);
        img.color = c;
        go.GetComponent<Button>().targetGraphic = img;

        GameObject t = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform trt = t.GetComponent<RectTransform>();
        trt.SetParent(rt, false);
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(6, 2);
        trt.offsetMax = new Vector2(-6, -2);

        TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
        if (font != null) tmp.font = font;
        tmp.text = label;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color32(28, 28, 28, 255);
        tmp.raycastTarget = false;
    }
}
