using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemSpecManager
{
    // ������ ���� �����͸� ������ ��ųʸ�
    private static Dictionary<int, ItemSpec> itemWeaponSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemEquipmentSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemFoodSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemMiscSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemBagSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemGemSpecDictionary = new Dictionary<int, ItemSpec>();

    // JSON ������ �̸�
    private const string JSON_FILE_PATH_WEAPON_SPECS = "ItemWeaponSpecs.json";
    private const string JSON_FILE_PATH_EQUIPMENT_SPECS = "ItemEquipmentSpecs.json";
    private const string JSON_FILE_PATH_FOOD_SPECS = "ItemFoodSpecs.json";
    private const string JSON_FILE_PATH_MISC_SPECS = "ItemMiscSpecs.json";
    private const string JSON_FILE_PATH_BAG_SPECS = "ItemBagSpecs.json";
    private const string JSON_FILE_PATH_GEM_SPECS = "ItemGemSpecs.json";

    /// <summary>
    /// ������ ���� �����͸� �ε��ϴ� ���� �޼���
    /// </summary>
    public static async Task LoadItemSpecs()
    {
        try
        {
            // ���� ��Ʈ���� ������ �ε� �õ�

            for (int i = 0; i <= (int)GoogleSheetLoader.Sheets.GEM; i++)
            {
                GoogleSheetLoader.Sheets sheetName = (GoogleSheetLoader.Sheets)i;
                var task = await GoogleSheetLoader.LoadSpecificSheetData(sheetName);

                if (task.success)
                {
                    //Debug.Log("������ ���� ������ �ٿ�ε带 �����߽��ϴ�!");

                    Dictionary<int, ItemSpec> dictionary = GetDictionaryBySheetName(sheetName);
                    if (dictionary == null)
                    {
                        Debug.LogError("��ųʸ� �˻� ����");
                        return;
                    }

                    foreach (var kvp in task.itemSpecDictionary)
                    {
                        dictionary[kvp.Key] = kvp.Value; // GetDictionaryBySheetName�� ��ȯ���� ��ųʸ��� ���ο� ���� ������Ʈ
                    }
                    SaveItemSpecsToJson(sheetName, dictionary);
                }
                else
                {
                    Debug.LogWarning($"���� ��Ʈ���� {sheetName}������ �ε� ����. JSON ���Ͽ��� �ε带 �õ��մϴ�.");
                    LoadItemSpecsFromJson(sheetName); // ���� �� JSON���� �ε� �õ�
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
            // ���� ���� �˾�?
        }
    }

    /// <summary>
    /// ������ ���� �����͸� JSON ���Ϸ� ����
    /// </summary>
    private static void SaveItemSpecsToJson(GoogleSheetLoader.Sheets sheetName, Dictionary<int, ItemSpec> dictionaryToSave)
    {
        try
        {
            // ��ųʸ��� JSON ���ڿ��� ��ȯ
            string json = JsonConvert.SerializeObject(dictionaryToSave, Formatting.Indented);
            // JSON ���ڿ��� ���Ϸ� ����
            File.WriteAllText(GetFilePath(sheetName), json);
            //Debug.Log($"������ ���� �����͸� JSON ���Ϸ� �����߽��ϴ�. {GetFilePath(sheetName)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON ���� ���� �� ���� �߻�: {e.Message}");
        }
    }

    /// <summary>
    /// JSON ���Ͽ��� ������ ���� �����͸� �ε�
    /// </summary>
    private static void LoadItemSpecsFromJson(GoogleSheetLoader.Sheets sheetName)
    {
        try
        {
            string filePath = GetFilePath(sheetName);
            if (File.Exists(filePath))
            {
                // JSON ���� �б�
                string json = File.ReadAllText(filePath);
                // JSON ���ڿ��� ��ųʸ��� ��ȯ
                Dictionary<int, ItemSpec> dictionary = GetDictionaryBySheetName(sheetName);
                dictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemSpec>>(json);
                Debug.Log("JSON ���Ͽ��� ������ ���� �����͸� �ε��߽��ϴ�.");
            }
            else
            {
                Debug.LogError("JSON ������ �������� �ʽ��ϴ�. ������ ���� ������ �ٿ�ε带 ��õ��ؾ��մϴ�.");
                // ���ͳ� ������ Ȯ���϶� �˾��� �Բ� ������ �����Ű�°� ���ƺ���.
                //LoadItemSpecs().Wait(); // �񵿱� �޼��带 ���������� ȣ�� (����: ���� �����忡�� ȣ�� �� ����� ���ɼ�)
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON ���� �ε� �� ���� �߻�: {e.Message}");
        }
    }

    /// <summary>
    /// JSON ������ ��ü ��θ� ��ȯ
    /// </summary>
    private static string GetFilePath(GoogleSheetLoader.Sheets sheetName)
    {
        string jsonFilePath = "";
        switch (sheetName)
        {
            case GoogleSheetLoader.Sheets.WEAPON:
                jsonFilePath = JSON_FILE_PATH_WEAPON_SPECS;
                break;
            case GoogleSheetLoader.Sheets.EQUIPMENT:
                jsonFilePath = JSON_FILE_PATH_EQUIPMENT_SPECS;
                break;
            case GoogleSheetLoader.Sheets.FOOD:
                jsonFilePath = JSON_FILE_PATH_FOOD_SPECS;
                break;
            case GoogleSheetLoader.Sheets.MISC:
                jsonFilePath = JSON_FILE_PATH_MISC_SPECS;
                break;
            case GoogleSheetLoader.Sheets.BAG:
                jsonFilePath = JSON_FILE_PATH_BAG_SPECS;
                break;
            case GoogleSheetLoader.Sheets.GEM:
                jsonFilePath = JSON_FILE_PATH_GEM_SPECS;
                break;
            default:
                Debug.LogError("GetFilePath error");
                break;
        }
        return Path.Combine(Application.persistentDataPath, jsonFilePath);
    }

    /// <summary>
    /// Ư�� ������ ���� ID�� �ش��ϴ� ItemSpec ��ü�� ��ȯ
    /// </summary>
    /// <param name="itemSpecID">ã���� �ϴ� �������� ���� ID</param>
    /// <returns>ã�� ItemSpec ��ü, ������ null</returns>
    public static ItemSpec GetItemSpecBySpecID(GoogleSheetLoader.Sheets sheetName, int itemSpecID)
    {
        if (GetDictionaryBySheetName(sheetName).TryGetValue(itemSpecID, out ItemSpec itemSpec))
        {
            return itemSpec;
        }
        Debug.LogWarning($"{sheetName} ItemSpec with ItemSpecID {itemSpecID} not found.");
        return null;
    }

    private static Dictionary<int, ItemSpec> GetDictionaryBySheetName(GoogleSheetLoader.Sheets sheetName)
    {
        switch (sheetName)
        {
            case GoogleSheetLoader.Sheets.WEAPON: return itemWeaponSpecDictionary;
            case GoogleSheetLoader.Sheets.EQUIPMENT: return itemEquipmentSpecDictionary;
            case GoogleSheetLoader.Sheets.FOOD: return itemFoodSpecDictionary;
            case GoogleSheetLoader.Sheets.MISC: return itemMiscSpecDictionary;
            case GoogleSheetLoader.Sheets.BAG: return itemBagSpecDictionary;
            case GoogleSheetLoader.Sheets.GEM: return itemGemSpecDictionary;
            default:
                Debug.LogError("GetDictionaryBySheetName() error");
                return null;
        }
    }
}