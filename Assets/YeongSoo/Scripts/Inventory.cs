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
    public GameObject bags; // 가방 아이템들의 부모 오브젝트

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
            GameObject itemObject = Instantiate(itemPrefab);

            if (itemObject.TryGetComponent<InventoryItem>(out InventoryItem inventoryItem))
            {
                // 아이템 정보 초기화
                inventoryItem.InitInventoryItem(newItemData, mainCanvas);

                // 가방과 일반 아이템들의 부모 오브젝트를 분리함으로써 이미지 레이어 분리
                if (inventoryItem.GetIsBag())
                    inventoryItem.transform.SetParent(Inventory.Instance.bags.transform);
                else
                    inventoryItem.transform.SetParent(Inventory.Instance.contents.transform);
                // 아이템 오브젝트 위치 초기화
                inventoryItem.transform.localPosition = cellArray[x, y].transform.localPosition;
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

    // 인벤토리 특정 셀에 아이템이 드랍되었을 때의 처리를 담당하는 메서드
    public void OnDrop(InventoryCell cell, InventoryItem droppedItem)
    {
        if(droppedItem == null) { return; }
        if(cell == null) { return; } 

        // 가방 여부를 확인 후 진행
        if(droppedItem.GetIsBag())
        {
            // 드랍된 셀에 가방이 없는 경우
            if (cell.GetOccupyingBag() == null)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // 해당 위치에 드랍
            }
            // 있는 가방이 droppedItem 자신일 경우
            else if (cell.GetOccupyingBag() == droppedItem)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // 해당 위치에 드랍
            }
            // 다른 가방이 있는 경우
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingBag(), droppedItem); // 위치 교체
            }
        }
        // droppedItem이 가방 이외의 아이템인 경우 처리
        else
        {
            // 드롭한 셀이 BagSlot이 아닌 경우 드랍 작업 취소
            if (!cell.GetIsBagSlot())
            {
                HandleInvalidDropLocation(droppedItem);
                return;
            }


            // 드롭한 셀에 다른 아이템이 없는 경우
            if(cell.GetOccupyingItem() == null)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // 해당 위치에 드랍
            }
            // 있는 아이템이 droppedItem 자신일 경우
            else if(cell.GetOccupyingItem() == droppedItem)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // 해당 위치에 드랍
            }
            // 다른 아이템이 있는 경우
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingItem(), droppedItem); // 위치 교체
            }
        }
    }

    // 빈 셀에 드롭했을 때 처리하는 메서드
    private void HandleEmptyCellDrop(InventoryCellDragHandler emptyCell, InventoryItem droppedItem)
    {
        if (droppedItem.GetIsBag())
        {
            // 기존 셀에 있는 occupied가방 정보 제거
            Inventory.Instance.GetInventoryCellByPos(droppedItem.GetItemData().currentCellPos).RemoveOccupyingBag();
        }
        else
        {
            // 기존 셀에 있는 occupied아이템 정보 제거
            Inventory.Instance.GetInventoryCellByPos(droppedItem.GetItemData().currentCellPos).RemoveOccupyingItem();
        }

        emptyCell.OnDrop(droppedItem);
    }

    // 이미 아이템이 있는 셀에 드롭했을 때 처리하는 메서드
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem, InventoryItem droppedItem)
    {
        if(occupyingItem == null || droppedItem == null) { return; }

        // 아이템 크기가 동일한 경우만 교환 처리
        if (occupyingItem.GetItemData().itemSpec.itemShape != droppedItem.GetItemData().itemSpec.itemShape)
        {
            Debug.Log($"아이템 모양이 다르기 때문에 위치 교환이 진행되지 않습니다");
            ReturnToOriginalPosition(droppedItem);
            return;
        }

        // 현재 아이템과 교환할 셀을 찾아서 아이템 교환 처리
        InventoryCellDragHandler occupiedCell = Inventory.Instance.GetInventoryCellByPos(occupyingItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        occupiedCell.OnSwapItems(droppedItem);
    }

    // 아이템을 원래 위치로 되돌리는 메서드
    private void ReturnToOriginalPosition(InventoryItem droppedItem)
    {
        droppedItem.transform.SetParent(droppedItem.originalParent);
        droppedItem.rectTransform.anchoredPosition = droppedItem.originalPosition;
    }

    // 드롭한 위치가 유효하지 않은 경우 처리하는 메서드
    private void HandleInvalidDropLocation(InventoryItem droppedItem)
    {
        // 유효하지 않은 드롭 위치에 대한 로그 출력
        Debug.Log($"Invalid drop location detected!");
        // 원래 위치로 돌아감
        ReturnToOriginalPosition(droppedItem);
    }

    // 아이템 점유 사실을 해당 영역 셀들에게 업데이트하는 메서드 입니다
    // 추후에 이 메서드 실행 전에 배치 가능 여부도 고려해줘야 합니다. 
    public void UpdateItemArea(InventoryItem inventoryItem)
    {
        // itemShapeArray 입장에선 [2,2]가 현 아이템의 중심. 
        // 따라서 드래그 드랍이 된 셀에서 (-2,-2) 이동한 위치가 itemShapeArray의 [0,0]. 오프셋 값이 됩니다.
        // itemShapeArray의 값이 1인 포지션의 셀들에 아이템 점유 설정을 해줘야 합니다. 0일 경우 점유 해제

        // 우선 현 셀의 pos를 얻어옵니다
        Vector2 currentCellPos = inventoryItem.GetItemData().currentCellPos;
        // 현 셀의 포지션과 itemShapeArray 인덱스 사이의 오프셋 값 구하기
        Vector2 offset = currentCellPos - new Vector2(2, 2);

        // 실제 인벤토리의 셀들에 아이템 점유 설정을 해주기.
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                // offset이 적용된 최종 포지션을 담을 Vector2 변수
                Vector2 actualCellPos = offset + new Vector2(x, y); // offest이 적용된 최정 포지션을 담습니다

                // 1일 경우 해당 셀에 점유 설정
                if (inventoryItem.GetItemShapeArray()[x, y] == '1')
                {
                    if (inventoryItem.GetIsBag())
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingBag(inventoryItem);
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(inventoryItem);
                }
                // 0일 경우 해당 셀에 점유 해제
                else
                {
                    if (inventoryItem.GetIsBag())
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingBag();
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingItem();
                }
            }
        }

        Debug.Log($"셀 그리드상 아이템 영역 업데이트 완료!");
    }
}