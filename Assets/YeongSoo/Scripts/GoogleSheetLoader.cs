using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class GoogleSheetLoader : MonoBehaviour
{
    private enum Columns
    {
        ItemID,
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

    // �Խõ� ���� ��Ʈ�� url
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQLgdf4HJcBCjMIQLWNSTqchySCpzpHIArTWuIwHjYYCV1S4K_j5kDtZ9sp47hDLDPhyHF7D2nXoKdO/pub?output=tsv";

    // ���� ��Ʈ �����͸� �о�� �� ���̴� ������
    private int rowOffset = 5; // �� �����۵��� 5�྿ �����ϱ� ������, ���� �������� �˻��ϱ� ���� ������
    private int itemShapeColumnOffset = 5; // ������ ���� Į���� �ټ� ���̱� ������, ���� �о���� ���� ������.

    private void Start()
    {
        StartCoroutine(LoadGoogleSheetData());
    }

    private IEnumerator LoadGoogleSheetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(googleSheetUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            string[] rows = www.downloadHandler.text.Split('\n');
            Debug.Log(rows[0]);
            for (int lineNum = 1; lineNum < rows.Length; lineNum += rowOffset)
            {
                //Debug.Log(rows[lineNum]);

                Debug.Log("======================================");

                // �Ʒ��� ���� �������� �⺻������ ù ��° �ٿ�.
                string[] columns = rows[lineNum].Split('\t');
                // Item ID
                Debug.Log($"Item ID : {columns[(int)Columns.ItemID]}");
                // Item Name
                Debug.Log($"Item Name : {columns[(int)Columns.ItemName]}");
                // Item Price
                Debug.Log($"Item Price : {columns[(int)Columns.ItemPrice]}");
                // Item Description
                Debug.Log($"Item Description : {columns[(int)Columns.ItemDescription]}");
                // Attack
                Debug.Log($"Attack : {columns[(int)Columns.Attack]}");
                // Defence
                Debug.Log($"Defence : {columns[(int)Columns.Defence]}");
                // Attack Speed
                Debug.Log($"Attack Speed : {columns[(int)Columns.AttackSpeed]}");
                // Healing Amount
                Debug.Log($"Healing Amount : {columns[(int)Columns.HealingAmount]}");
                // Item Type
                Debug.Log($"Item Type : {columns[(int)Columns.ItemType]}");
                // Type Main Stat
                Debug.Log($"Type Main Stat : {columns[(int)Columns.TypeMainStat]}");
                // Type Sub Stat
                Debug.Log($"Type Sub Stat : {columns[(int)Columns.TypeSubStat]}");

                // ������ ���� ������ ù ��° ~ �ټ���° �ٱ���. 5x5������� ����. 
                // ������ ���� ���� �о����
                Debug.Log("Item Shape : ");
                for (int r = lineNum; r < lineNum + rowOffset; r++)
                {
                    if(r>=rows.Length)
                    {
                        Debug.Log($"row line num:{r} �������� �� �Դϴ�.");
                        break;
                    }

                    string itemShape = "";
                    columns = rows[r].Split('\t');
                    for (int c = (int)Columns.ItemShape; c < (int)Columns.ItemShape + itemShapeColumnOffset; c++)
                    {
                        if (columns[c] == "")
                            itemShape += "-";
                        else
                            itemShape += columns[c];
                    }
                    Debug.Log(itemShape);
                }

                Debug.Log("======================================");


                // ���⼭ columns �迭�� �� ��Ҹ� ó���մϴ�.
                // ��: Debug.Log(string.Join(", ", columns));

                //columns = columns.Take((int)Columns.ItemShape).ToArray();
                //Debug.Log(string.Join(", ", columns));
            }


/*            foreach (string row in rows)
            {
                Debug.Log("======================================");
                string[] columns = row.Split('\t');

                foreach (string column in columns)
                {
                    Debug.Log(column);
                }
                Debug.Log("======================================");
            }*/

        }
    }
}