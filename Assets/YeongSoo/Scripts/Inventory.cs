using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 인벤토리의 전반적 기능을 담당하는 스크립트
/// </summary>
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public const float CELL_SIZE = 100f;

    public int widthCellCount; // 인벤토리의 너비
    public int heightCellCount; // 인벤토리의 높이
    public GameObject cellPrefab; // 셀 프리팹을 할당하는 변수
    public GameObject itemPrefab; // 아이템 프리팹을 할당하는 변수
    public Canvas mainCanvas;
    public GameObject cells; // 셀들의 부모 오브젝트
    public GameObject contents; // 아이템 컨텐츠들의 부모 오브젝트

    private InventoryCell[,] cellArray; // 인벤토리 셀을 저장하는 2차원 배열
    private Vector2 gridOffset; // 그리드가 오브젝트를 중앙에 두고 형성되도록 위치를 보정해주는 변수
    private List<ItemData> playerItems;

    private void Awake()
    {
        Instance = this;
        playerItems = new List<ItemData>();
    }

    private async void Start()
    {
        // 인벤토리 셀 배열을 초기화
        cellArray = new InventoryCell[widthCellCount, heightCellCount];
        // 그리드 생성 
        CreateGrid(); 
        // 1. 구글시트에서 아이템 스펙 정보를 로드한다.  <실제로는 게임 시작시 로드해야함>
        await ItemSpecManager.LoadItemSpecs();
        // 2. 로드한 정보를 바탕으로 아이템을 로드한다.
        LoadItems(); // 아이템 로드
    }

    private void OnDestroy()
    {
        // 씬이 닫힐 때 현재 인벤토리 내 아이템 정보 자동 저장
        List<ItemData> itemDataList = new List<ItemData>();

        foreach (InventoryCell inventoryCell in cellArray)
        {
            // 점유된 cell일 경우, 해당 cell을 점유중인 아이템의 중심 포지션에 있는 cell만 저장합니다
            if (inventoryCell.GetOccupyingItem() && inventoryCell.cellPos == inventoryCell.GetOccupyingItem().GetItemData().currentCellPos)
                itemDataList.Add(inventoryCell.GetOccupyingItem().GetItemData());
        }

        //Debug.Log($"인벤토리 OnDestroy! 자동 저장 시작!");
        ItemDataManager.UpdateItemDataListToJson(itemDataList);
        //Debug.Log($"인벤토리 OnDestroy! 자동 저장 완료");
    }

    // 저장된 아이템 리스트를 로드해서 인벤토리상에 배치하는 메서드
    private void LoadItems()
    {
        try
        {
            playerItems = ItemDataManager.LoadItemsFromJson();

            //Debug.Log($"playerItems.Count {playerItems.Count}");

            foreach (ItemData itemData in playerItems)
            {
                //Debug.Log(itemData.itemSpec.itemName);
                InstantiateItem(itemData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading items: {e.Message}");
        }
    }

    // 그리드의 위치 오프셋을 계산하는 메서드
    private void CalculateGridOffset()
    {
        // 그리드의 전체 크기 계산
        float gridWidth = widthCellCount * CELL_SIZE;
        float gridHeight = heightCellCount * CELL_SIZE;
        // 그리드의 시작 위치 계산 (중앙에서 그리드의 절반 크기만큼 이동)
        gridOffset = new Vector2(-gridWidth / 2f + CELL_SIZE / 2, gridHeight / 2f - CELL_SIZE / 2);
    }

    // 인벤토리 그리드를 생성하는 메서드
    private void CreateGrid()
    {
        CalculateGridOffset(); // 그리드 오프셋 계산

        for (int x = 0; x < widthCellCount; x++)
        {
            for (int y = 0; y < heightCellCount; y++)
            {
                // 셀 프리팹을 인스턴스화하고 위치 설정
                GameObject cellObject = Instantiate(cellPrefab, cells.transform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = gridOffset + new Vector2(x * CELL_SIZE, -y * CELL_SIZE);
                //Debug.Log($"그리드 셀을 그립니다. cell(x:{x},y{y}), anchoredPos : {cellObject.GetComponent<RectTransform>().anchoredPosition}");
                InventoryCell cell = cellObject.GetComponent<InventoryCell>();
                cellArray[x, y] = cell;
                cell.cellPos = new Vector2(x, y);
            }
        }
    }

    public void InstantiateItem(ItemData newItemData)
    {
        Debug.Log($"InstantiateItem. itemName:{newItemData.itemSpec.itemName}, cellPos:{newItemData.currentCellPos}");
        int x = (int)newItemData.currentCellPos.x;
        int y = (int)newItemData.currentCellPos.y;

        if (x >= 0 && x < widthCellCount && y >= 0 && y < heightCellCount)
        {
            GameObject itemObject = Instantiate(itemPrefab, cellArray[x, y].transform);

            if (itemObject.TryGetComponent<InventoryItem>(out InventoryItem inventoryItem))
            {
                // 아이템 정보 초기화
                inventoryItem.InitInventoryItem(newItemData, mainCanvas);
                // 해당 셀에 아이템 오브젝트 배치 & 아이템 정보 저장
                cellArray[x, y].inventoryCellDragHandler.SetItemOnCurrentCell(inventoryItem);
            }
        }
        else
        {
            Debug.Log("Inventory.InstantiateItem 올바른 인벤토리 Pos가 아닙니다.");
        }
    }

    /// <summary>
    /// 새로운 아이템 추가시 비어있는 셀을 찾아주는 메서드 입니다. 튜플로 결과를 구분지어서 반환해줍니다. 
    /// </summary>
    /// <returns>true 성공, false 실패</returns>
    public (bool success, Vector2 cellPosition) GetEmptyInventoryCellPos()
    {
        for (int y = 0; y < heightCellCount; y++)
        {
            for (int x = 0; x < widthCellCount; x++)
            {
                if (cellArray[x, y].GetOccupyingItem() == null)
                {
                    return (true, new Vector2(x, y));
                }
            }
        }

        return (false, Vector2.zero);
    }

    // 파라미터로 전달받은 위치의 셀 정보를 반환하는 메서드
    public InventoryCell GetInventoryCellByPos(Vector2 cellPos)
    {
        // 유효한 인덱스인지 확인
        if (cellPos.x < 0 || cellPos.x >= cellArray.GetLength(0) || cellPos.y < 0 || cellPos.y >= cellArray.GetLength(1))
        {
            // 유효하지 않은 경우 null 반환
            return null;
        }

        // 유효한 경우 셀 반환
        return cellArray[(int)cellPos.x, (int)cellPos.y];
    }
}