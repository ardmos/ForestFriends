using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ItemIDValidator : EditorWindow
{
    [MenuItem("Tools/Validate Item IDs")]
    public static void ShowWindow()
    {
        GetWindow<ItemIDValidator>("Item ID Validator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Validate All Item IDs"))
        {
            ValidateAllItemIDs();
        }
    }

    private void ValidateAllItemIDs()
    {
        // ��� ItemData ������ �˻�
        string[] guids = AssetDatabase.FindAssets("t:ItemData");
        Dictionary<int, ItemData> idMap = new Dictionary<int, ItemData>();
        bool hasConflicts = false;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);

            // ID �ߺ� ���� Ȯ��
            if (idMap.ContainsKey(item.ID))
            {
                Debug.LogError($"ID conflict: {item.name} and {idMap[item.ID].name} both have ID {item.ID}");
                hasConflicts = true;
            }
            else
            {
                idMap.Add(item.ID, item);
            }
        }

        if (!hasConflicts)
        {
            Debug.Log("No ID conflicts found.");
        }
    }
}