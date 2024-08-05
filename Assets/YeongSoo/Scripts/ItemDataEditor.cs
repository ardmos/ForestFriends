using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    private static List<int> usedIDs = new List<int>(); // 사용된 ID 목록

    private void OnEnable()
    {
        ItemData item = (ItemData)target;
        // 아이템이 처음 생성되었을 때 고유 ID 할당
        if (item.ID == 0)
        {
            AssignUniqueID(item);
        }
        else if (!usedIDs.Contains(item.ID))
        {
            usedIDs.Add(item.ID);
        }
    }

    public override void OnInspectorGUI()
    {
        ItemData item = (ItemData)target;

        EditorGUI.BeginChangeCheck();
        int newID = EditorGUILayout.IntField("ID", item.ID);
        if (EditorGUI.EndChangeCheck())
        {
            // 새로운 ID가 중복되지 않으면 ID 변경
            if (newID != item.ID && !usedIDs.Contains(newID))
            {
                usedIDs.Remove(item.ID);
                item.SetID(newID);
                usedIDs.Add(newID);
            }
            else if (usedIDs.Contains(newID))
            {
                EditorUtility.DisplayDialog("Invalid ID", "This ID is already in use.", "OK");
            }
        }

        DrawDefaultInspector();
    }

    private void AssignUniqueID(ItemData item)
    {
        int newID = 1;
        // 사용되지 않은 ID를 찾을 때까지 증가
        while (usedIDs.Contains(newID))
        {
            newID++;
        }
        item.SetID(newID);
        usedIDs.Add(newID);
    }
}