using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ItemSpecID��, �������� �̹����� �̸�, ���� �� ������ �ùٸ��� �ʱ�ȭ���ֱ� ���� ���̰�
/// ItemID��, �̹� ������ �������� Ư���ϱ� ���� ������ ���̴�. 
/// </summary>
public static class ItemDataManager 
{
    // �÷��̾ �����ִ� ������ ����� �����ϴ� JSON ���� �̸�
    private const string JSON_FILE_PATH = "PlayerItems.json";

    [SerializeField] private static List<ItemData> playerItems = new List<ItemData>(); // �÷��̾ ������ ��� ������ ������ ����Ʈ
    private static Dictionary<int, ItemData> playerItemDictionary = new Dictionary<int, ItemData>(); // �÷��̾ ������ �������� ID���� �����͸� �����ϴ� ��ųʸ�. ID�� �������� ������ �˻��� �� �ְ� ���ݴϴ�.
    
    // ���� ū ItemID ���� �����ϴ� ����
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

                Debug.Log($"�÷��̾� ���� ������ ����� ã�ҽ��ϴ�. {json}");

                foreach (ItemData itemData in playerItems)
                {
                    //Debug.Log($"playerItems : {itemData}");

                    AddToPlayerItemDictionary(itemData);
                }

                return playerItems;
            }
            else
            {
                Debug.LogError("JSON file not found! JSON ������ �����մϴ�");
                SaveItemsToJson();
                return playerItems;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON ���� �ε� �� ���� �߻�: {e.Message}");
            return null;
        }
    }

    private static void SaveItemsToJson()
    {
        try
        {
            string json = JsonConvert.SerializeObject(playerItems, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // Json�� �б� ���� ���ִ� ������ �ɼ�
                Converters = new List<JsonConverter> { new Vector2Converter() } // Vector2 �ڷ����� ����ȭ �ϱ� ���� Ŀ���� JSON ������
            });
            File.WriteAllText(GetFilePath(), json);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON ���� ���� �� ���� �߻�: {e.Message}");
        }
    }

    private static void AddToPlayerItemDictionary(ItemData itemData)
    {
        try
        {
            // Dictionary�� ID�� Ű�� �̹� �߰��� ���������� Ȯ���մϴ�.
            if (!playerItemDictionary.ContainsKey(itemData.itemID))
            {
                playerItemDictionary.Add(itemData.itemID, itemData);

                // �׽�Ʈ��. ���� ���ӿ��� ���� �������÷� �������� ���� �� �� ID���� �����������.
                UpdateMaxItemIdIfGreater(itemData.itemID);
            }
            else
            {
                Debug.LogError($"�ߺ��� item ID �Դϴ�: {itemData.itemID} for item {itemData.itemSpec.itemName}");
            }
        }
        catch(Exception e) 
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
        }
    }

    // ���ο� ������ �����͸� �߰��ϴ� �޼���
    public static bool TryAddPlayerItem(ItemData newItem)
    {
        // Dictionary�� ID�� Ű�� �̹� �߰��� ���������� Ȯ���մϴ�.
        if (!playerItemDictionary.ContainsKey(newItem.itemID))
        {
            playerItemDictionary.Add(newItem.itemID, newItem);
            playerItems.Add(newItem);
            SaveItemsToJson();

            // �׽�Ʈ��. ���� ���ӿ��� ���� �������÷� �������� ���� �� �� ID���� �����������.
            UpdateMaxItemIdIfGreater(newItem.itemID);

            return true;
        }
        else
        {
            Debug.LogError($"�ߺ��� item ID �Դϴ�: {newItem.itemID} for item {newItem.itemSpec.itemName}");
            return false;
        }
    }

    public static bool TryRemovePlayerItem(ItemData item)
    {
        // Dictonary�� ID�� Ű�� �ش� �������� �����ϴ��� Ȯ���մϴ�. 
        if (playerItemDictionary.ContainsKey(item.itemID))
        {
            playerItemDictionary.Remove(item.itemID);
            playerItems.Remove(item);
            SaveItemsToJson();
            return true;
        }
        else
        {
            Debug.LogError($"���� ����. �ش��ϴ� ID���� ���� �������� ����Ʈ�� �����ϴ�: {item.itemID}, {item.itemSpec.itemName}");
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
    /// JSON ������ ��ü ��θ� ��ȯ
    /// </summary>
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, JSON_FILE_PATH);
    }

    // ID�� �������� �˻��ϴ� �޼���
    public static ItemData GetItemById(int id)
    {
        if (playerItemDictionary.TryGetValue(id, out ItemData item))
        {
            return item;
        }
        Debug.LogWarning($"Item with ID {id} not found.");
        return null;
    }

    // ������ ID���� �̾��ִ� �޼���
    public static int GenerateUniqueItemId()
    {
        //���� �����ϴ� key�� �� ���� ū ������ +1 �� ���� �������ش�. 
        return ++currentMaxItemID;
    }

    // MaxItemID���� �ֽ�ȭ �����ִ� �޼���
    // �׽�Ʈ��. ���� ���ӿ��� ���� �������÷� �������� ���� �� �� ID���� �����������.
    public static void UpdateMaxItemIdIfGreater(int id)
    {
        if (id > currentMaxItemID)
            currentMaxItemID = id; // currentMaxItemID ������Ʈ
    }
}