using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemSpecManager
{
    private static Dictionary<int, ItemSpec> itemSpecDictionary = new Dictionary<int, ItemSpec>();

    public static async Task LoadItemSpecs()
    {
        try
        {
            var task = await GoogleSheetLoader.LoadItemSpecsFromGoogleSheet();

            if (!task.success) return;

            itemSpecDictionary = task.itemSpecDictionary;
            Debug.Log("������ ���� ������ �ٿ�ε带 �����߽��ϴ�!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
        }
    }

    public static ItemSpec GetItemSpecBySpecID(int itemSpecID)
    {
        if (itemSpecDictionary.TryGetValue(itemSpecID, out ItemSpec itemSpec))
        {
            return itemSpec;
        }
        Debug.LogWarning($"ItemSpec with ItemSpecID {itemSpecID} not found.");
        return null;
    }
}
