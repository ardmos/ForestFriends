using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class ItemSpecManager
{
    // 아이템 스펙 데이터를 저장할 딕셔너리
    private static Dictionary<int, ItemSpec> itemSpecDictionary = new Dictionary<int, ItemSpec>();
    // JSON 파일의 이름
    private const string JSON_FILE_PATH = "ItemSpecs.json";

    /// <summary>
    /// 아이템 스펙 데이터를 로드하는 메인 메서드
    /// </summary>
    public static async Task LoadItemSpecs()
    {
        try
        {
            // 구글 시트에서 데이터 로드 시도
            var task = await GoogleSheetLoader.LoadItemSpecsFromGoogleSheet();

            if (task.success)
            {
                itemSpecDictionary = task.itemSpecDictionary;
                Debug.Log("아이템 스펙 데이터 다운로드를 성공했습니다!");
                SaveItemSpecsToJson(); // 성공 시 JSON으로 저장
            }
            else
            {
                Debug.LogWarning("구글 시트에서 데이터 로드 실패. JSON 파일에서 로드를 시도합니다.");
                LoadItemSpecsFromJson(); // 실패 시 JSON에서 로드 시도
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
            LoadItemSpecsFromJson(); // 예외 발생 시 JSON에서 로드 시도
        }
    }

    /// <summary>
    /// 아이템 스펙 데이터를 JSON 파일로 저장
    /// </summary>
    private static void SaveItemSpecsToJson()
    {
        try
        {
            // 딕셔너리를 JSON 문자열로 변환
            string json = JsonConvert.SerializeObject(itemSpecDictionary, Formatting.Indented);
            // JSON 문자열을 파일로 저장
            File.WriteAllText(GetFilePath(), json);
            Debug.Log($"아이템 스펙 데이터를 JSON 파일로 저장했습니다. {GetFilePath()}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 파일 저장 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// JSON 파일에서 아이템 스펙 데이터를 로드
    /// </summary>
    private static void LoadItemSpecsFromJson()
    {
        try
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                // JSON 파일 읽기
                string json = File.ReadAllText(filePath);
                // JSON 문자열을 딕셔너리로 변환
                itemSpecDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemSpec>>(json);
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
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, JSON_FILE_PATH);
    }

    /// <summary>
    /// 특정 아이템 스펙 ID에 해당하는 ItemSpec 객체를 반환
    /// </summary>
    /// <param name="itemSpecID">찾고자 하는 아이템의 스펙 ID</param>
    /// <returns>찾은 ItemSpec 객체, 없으면 null</returns>
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