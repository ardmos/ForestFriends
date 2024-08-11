using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemSpecManager
{
    // 아이템 스펙 데이터를 저장할 딕셔너리
    private static Dictionary<int, ItemSpec> itemWeaponSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemEquipmentSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemFoodSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemMiscSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemBagSpecDictionary = new Dictionary<int, ItemSpec>();
    private static Dictionary<int, ItemSpec> itemGemSpecDictionary = new Dictionary<int, ItemSpec>();

    // JSON 파일의 이름
    private const string JSON_FILE_PATH_WEAPON_SPECS = "ItemWeaponSpecs.json";
    private const string JSON_FILE_PATH_EQUIPMENT_SPECS = "ItemEquipmentSpecs.json";
    private const string JSON_FILE_PATH_FOOD_SPECS = "ItemFoodSpecs.json";
    private const string JSON_FILE_PATH_MISC_SPECS = "ItemMiscSpecs.json";
    private const string JSON_FILE_PATH_BAG_SPECS = "ItemBagSpecs.json";
    private const string JSON_FILE_PATH_GEM_SPECS = "ItemGemSpecs.json";

    /// <summary>
    /// 아이템 스펙 데이터를 로드하는 메인 메서드
    /// </summary>
    public static async Task LoadItemSpecs()
    {
        try
        {
            // 구글 시트에서 데이터 로드 시도

            for (int i = 0; i <= (int)GoogleSheetLoader.Sheets.GEM; i++)
            {
                GoogleSheetLoader.Sheets sheetName = (GoogleSheetLoader.Sheets)i;
                var task = await GoogleSheetLoader.LoadSpecificSheetData(sheetName);

                if (task.success)
                {
                    //Debug.Log("아이템 스펙 데이터 다운로드를 성공했습니다!");

                    Dictionary<int, ItemSpec> dictionary = GetDictionaryBySheetName(sheetName);
                    if (dictionary == null)
                    {
                        Debug.LogError("딕셔너리 검색 실패");
                        return;
                    }

                    foreach (var kvp in task.itemSpecDictionary)
                    {
                        dictionary[kvp.Key] = kvp.Value; // GetDictionaryBySheetName로 반환받은 딕셔너리에 새로운 값을 업데이트
                    }
                    SaveItemSpecsToJson(sheetName, dictionary);
                }
                else
                {
                    Debug.LogWarning($"구글 시트에서 {sheetName}데이터 로드 실패. JSON 파일에서 로드를 시도합니다.");
                    LoadItemSpecsFromJson(sheetName); // 실패 시 JSON에서 로드 시도
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
            // 게임 종료 팝업?
        }
    }

    /// <summary>
    /// 아이템 스펙 데이터를 JSON 파일로 저장
    /// </summary>
    private static void SaveItemSpecsToJson(GoogleSheetLoader.Sheets sheetName, Dictionary<int, ItemSpec> dictionaryToSave)
    {
        try
        {
            // 딕셔너리를 JSON 문자열로 변환
            string json = JsonConvert.SerializeObject(dictionaryToSave, Formatting.Indented);
            // JSON 문자열을 파일로 저장
            File.WriteAllText(GetFilePath(sheetName), json);
            //Debug.Log($"아이템 스펙 데이터를 JSON 파일로 저장했습니다. {GetFilePath(sheetName)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 저장 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// JSON 파일에서 아이템 스펙 데이터를 로드
    /// </summary>
    private static void LoadItemSpecsFromJson(GoogleSheetLoader.Sheets sheetName)
    {
        try
        {
            string filePath = GetFilePath(sheetName);
            if (File.Exists(filePath))
            {
                // JSON 파일 읽기
                string json = File.ReadAllText(filePath);
                // JSON 문자열을 딕셔너리로 변환
                Dictionary<int, ItemSpec> dictionary = GetDictionaryBySheetName(sheetName);
                dictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemSpec>>(json);
                Debug.Log("JSON 파일에서 아이템 스펙 데이터를 로드했습니다.");
            }
            else
            {
                Debug.LogError("JSON 파일이 존재하지 않습니다. 아이템 스펙 데이터 다운로드를 재시도해야합니다.");
                // 인터넷 연결을 확인하란 팝업과 함께 게임을 종료시키는게 좋아보임.
                //LoadItemSpecs().Wait(); // 비동기 메서드를 동기적으로 호출 (주의: 메인 스레드에서 호출 시 데드락 가능성)
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 로드 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// JSON 파일의 전체 경로를 반환
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
    /// 특정 아이템 스펙 ID에 해당하는 ItemSpec 객체를 반환
    /// </summary>
    /// <param name="itemSpecID">찾고자 하는 아이템의 스펙 ID</param>
    /// <returns>찾은 ItemSpec 객체, 없으면 null</returns>
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