using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static GoogleSheetLoader;

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

    private enum Columns
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

    // 특정 시트 gid
    private const string GID_WEAPON = "1208485493";
    private const string GID_EQUIPMENT = "1279431268";
    private const string GID_FOOD = "276662484";
    private const string GID_MISC = "229270413";
    private const string GID_BAG = "1179647824";
    private const string GID_GEM = "961347244";

    // 구글 시트 데이터를 읽어올 때 쓰이는 변수들
    private static int rowOffset = 5; // 각 아이템들이 5행씩 차지하기 때문에, 다음 아이템을 검색하기 위한 보정값
    private static int itemShapeColumnOffset = 5; // 아이템 형태 칼럼은 다섯 줄이기 때문에, 전부 읽어오기 위한 보정값.

    public static async Task<(bool success, Dictionary<int, ItemSpec> itemSpecDictionary)> LoadSpecificSheetData(Sheets sheetName)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(GetItemSheetURL(sheetName)))
        {
            //Debug.Log("아이템 스펙 데이터 다운로드를 시작합니다");
            var webRequestOpration = www.SendWebRequest();

            while (!webRequestOpration.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"아이템 스펙 데이터 다운로드를 실패했습니다. {www.error}");
                return (false, null);
            }
            else
            {
                Debug.Log($"{sheetName}아이템 스펙 데이터 다운로드를 성공했습니다.");
                //Debug.Log($"{www.downloadHandler.text}");
                return (true, ParshingSheetDataToItemSpecDictionary(www.downloadHandler.text.Split('\n')));
            }
        }
    }

    private static Dictionary<int, ItemSpec> ParshingSheetDataToItemSpecDictionary(string[] rows)
    {
        if (rows.Length == 0) return null;

        //Debug.Log($"itemSpecList로의 파싱을 시작합니다.");

        Dictionary<int, ItemSpec> result = new Dictionary<int, ItemSpec>();

        for (int lineNum = 1; lineNum < rows.Length; lineNum += rowOffset)
        {
            //Debug.Log("======================================");

            ItemSpec itemSpec = new ItemSpec();

            string[] columns = rows[lineNum].Split('\t');
            // Item Spec ID
            //Debug.Log($"Item Spec ID : {columns[(int)Columns.ItemSpecID]}");
            itemSpec.itemSpecID = StringDataParser.ParseToInt(columns[(int)Columns.ItemSpecID]);
            // Item Name
            //Debug.Log($"Item Name : {columns[(int)Columns.ItemName]}");
            itemSpec.itemName = columns[(int)Columns.ItemName];
            // Item Price
            //Debug.Log($"Item Price : {columns[(int)Columns.ItemPrice]}");
            itemSpec.itemPrice = StringDataParser.ParseToInt(columns[(int)Columns.ItemPrice]);
            // Item Description
            //Debug.Log($"Item Description : {columns[(int)Columns.ItemDescription]}");
            itemSpec.itemDescription = columns[(int)Columns.ItemDescription];
            // Attack
            //Debug.Log($"Attack : {columns[(int)Columns.Attack]}");
            itemSpec.attack = StringDataParser.ParseToFloat(columns[(int)Columns.Attack]);
            // Defence
            //Debug.Log($"Defence : {columns[(int)Columns.Defence]}");
            itemSpec.defence = StringDataParser.ParseToFloat(columns[(int)Columns.Defence]);
            // Attack Speed
            //Debug.Log($"Attack Speed : {columns[(int)Columns.AttackSpeed]}");
            itemSpec.attackSpeed = StringDataParser.ParseToFloat(columns[(int)Columns.AttackSpeed]);
            // Healing Amount
            //Debug.Log($"Healing Amount : {columns[(int)Columns.HealingAmount]}");
            itemSpec.healingAmount = StringDataParser.ParseToFloat(columns[(int)Columns.HealingAmount]);
            // Item Type
            //Debug.Log($"Item Type : {columns[(int)Columns.ItemType]}");
            itemSpec.itemType = columns[(int)Columns.ItemType];
            // Type Main Stat
            //Debug.Log($"Type Main Stat : {columns[(int)Columns.TypeMainStat]}");
            itemSpec.typeMainStat = columns[(int)Columns.TypeMainStat];
            // Type Sub Stat
            //Debug.Log($"Type Sub Stat : {columns[(int)Columns.TypeSubStat]}");
            itemSpec.typeSubStat = columns[(int)Columns.TypeSubStat];

            // 아이템 형태 정보는 첫 번째 ~ 다섯번째 줄까지. 5x5사이즈로 존재. 
            // 아이템 형태 정보 읽어오기
            //Debug.Log("Item Shape : ");
            string itemShape = "";

            for (int r = lineNum; r < lineNum + rowOffset; r++)
            {
                if (r >= rows.Length)
                {
                    //Debug.Log($"row line num:{r} 데이터의 끝 입니다.");
                    break;
                }
                
                columns = rows[r].Split('\t');
                for (int c = (int)Columns.ItemShape; c < (int)Columns.ItemShape + itemShapeColumnOffset; c++)
                {
                    if (columns[c] == "o") 
                        itemShape += "1"; // 아이템이 존재하는 칸인 경우 1로 처리
                    else
                        itemShape += "0"; // 빈칸은 0으로 처리
                }              
            }
            itemSpec.itemShape = itemShape;
            result.Add(itemSpec.itemSpecID, itemSpec);

            //Debug.Log(itemShape);
            //Debug.Log("======================================");
        }

        //Debug.Log("ItemSpecList로의 파싱이 완료되었습니다.");
        return result;
    }

    private static string GetItemSheetURL(Sheets sheet)
    {
        string gid = "";

        switch (sheet)
        {
            case Sheets.WEAPON: gid = GID_WEAPON; break;
            case Sheets.EQUIPMENT: gid = GID_EQUIPMENT; break;
            case Sheets.FOOD: gid = GID_FOOD; break;
            case Sheets.MISC: gid = GID_MISC; break;
            case Sheets.BAG: gid = GID_BAG; break;
            case Sheets.GEM: gid = GID_GEM; break;
            default: gid = GID_WEAPON; break;
        }

        return $"https://docs.google.com/spreadsheets/d/e/2PACX-1vQLgdf4HJcBCjMIQLWNSTqchySCpzpHIArTWuIwHjYYCV1S4K_j5kDtZ9sp47hDLDPhyHF7D2nXoKdO/pub?gid={gid}&single=true&output=tsv";
    }
}