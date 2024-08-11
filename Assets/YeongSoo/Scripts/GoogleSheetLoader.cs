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

    // Ư�� ��Ʈ gid
    private const string GID_WEAPON = "1208485493";
    private const string GID_EQUIPMENT = "1279431268";
    private const string GID_FOOD = "276662484";
    private const string GID_MISC = "229270413";
    private const string GID_BAG = "1179647824";
    private const string GID_GEM = "961347244";

    // ���� ��Ʈ �����͸� �о�� �� ���̴� ������
    private static int rowOffset = 5; // �� �����۵��� 5�྿ �����ϱ� ������, ���� �������� �˻��ϱ� ���� ������
    private static int itemShapeColumnOffset = 5; // ������ ���� Į���� �ټ� ���̱� ������, ���� �о���� ���� ������.

    public static async Task<(bool success, Dictionary<int, ItemSpec> itemSpecDictionary)> LoadSpecificSheetData(Sheets sheetName)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(GetItemSheetURL(sheetName)))
        {
            //Debug.Log("������ ���� ������ �ٿ�ε带 �����մϴ�");
            var webRequestOpration = www.SendWebRequest();

            while (!webRequestOpration.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"������ ���� ������ �ٿ�ε带 �����߽��ϴ�. {www.error}");
                return (false, null);
            }
            else
            {
                Debug.Log($"{sheetName}������ ���� ������ �ٿ�ε带 �����߽��ϴ�.");
                //Debug.Log($"{www.downloadHandler.text}");
                return (true, ParshingSheetDataToItemSpecDictionary(www.downloadHandler.text.Split('\n')));
            }
        }
    }

    private static Dictionary<int, ItemSpec> ParshingSheetDataToItemSpecDictionary(string[] rows)
    {
        if (rows.Length == 0) return null;

        //Debug.Log($"itemSpecList���� �Ľ��� �����մϴ�.");

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

            // ������ ���� ������ ù ��° ~ �ټ���° �ٱ���. 5x5������� ����. 
            // ������ ���� ���� �о����
            //Debug.Log("Item Shape : ");
            string itemShape = "";

            for (int r = lineNum; r < lineNum + rowOffset; r++)
            {
                if (r >= rows.Length)
                {
                    //Debug.Log($"row line num:{r} �������� �� �Դϴ�.");
                    break;
                }
                
                columns = rows[r].Split('\t');
                for (int c = (int)Columns.ItemShape; c < (int)Columns.ItemShape + itemShapeColumnOffset; c++)
                {
                    if (columns[c] == "o") 
                        itemShape += "1"; // �������� �����ϴ� ĭ�� ��� 1�� ó��
                    else
                        itemShape += "0"; // ��ĭ�� 0���� ó��
                }              
            }
            itemSpec.itemShape = itemShape;
            result.Add(itemSpec.itemSpecID, itemSpec);

            //Debug.Log(itemShape);
            //Debug.Log("======================================");
        }

        //Debug.Log("ItemSpecList���� �Ľ��� �Ϸ�Ǿ����ϴ�.");
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