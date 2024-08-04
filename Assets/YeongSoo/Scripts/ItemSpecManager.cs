using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemSpecManager
{
    public static List<ItemSpec> itemSpecList = new List<ItemSpec>();

    public static async Task LoadItemSpecs()
    {
        try
        {
            var task = await GoogleSheetLoader.LoadItemSpecsFromGoogleSheet();

            if (!task.success) return;
            
            itemSpecList = task.itemSpecList;
            Debug.Log("������ ���� �ε带 �����߽��ϴ�!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
        }
    }
}
