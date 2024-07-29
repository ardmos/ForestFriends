using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ItemLoader 
{
    public static string jsonFilePath = "Assets/Resources/itemData.json";
    public static List<ItemData> itemDataList = new List<ItemData>();

    public static List<ItemData> LoadItemsFromJson()
    {
        Debug.Log($"ItemLoader.LoadItemsFromJson()");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            itemDataList = new List<ItemData>(wrapper.items);

            Debug.Log($"JSON ������ ã�ҽ��ϴ�. ������ ������ ����Ʈ�� ��ȯ�մϴ�. {json}");

            foreach (ItemData wrapperItem in wrapper.items)
            {
                Debug.Log($"wrapperItem.item : {wrapperItem.item}");
            }

            foreach(ItemData itemData in itemDataList)
            {
                Debug.Log($"itemDataList.item : {itemData.item}");
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
        SaveItemsToJson();
    }

    public static void SaveAllItems(List<ItemData> newItemDataList)
    {
        Debug.Log($"1.�����۸���Ʈ ������. itemDataList : (");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.item.itemName} : {item.item.currentCellPos}");
        }
        Debug.Log($")");
        itemDataList = new List<ItemData>(newItemDataList);
        Debug.Log($"2.�����۸���Ʈ ������. new itemDataList : ");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.item.itemName} : {item.item.currentCellPos}");
        }
        Debug.Log($")");
        SaveItemsToJson();
    }

    public static List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDataList);
    }
}