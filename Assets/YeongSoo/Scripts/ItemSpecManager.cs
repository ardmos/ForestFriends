using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class ItemSpecManager
{
    // ������ ���� �����͸� ������ ��ųʸ�
    private static Dictionary<int, ItemSpec> itemSpecDictionary = new Dictionary<int, ItemSpec>();
    // JSON ������ �̸�
    private const string JSON_FILE_PATH = "ItemSpecs.json";

    /// <summary>
    /// ������ ���� �����͸� �ε��ϴ� ���� �޼���
    /// </summary>
    public static async Task LoadItemSpecs()
    {
        try
        {
            // ���� ��Ʈ���� ������ �ε� �õ�
            var task = await GoogleSheetLoader.LoadItemSpecsFromGoogleSheet();

            if (task.success)
            {
                itemSpecDictionary = task.itemSpecDictionary;
                Debug.Log("������ ���� ������ �ٿ�ε带 �����߽��ϴ�!");
                SaveItemSpecsToJson(); // ���� �� JSON���� ����
            }
            else
            {
                Debug.LogWarning("���� ��Ʈ���� ������ �ε� ����. JSON ���Ͽ��� �ε带 �õ��մϴ�.");
                LoadItemSpecsFromJson(); // ���� �� JSON���� �ε� �õ�
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
            LoadItemSpecsFromJson(); // ���� �߻� �� JSON���� �ε� �õ�
        }
    }

    /// <summary>
    /// ������ ���� �����͸� JSON ���Ϸ� ����
    /// </summary>
    private static void SaveItemSpecsToJson()
    {
        try
        {
            // ��ųʸ��� JSON ���ڿ��� ��ȯ
            string json = JsonConvert.SerializeObject(itemSpecDictionary, Formatting.Indented);
            // JSON ���ڿ��� ���Ϸ� ����
            File.WriteAllText(GetFilePath(), json);
            Debug.Log($"������ ���� �����͸� JSON ���Ϸ� �����߽��ϴ�. {GetFilePath()}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON ���� ���� �� ���� �߻�: {e.Message}");
        }
    }

    /// <summary>
    /// JSON ���Ͽ��� ������ ���� �����͸� �ε�
    /// </summary>
    private static void LoadItemSpecsFromJson()
    {
        try
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                // JSON ���� �б�
                string json = File.ReadAllText(filePath);
                // JSON ���ڿ��� ��ųʸ��� ��ȯ
                itemSpecDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemSpec>>(json);
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
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, JSON_FILE_PATH);
    }

    /// <summary>
    /// Ư�� ������ ���� ID�� �ش��ϴ� ItemSpec ��ü�� ��ȯ
    /// </summary>
    /// <param name="itemSpecID">ã���� �ϴ� �������� ���� ID</param>
    /// <returns>ã�� ItemSpec ��ü, ������ null</returns>
    public static ItemSpec GetItemSpecBySpecID(int itemSpecID)
    {
        if (itemSpecDictionary.TryGetValue(itemSpecID, out ItemSpec itemSpec))
        {
            return itemSpec;
        }
        Debug.LogWarning($"ItemSpec with ItemSpecID {itemSpecID} not found.");
        return null;
    }
}