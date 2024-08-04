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

    // 게시된 구글 시트의 url
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQLgdf4HJcBCjMIQLWNSTqchySCpzpHIArTWuIwHjYYCV1S4K_j5kDtZ9sp47hDLDPhyHF7D2nXoKdO/pub?output=tsv";

    // 구글 시트 데이터를 읽어올 때 쓰이는 변수들
    private int rowOffset = 5; // 각 아이템들이 5행씩 차지하기 때문에, 다음 아이템을 검색하기 위한 보정값
    private int itemShapeColumnOffset = 5; // 아이템 형태 칼럼은 다섯 줄이기 때문에, 전부 읽어오기 위한 보정값.

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

                // 아래와 같은 아이템의 기본정보는 첫 번째 줄에.
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

                // 아이템 형태 정보는 첫 번째 ~ 다섯번째 줄까지. 5x5사이즈로 존재. 
                // 아이템 형태 정보 읽어오기
                Debug.Log("Item Shape : ");
                for (int r = lineNum; r < lineNum + rowOffset; r++)
                {
                    if(r>=rows.Length)
                    {
                        Debug.Log($"row line num:{r} 데이터의 끝 입니다.");
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


                // 여기서 columns 배열의 각 요소를 처리합니다.
                // 예: Debug.Log(string.Join(", ", columns));

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