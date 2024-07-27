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

            Debug.Log($"JSON 파일을 찾았습니다. 아이템 데이터 리스트를 반환합니다. {json}");

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

    public static void SaveItemsToJson()
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

    public static List<ItemData> GetAllItems()
    {
        return new List<ItemData>(itemDataList);
    }
}