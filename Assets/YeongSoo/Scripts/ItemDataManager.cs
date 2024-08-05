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
    // ������ �����ִ� ������ ����� �����ϴ� JSON ����
    public static readonly string jsonFilePath = "Assets/Resources/itemData.json";

    [SerializeField] private static List<ItemData> itemDataList = new List<ItemData>(); // ��� ������ ������ ����Ʈ
    private static Dictionary<int, ItemData> itemDictionary; // ID�� ������ �����͸� �����ϴ� ��ųʸ�. ID�� �������� �˻��� �� �ְ� ���ݴϴ�.

    // itemDictionary�� ID�� ������ �����͸� �����մϴ�.
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
        // ������ ID�� �ߺ����� �ʵ��� Ȯ��
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
            Debug.LogError($"���� ����. �ش��ϴ� ID���� ���� �������� ����Ʈ�� �����ϴ�: {item.ID}, {item.name}");
        }
    }

    // ID�� �������� �˻��ϴ� �޼���
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
        AddItemToItemDictionary(newItem);
        //Debug.Log($"�������� �߰��߽��ϴ� : {newItem}");
        SaveItemsToJson();
    }

    public static void RemoveItem(ItemData item)
    {
        itemDataList.Remove(item);
        RemoveItemFromItemDictionary(item);
        //Debug.Log($"�������� �����߽��ϴ� : {item}");
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