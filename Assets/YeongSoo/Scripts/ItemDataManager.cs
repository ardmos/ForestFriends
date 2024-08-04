using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public static class ItemDataManager 
{
    public static readonly string jsonFilePath = "Assets/Resources/itemData.json";

    private static List<ItemData> itemDataList = new List<ItemData>();

    public static List<ItemData> LoadItemsFromJson()
    {
        Debug.Log($"ItemDataManager.LoadItemsFromJson()");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            itemDataList = new List<ItemData>(wrapper.items);

            Debug.Log($"JSON ������ ã�ҽ��ϴ�. ������ ������ ����Ʈ�� ��ȯ�մϴ�. {json}");

            foreach (ItemData wrapperItem in wrapper.items)
            {
                Debug.Log($"wrapperItem : {wrapperItem}");
            }

            foreach(ItemData itemData in itemDataList)
            {
                Debug.Log($"itemDataList : {itemData}");
            }

            return itemDataList;
        }
        else
        {
            Debug.LogError("JSON file not found!");

            return null;
        }
    }

    private static void SaveItemsToJson()
    {
        ItemDataWrapper wrapper = new ItemDataWrapper { items = itemDataList.ToArray() };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(jsonFilePath, json);
    }

    public static void AddItem(ItemData newItem)
    {
        itemDataList.Add(newItem);
        Debug.Log($"�������� �߰��߽��ϴ� : {newItem}");
        SaveItemsToJson();
    }

    public static void RemoveItem(ItemData item)
    {
        itemDataList.Remove(item);
        Debug.Log($"�������� �����߽��ϴ� : {item}");
        SaveItemsToJson();
    }

    public static void UpdateItemDataList(List<ItemData> newItemDataList)
    {
        Debug.Log($"1.�κ��丮 ���� ������. itemDataList : (");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.name} : {item.currentCellPos}");
        }
        Debug.Log($")");
        itemDataList = new List<ItemData>(newItemDataList);
        Debug.Log($"2.�κ��丮 ���� ������. new itemDataList : ");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.name} : {item.currentCellPos}");
        }
        Debug.Log($")");
        SaveItemsToJson();
    }

    public static List<ItemData> GetAllItems()
    {
        return itemDataList;
    }
}