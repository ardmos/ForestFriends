using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;
/// <summary>
/// ItemSpecID는, 아이템의 이미지나 이름, 스펙 등 정보를 올바르게 초기화해주기 위한 값이고
/// ItemID는, 이미 생성된 아이템을 특정하기 위한 고유한 값이다. 
/// </summary>
public static class ItemDataManager 
{
    // 유저가 갖고있는 아이템 목록을 저장하는 JSON 파일
    public static readonly string jsonFilePath = "Assets/Resources/itemData.json";

    [SerializeField] private static List<ItemData> itemDataList = new List<ItemData>(); // 모든 아이템 데이터 리스트
    private static Dictionary<int, ItemData> itemDictionary; // ID와 아이템 데이터를 매핑하는 딕셔너리. ID로 아이템을 검색할 수 있게 해줍니다.

    // itemDictionary에 ID와 아이템 데이터를 매핑합니다.
    private static void InitializeItemDictionary()
    {
        itemDictionary = new Dictionary<int, ItemData>();
        foreach (ItemData item in itemDataList)
        {
            AddItemToItemDictionary(item);
        }
    }
    
    private static void AddItemToItemDictionary(ItemData item)
    {
        // 아이템 ID가 중복되지 않도록 확인
        if (!itemDictionary.ContainsKey(item.ID))
        {
            itemDictionary.Add(item.ID, item);
        }
        else
        {
            Debug.LogError($"Duplicate item ID found: {item.ID} for item {item.name}");
        }
    }
    private static void RemoveItemFromItemDictionary(ItemData item)
    {
        if (itemDictionary.ContainsKey(item.ID))
        {
            itemDictionary.Remove(item.ID);
        }
        else
        {
            Debug.LogError($"삭제 실패. 해당하는 ID값을 가진 아이템이 리스트에 없습니다: {item.ID}, {item.name}");
        }
    }

    // ID로 아이템을 검색하는 메서드
    public static ItemData GetItemById(int id)
    {
        if (itemDictionary.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"Item with ID {id} not found.");
        return null;
    }


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
        AddItemToItemDictionary(newItem);
        //Debug.Log($"아이템을 추가했습니다 : {newItem}");
        SaveItemsToJson();
    }

    public static void RemoveItem(ItemData item)
    {
        itemDataList.Remove(item);
        RemoveItemFromItemDictionary(item);
        //Debug.Log($"아이템을 제거했습니다 : {item}");
        SaveItemsToJson();
    }

    public static void UpdateItemDataListToJson(List<ItemData> newItemDataList)
    {
        itemDataList = new List<ItemData>(newItemDataList);
        SaveItemsToJson();
    }

    public static List<ItemData> GetAllItems()
    {
        return itemDataList;
    }
}