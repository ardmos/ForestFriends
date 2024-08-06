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
    // 플레이어가 갖고있는 아이템 목록을 저장하는 JSON 파일
    public static readonly string jsonFilePath = "Assets/Resources/PlayerItems.json";

    [SerializeField] private static List<ItemData> playerItems = new List<ItemData>(); // 플레이어가 소지한 모든 아이템 데이터 리스트
    private static Dictionary<int, ItemData> playerItemDictionary = new Dictionary<int, ItemData>(); // 플레이어가 소지한 아이템의 ID값과 데이터를 매핑하는 딕셔너리. ID로 아이템을 빠르게 검색할 수 있게 해줍니다.
    
    private static bool AddItemToItemDictionary(ItemData item)
    {
        // 아이템 ID가 중복되지 않도록 확인
        if (!playerItemDictionary.ContainsKey(item.ID))
        {
            playerItemDictionary.Add(item.ID, item);
            return true;
        }
        else
        {
            Debug.LogError($"Duplicate item ID found: {item.ID} for item {item.itemSpec.itemName}");
            return false;
        }
    }
    private static bool RemoveItemFromItemDictionary(ItemData item)
    {
        if (playerItemDictionary.ContainsKey(item.ID))
        {
            playerItemDictionary.Remove(item.ID);
            return true;
        }
        else
        {
            Debug.LogError($"삭제 실패. 해당하는 ID값을 가진 아이템이 리스트에 없습니다: {item.ID}, {item.itemSpec.itemName}");
            return false;
        }
    }

    // ID로 아이템을 검색하는 메서드
    public static ItemData GetItemById(int id)
    {
        if (playerItemDictionary.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"Item with ID {id} not found.");
        return null;
    }


    public static List<ItemData> LoadItemsFromJson()
    {
        //Debug.Log($"ItemDataManager.LoadItemsFromJson()");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            playerItems = new List<ItemData>(wrapper.items);

            Debug.Log($"JSON 파일을 찾았습니다. 플레이어가 소지한 아이템 데이터 리스트를 반환합니다. {json}");

            foreach (ItemData wrapperItem in wrapper.items)
            {
                Debug.Log($"wrapperItem : {wrapperItem}");
            }

            foreach(ItemData itemData in playerItems)
            {
                Debug.Log($"playerItems : {itemData}");
            }

            return playerItems;
        }
        else
        {
            Debug.LogError("JSON file not found!");

            return null;
        }
    }

    private static void SaveItemsToJson()
    {
        ItemDataWrapper wrapper = new ItemDataWrapper { items = playerItems.ToArray() };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(jsonFilePath, json);
    }

    // 새로운 아이템 데이터를 추가하는 메서드
    public static bool TryAddItem(ItemData newItem)
    {
        // Dictionary의 ID를 키로 이미 추가된 아이템인지 확인합니다.
        if (!AddItemToItemDictionary(newItem)) { return false; }
        playerItems.Add(newItem);
        SaveItemsToJson();
        return true;
    }

    public static bool TryRemoveItem(ItemData item)
    {
        // Dictonary의 ID를 키로 삭제 가능한 아이템인지 확인합니다. 
        if(!RemoveItemFromItemDictionary(item)) { return false; }
        playerItems.Remove(item);  
        SaveItemsToJson();
        return true;
    }

    public static void UpdateItemDataListToJson(List<ItemData> newItemDataList)
    {
        playerItems = new List<ItemData>(newItemDataList);
        SaveItemsToJson();
    }

    public static List<ItemData> GetAllItems()
    {
        return playerItems;
    }
}