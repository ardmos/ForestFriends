using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ItemSpecID는, 아이템의 이미지나 이름, 스펙 등 정보를 올바르게 초기화해주기 위한 값이고
/// ItemID는, 이미 생성된 아이템을 특정하기 위한 고유한 값이다. 
/// </summary>
public static class ItemDataManager 
{
    // 플레이어가 갖고있는 아이템 목록을 저장하는 JSON 파일 이름
    private const string JSON_FILE_PATH = "PlayerItems.json";

    [SerializeField] private static List<ItemData> playerItems = new List<ItemData>(); // 플레이어가 소지한 모든 아이템 데이터 리스트
    private static Dictionary<int, ItemData> playerItemDictionary = new Dictionary<int, ItemData>(); // 플레이어가 소지한 아이템의 ID값과 데이터를 매핑하는 딕셔너리. ID로 아이템을 빠르게 검색할 수 있게 해줍니다.
    
    // 가장 큰 ItemID 값을 관리하는 변수
    private static int currentMaxItemID = 0;

    public static List<ItemData> LoadItemsFromJson()
    {
        try
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                playerItems = JsonConvert.DeserializeObject<List<ItemData>>(json);

                Debug.Log($"플레이어 소지 아이템 목록을 찾았습니다. {json}");

                foreach (ItemData itemData in playerItems)
                {
                    //Debug.Log($"playerItems : {itemData}");

                    AddToPlayerItemDictionary(itemData);
                }

                return playerItems;
            }
            else
            {
                Debug.LogError("JSON file not found! JSON 파일을 생성합니다");
                SaveItemsToJson();
                return playerItems;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 로드 중 오류 발생: {e.Message}");
            return null;
        }
    }

    private static void SaveItemsToJson()
    {
        try
        {
            string json = JsonConvert.SerializeObject(playerItems, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // Json을 읽기 좋게 해주는 포맷팅 옵션
                Converters = new List<JsonConverter> { new Vector2Converter() } // Vector2 자료형을 직렬화 하기 위한 커스텀 JSON 컨버터
            });
            File.WriteAllText(GetFilePath(), json);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 저장 중 오류 발생: {e.Message}");
        }
    }

    private static void AddToPlayerItemDictionary(ItemData itemData)
    {
        try
        {
            // Dictionary의 ID를 키로 이미 추가된 아이템인지 확인합니다.
            if (!playerItemDictionary.ContainsKey(itemData.itemID))
            {
                playerItemDictionary.Add(itemData.itemID, itemData);

                // 테스트용. 실제 게임에선 상점 리프레시로 아이템이 생성 될 때 ID값을 설정해줘야함.
                UpdateMaxItemIdIfGreater(itemData.itemID);
            }
            else
            {
                Debug.LogError($"중복된 item ID 입니다: {itemData.itemID} for item {itemData.itemSpec.itemName}");
            }
        }
        catch(Exception e) 
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
        }
    }

    // 새로운 아이템 데이터를 추가하는 메서드
    public static bool TryAddPlayerItem(ItemData newItem)
    {
        // Dictionary의 ID를 키로 이미 추가된 아이템인지 확인합니다.
        if (!playerItemDictionary.ContainsKey(newItem.itemID))
        {
            playerItemDictionary.Add(newItem.itemID, newItem);
            playerItems.Add(newItem);
            SaveItemsToJson();

            // 테스트용. 실제 게임에선 상점 리프레시로 아이템이 생성 될 때 ID값을 설정해줘야함.
            UpdateMaxItemIdIfGreater(newItem.itemID);

            return true;
        }
        else
        {
            Debug.LogError($"중복된 item ID 입니다: {newItem.itemID} for item {newItem.itemSpec.itemName}");
            return false;
        }
    }

    public static bool TryRemovePlayerItem(ItemData item)
    {
        // Dictonary의 ID를 키로 해당 아이템이 존재하는지 확인합니다. 
        if (playerItemDictionary.ContainsKey(item.itemID))
        {
            playerItemDictionary.Remove(item.itemID);
            playerItems.Remove(item);
            SaveItemsToJson();
            return true;
        }
        else
        {
            Debug.LogError($"삭제 실패. 해당하는 ID값을 가진 아이템이 리스트에 없습니다: {item.itemID}, {item.itemSpec.itemName}");
            return false;
        }
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

    /// <summary>
    /// JSON 파일의 전체 경로를 반환
    /// </summary>
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, JSON_FILE_PATH);
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

    // 고유한 ID값을 뽑아주는 메서드
    public static int GenerateUniqueItemId()
    {
        //지금 존재하는 key값 중 가장 큰 값에서 +1 한 값을 리턴해준다. 
        return ++currentMaxItemID;
    }

    // MaxItemID값을 최신화 시켜주는 메서드
    // 테스트용. 실제 게임에선 상점 리프레시로 아이템이 생성 될 때 ID값을 설정해줘야함.
    public static void UpdateMaxItemIdIfGreater(int id)
    {
        if (id > currentMaxItemID)
            currentMaxItemID = id; // currentMaxItemID 업데이트
    }
}