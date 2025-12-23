// Assets/Editor/SelectionUtils.cs
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SelectionUtils
{
    // 현재 선택 중에서 "부모가 선택되어 있지 않은 오브젝트"만 남김 (자식 선택 자동 제거)
    [MenuItem("Tools/Selection/Keep Top-Level Only")]
    public static void KeepTopLevelOnly()
    {
        var sel = Selection.transforms;
        var top = sel.Where(t => t.parent == null || !sel.Contains(t.parent)).Select(t => t.gameObject).ToArray();
        Selection.objects = top;
        Debug.Log($"Top-level only: {Selection.objects.Length} objects");
    }

    // 현재 선택들을 이름이 "Walls"인 오브젝트의 자식으로 이동
    [MenuItem("Tools/Selection/Set Parent To \"Walls\"")]
    public static void SetParentToWalls()
    {
        var walls = GameObject.Find("Walls");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Walls\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Walls");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Walls");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Walls");
    }

    // 현재 선택들을 이름이 "Walls"인 오브젝트의 자식으로 이동
    [MenuItem("Tools/Selection/Set Parent To \"Jambs\"")]
    public static void SetParentToJambs()
    {
        var walls = GameObject.Find("Jambs");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Jambs\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Jambs");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Jambs");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Walls");
    }

    // 현재 선택들을 이름이 "Walls"인 오브젝트의 자식으로 이동
    [MenuItem("Tools/Selection/Set Parent To \"Floors\"")]
    public static void SetParentToFloors()
    {
        var walls = GameObject.Find("Floors");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Floors\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Floors");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Floors");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Floors");
    }

    // 현재 선택들을 이름이 "Walls"인 오브젝트의 자식으로 이동
    [MenuItem("Tools/Selection/Set Parent To \"Windows\"")]
    public static void SetParentToWindows()
    {
        var walls = GameObject.Find("Windows");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Windows\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Windows");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Windows");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Windows");
    }

    // 현재 선택들을 이름이 "Walls"인 오브젝트의 자식으로 이동
    [MenuItem("Tools/Selection/Set Parent To \"Desks\"")]
    public static void SetParentToDesks()
    {
        var walls = GameObject.Find("Desks");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Desks\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Desks");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Desks");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Desks");
    }

    [MenuItem("Tools/Selection/Set Parent To \"Doors\"")]
    public static void SetParentToDoors()
    {
        var walls = GameObject.Find("Doors");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Doors\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Doors");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Doors");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Doors");
    }

    [MenuItem("Tools/Selection/Set Parent To \"Bottles\"")]
    public static void SetParentToBottles()
    {
        var walls = GameObject.Find("Bottles");
        if (!walls)
        {
            EditorUtility.DisplayDialog("Set Parent", "\"Bottles\" 오브젝트를 찾을 수 없습니다.", "OK");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(walls, "Reparent To Bottles");

        foreach (var t in Selection.transforms)
        {
            // 순환 방지: Walls가 선택된 대상의 자손이면 스킵
            if (walls.transform == t || walls.transform.IsChildOf(t)) continue;

            Undo.SetTransformParent(t, walls.transform, "Reparent To Bottles");
            t.SetAsLastSibling();
        }

        Debug.Log($"Moved {Selection.transforms.Length} objects under Bottles");
    }
}
