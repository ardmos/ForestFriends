using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

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
    
    private static bool AddItemToItemDictionary(ItemData item)
    {
        // ������ ID�� �ߺ����� �ʵ��� Ȯ��
        if (!playerItemDictionary.ContainsKey(item.itemID))
        {
            playerItemDictionary.Add(item.itemID, item);
            return true;
        }
        else
        {
            Debug.LogError($"Duplicate item ID found: {item.itemID} for item {item.itemSpec.itemName}");
            return false;
        }
    }
    private static bool RemoveItemFromItemDictionary(ItemData item)
    {
        if (playerItemDictionary.ContainsKey(item.itemID))
        {
            playerItemDictionary.Remove(item.itemID);
            return true;
        }
        else
        {
            Debug.LogError($"���� ����. �ش��ϴ� ID���� ���� �������� ����Ʈ�� �����ϴ�: {item.itemID}, {item.itemSpec.itemName}");
            return false;
        }
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

    public static List<ItemData> LoadItemsFromJson()
    {
        try
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                playerItems = JsonConvert.DeserializeObject<List<ItemData>>(json);

                Debug.Log($"JSON ������ ã�ҽ��ϴ�. �÷��̾ ������ ������ ������ ����Ʈ�� ��ȯ�մϴ�. {json}");

                foreach (ItemData itemData in playerItems)
                {
                    Debug.Log($"playerItems : {itemData}");
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

    // ���ο� ������ �����͸� �߰��ϴ� �޼���
    public static bool TryAddItem(ItemData newItem)
    {
        // Dictionary�� ID�� Ű�� �̹� �߰��� ���������� Ȯ���մϴ�.
        if (!AddItemToItemDictionary(newItem)) { return false; }
        playerItems.Add(newItem);
        SaveItemsToJson();
        return true;
    }

    public static bool TryRemoveItem(ItemData item)
    {
        // Dictonary�� ID�� Ű�� ���� ������ ���������� Ȯ���մϴ�. 
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

    /// <summary>
    /// JSON ������ ��ü ��θ� ��ȯ
    /// </summary>
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, JSON_FILE_PATH);
    }
}