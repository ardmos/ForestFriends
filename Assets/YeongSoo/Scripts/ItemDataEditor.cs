using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    private static List<int> usedIDs = new List<int>(); // ���� ID ���

    private void OnEnable()
    {
        ItemData item = (ItemData)target;
        // �������� ó�� �����Ǿ��� �� ���� ID �Ҵ�
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
            // ���ο� ID�� �ߺ����� ������ ID ����
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
        // ������ ���� ID�� ã�� ������ ����
        while (usedIDs.Contains(newID))
        {
            newID++;
        }
        item.SetID(newID);
        usedIDs.Add(newID);
    }
}