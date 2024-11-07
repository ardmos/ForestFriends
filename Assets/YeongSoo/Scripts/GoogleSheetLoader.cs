using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 구글 스프레드시트에서 게임 아이템 데이터를 로드하고 파싱하는 정적 클래스
/// </summary>
public static class GoogleSheetLoader
{
    public enum Sheets
    {
        WEAPON,
        EQUIPMENT,
        FOOD,
        MISC,
        BAG,
        GEM
    }

    private enum ItemPropertyIndex
    {
        ItemSpecID,
        ItemName,
        ItemPrice,
        ItemDescription,
        Attack,
        Defence,
        AttackSpeed,
        HealingAmount,
        ItemType,
        TypeMainStat,
        TypeSubStat,
        ItemShape
    }

    // 각 시트별 GID (구글 스프레드시트 고유 식별자)
    private static readonly Dictionary<Sheets, string> sheetGIDs = new Dictionary<Sheets, string>
    {
        { Sheets.WEAPON, "1208485493" },
        { Sheets.EQUIPMENT, "1279431268" },
        { Sheets.FOOD, "276662484" },
        { Sheets.MISC, "229270413" },
        { Sheets.BAG, "1179647824" },
        { Sheets.GEM, "961347244" }
    };

    // 데이터 파싱을 위한 상수들
    private const int itemDataRowSpacing = 5; // 각 아이템이 5행을 차지하므로, 다음 아이템을 검색하기 위한 오프셋
    private const int itemShapeHeight = itemDataRowSpacing;
    private const int itemShapeWidth = 5; // 아이템 형태 정보는 시트상에서 (row 5) x (colum 5)로 그 모든 정보를 읽어오기 위한 오프셋입니다

    /// <summary>
    /// 특정 시트의 데이터를 로드하고 파싱하는 비동기 메서드
    /// </summary>
    /// <param name="sheetName">로드할 시트 이름</param>
    /// <returns>성공 여부와 파싱된 아이템 스펙 딕셔너리</returns>
    public static async Task<(bool success, Dictionary<int, ItemSpec> itemSpecDictionary)> LoadSpecificSheetData(Sheets sheetName)
    {
        try 
        {
            using (UnityWebRequest www = UnityWebRequest.Get(GetItemSheetURL(sheetName)))
            {
                var webRequestOpration = www.SendWebRequest();

                // 비동기로 요청 완료 대기
                while (!webRequestOpration.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{sheetName}아이템 스펙 데이터 다운로드를 실패했습니다. {www.error}");
                    return (false, null);
                }
                else
                {
                    Debug.Log($"{sheetName}아이템 스펙 데이터 다운로드를 성공했습니다.");
                    return (true, ParshingSheetDataToItemSpecDictionary(www.downloadHandler.text.Split('\n'), sheetName));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"{sheetName} 데이터 로드 중 오류 발생: {ex.Message}");
            return (false, null);
        }
    }

    /// <summary>
    /// 다운로드한 시트 데이터를 ItemSpec 딕셔너리로 파싱하는 메서드
    /// </summary>
    /// <param name="sheetDataRows">시트 데이터를 행단위로 나눠둔 배열</param>
    /// <param name="sheetName">시트 이름</param>
    /// <returns>파싱된 ItemSpec 딕셔너리</returns>
    private static Dictionary<int, ItemSpec> ParshingSheetDataToItemSpecDictionary(string[] sheetDataRows, GoogleSheetLoader.Sheets sheetName)
    {
        if (sheetDataRows.Length == 0) return null;

        Dictionary<int, ItemSpec> result = new Dictionary<int, ItemSpec>();

        for (int rowNum = 1; rowNum < sheetDataRows.Length; rowNum += itemDataRowSpacing)
        {
            ItemSpec itemSpec = new ItemSpec();

            // Item Sheet  Name
            itemSpec.sheetName = sheetName;

            string[] rowCells = sheetDataRows[rowNum].Split('\t');
            // Item Spec ID
            itemSpec.itemSpecID = StringDataParser.ParseToInt(rowCells[(int)ItemPropertyIndex.ItemSpecID]);
            // Item Name
            itemSpec.itemName = rowCells[(int)ItemPropertyIndex.ItemName];
            // Item Price
            itemSpec.itemPrice = StringDataParser.ParseToInt(rowCells[(int)ItemPropertyIndex.ItemPrice]);
            // Item Description
            itemSpec.itemDescription = rowCells[(int)ItemPropertyIndex.ItemDescription];
            // Attack
            itemSpec.attack = StringDataParser.ParseToFloat(rowCells[(int)ItemPropertyIndex.Attack]);
            // Defence
            itemSpec.defence = StringDataParser.ParseToFloat(rowCells[(int)ItemPropertyIndex.Defence]);
            // Attack Speed
            itemSpec.attackSpeed = StringDataParser.ParseToFloat(rowCells[(int)ItemPropertyIndex.AttackSpeed]);
            // Healing Amount
            itemSpec.healingAmount = StringDataParser.ParseToFloat(rowCells[(int)ItemPropertyIndex.HealingAmount]);
            // Item Type
            itemSpec.itemType = rowCells[(int)ItemPropertyIndex.ItemType];
            // Type Main Stat
            itemSpec.typeMainStat = rowCells[(int)ItemPropertyIndex.TypeMainStat];
            // Type Sub Stat
            itemSpec.typeSubStat = rowCells[(int)ItemPropertyIndex.TypeSubStat];
            // Item Shape
            itemSpec.itemShape = ParseItemShape(sheetDataRows, rowNum);

            result.Add(itemSpec.itemSpecID, itemSpec);
        }

        return result;
    }

    /// <summary>
    /// 아이템 형태 정보를 파싱하는 메서드.
    /// itemShapeHeight x itemShapeWidth형태로 이루어져있는 정보를 문자열 형태로 변경해 반환합니다.
    /// </summary>
    /// <param name="sheetDataRows">시트의 전체 행 데이터</param>
    /// <param name="shapeStartRow">itemShapeHeight x itemShapeWidth 형태 데이터의 시작지점 행 번호</param>
    /// <returns>파싱된 아이템 형태 문자열</returns>
    private static string ParseItemShape(string[] sheetDataRows, int shapeStartRow)
    {
        string itemShape = "";
      
        for (int rowNum = shapeStartRow; rowNum < shapeStartRow + itemShapeHeight; rowNum++)
        {
            if (rowNum >= sheetDataRows.Length) break;

            string[] rowCells = sheetDataRows[rowNum].Split('\t');

            for (int columnNum = (int)ItemPropertyIndex.ItemShape; columnNum < (int)ItemPropertyIndex.ItemShape + itemShapeWidth; columnNum++)
            {
                itemShape += (rowCells[columnNum] == "o") ? "1" : "0";
            }
        }

        return itemShape;
    }

    /// <summary>
    /// 특정 시트의 URL을 반환하는 메서드
    /// </summary>
    /// <param name="sheet">시트 종류</param>
    /// <returns>해당 시트의 URL</returns>
    private static string GetItemSheetURL(Sheets sheet)
    {
        if (!sheetGIDs.TryGetValue(sheet, out string gid))
        {
            gid = sheetGIDs[Sheets.WEAPON]; // 기본값으로 WEAPON 시트 사용
        }

        return $"https://docs.google.com/spreadsheets/d/e/2PACX-1vQLgdf4HJcBCjMIQLWNSTqchySCpzpHIArTWuIwHjYYCV1S4K_j5kDtZ9sp47hDLDPhyHF7D2nXoKdO/pub?gid={gid}&single=true&output=tsv";
    }
}