using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Menu: Tools > Casino > Build Card Grid
// Creates a CardGrid inside CasinoFloor with a Grid Layout Group (3 columns)
// and moves every DealerCard* object into it. Safe to run once; it stops if CardGrid exists.
public static class BuildCardGrid
{
    [MenuItem("Tools/Casino/Build Card Grid")]
    static void Build()
    {
        GameObject floor = GameObject.Find("CasinoFloor");
        if (floor == null) { Debug.LogError("CardGrid: no object named CasinoFloor in the open scene"); return; }
        if (floor.transform.Find("CardGrid") != null) { Debug.Log("CardGrid: already exists, nothing done"); return; }

        // Collect the cards first, because moving them changes the child list.
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in floor.transform)
            if (child.name.StartsWith("DealerCard")) cards.Add(child);

        GameObject go = new GameObject("CardGrid", typeof(RectTransform), typeof(GridLayoutGroup));
        Undo.RegisterCreatedObjectUndo(go, "Build Card Grid");

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(floor.transform, false);
        rt.anchorMin = Vector2.zero;            // stretch to fill CasinoFloor
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(0, 10);      // 10 up from the bottom
        rt.offsetMax = new Vector2(0, -70);     // 70 down from the top, clear of the hire buttons

        GridLayoutGroup grid = go.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(150, 109);
        grid.spacing = new Vector2(10, 10);
        grid.padding = new RectOffset(15, 15, 0, 0);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;

        foreach (Transform card in cards)
            Undo.SetTransformParent(card, rt, "Move card into CardGrid");

        EditorSceneManager.MarkSceneDirty(floor.scene);
        Debug.Log("CardGrid: created and moved " + cards.Count + " cards into it. Press Cmd+S to save.");
    }
}
