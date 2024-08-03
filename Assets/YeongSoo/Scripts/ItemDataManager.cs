using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ItemDataManager 
{
    public static string jsonFilePath = "Assets/Resources/itemData.json";
    public static List<ItemData> itemDataList = new List<ItemData>();

    public static List<ItemData> LoadItemsFromJson()
    {
        Debug.Log($"ItemDataManager.LoadItemsFromJson()");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            itemDataList = new List<ItemData>(wrapper.items);

            Debug.Log($"JSON 파일을 찾았습니다. 아이템 데이터 리스트를 반환합니다. {json}");

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
        SaveItemsToJson();
    }

    public static void SaveCurrentInventoryData(List<ItemData> newItemDataList)
    {
        Debug.Log($"1.인벤토리 정보 저장중. itemDataList : (");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.itemName} : {item.currentCellPos}");
        }
        Debug.Log($")");
        itemDataList = new List<ItemData>(newItemDataList);
        Debug.Log($"2.인벤토리 정보 저장중. new itemDataList : ");
        foreach (ItemData item in itemDataList)
        {
            Debug.Log($"{item.itemName} : {item.currentCellPos}");
        }
        Debug.Log($")");
        SaveItemsToJson();
    }

    public static List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDataList);
    }
}