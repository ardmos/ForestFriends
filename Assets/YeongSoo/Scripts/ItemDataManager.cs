using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;
/// <summary>
/// ItemSpecID��, �������� �̹����� �̸�, ���� �� ������ �ùٸ��� �ʱ�ȭ���ֱ� ���� ���̰�
/// ItemID��, �̹� ������ �������� Ư���ϱ� ���� ������ ���̴�. 
/// </summary>
public static class ItemDataManager 
{
    // �÷��̾ �����ִ� ������ ����� �����ϴ� JSON ����
    public static readonly string jsonFilePath = "Assets/Resources/PlayerItems.json";

    [SerializeField] private static List<ItemData> playerItems = new List<ItemData>(); // �÷��̾ ������ ��� ������ ������ ����Ʈ
    private static Dictionary<int, ItemData> playerItemDictionary = new Dictionary<int, ItemData>(); // �÷��̾ ������ �������� ID���� �����͸� �����ϴ� ��ųʸ�. ID�� �������� ������ �˻��� �� �ְ� ���ݴϴ�.
    
    private static bool AddItemToItemDictionary(ItemData item)
    {
        // ������ ID�� �ߺ����� �ʵ��� Ȯ��
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
            Debug.LogError($"���� ����. �ش��ϴ� ID���� ���� �������� ����Ʈ�� �����ϴ�: {item.ID}, {item.itemSpec.itemName}");
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
        //Debug.Log($"ItemDataManager.LoadItemsFromJson()");
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            playerItems = new List<ItemData>(wrapper.items);

            Debug.Log($"JSON ������ ã�ҽ��ϴ�. �÷��̾ ������ ������ ������ ����Ʈ�� ��ȯ�մϴ�. {json}");

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
}