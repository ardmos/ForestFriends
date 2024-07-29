using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private const float CELL_SIZE = 100f;

    public int width = 10; // 인벤토리의 너비
    public int height = 10; // 인벤토리의 높이
    public GameObject cellPrefab; // 셀 프리팹을 할당하는 변수
    public GameObject itemPrefab; // 아이템 프리팹을 할당하는 변수
    public Canvas mainCanvas;
    
    private InventoryCell[,] cells; // 인벤토리 셀을 저장하는 2차원 배열
    private Vector2 gridOffset; // 그리드가 오브젝트를 중앙에 두고 형성되도록 위치를 보정해주는 변수

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 인벤토리 그리드와 셀 배열을 초기화
        cells = new InventoryCell[width, height];
        CalculateGridOffset();
        CreateGrid(); // 그리드 생성

        // 아이템 로드
        List<ItemData> itemDataList = ItemLoader.LoadItemsFromJson();

        Debug.Log($"itemDataList.Count {itemDataList.Count}");

        foreach (ItemData itemData in itemDataList)
        {
            InstantiateItem(itemData);
        }

    }

    private void OnDestroy()
    {
        // 씬이 닫힐 때 현재 인벤토리 내 아이템 정보 자동 저장
        List<ItemData> itemDataList = new List<ItemData>();

        foreach(InventoryCell inventoryCell in cells)
        {
            if(inventoryCell.occupyingItem)
                itemDataList.Add(inventoryCell.occupyingItem.GetItemData());
        }

        ItemLoader.SaveAllItems(itemDataList);
        Debug.Log($"인벤토리 OnDestroy! 자동 저장 시작!");
    }

    private void CalculateGridOffset()
    {
        float gridWidth = width * CELL_SIZE;
        float gridHeight = height * CELL_SIZE;
        gridOffset = new Vector2(-gridWidth / 2f + CELL_SIZE / 2f, gridHeight / 2f - CELL_SIZE / 2f);
    }

    // 인벤토리 그리드를 생성하는 메서드
    void CreateGrid()
    {
        // 그리드의 전체 크기 계산
        float gridWidth = width * CELL_SIZE; // 50은 그리드 셀의 크기
        float gridHeight = height * CELL_SIZE;

        // 그리드의 시작 위치 계산 (중앙에서 그리드의 절반 크기만큼 이동)
        Vector2 startPosition = new Vector2(-gridWidth / 2f + 25f, gridHeight / 2f - 25f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 셀 프리팹을 인스턴스화하고 위치 설정
                GameObject cellObject = Instantiate(cellPrefab, transform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = startPosition + new Vector2(x * CELL_SIZE, -y * CELL_SIZE);
                //Debug.Log($"그리드 셀을 그립니다. cell(x:{x},y{y}), anchoredPos : {cellObject.GetComponent<RectTransform>().anchoredPosition}");
                InventoryCell cell = cellObject.GetComponent<InventoryCell>();
                cells[x, y] = cell;
                cell.cellPos = new Vector2(x, y);
            }
        }
    }

    public void InstantiateItem(ItemData newItemData)
    {
        Debug.Log($"{newItemData.item.itemName}");
        int x = (int)newItemData.item.currentCellPos.x;
        int y = (int)newItemData.item.currentCellPos.y;

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            GameObject itemObject = Instantiate(itemPrefab, cells[x, y].transform);

            if (itemObject.TryGetComponent<InventoryItem>(out InventoryItem inventoryItem))
            {
                // 해당 셀에 아이템 오브젝트 배치 & 아이템 정보 저장
                cells[x, y].inventoryCellDragHandler.OnDrop(inventoryItem);
                // 아이템 UI 정보 초기화
                inventoryItem.SetItemData(newItemData, mainCanvas, new Vector2(x,y));           
            }

            // InventoryItem 컴포넌트가 있다면 추가 설정을 할 수 있습니다.
/*            InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                // 필요한 경우 InventoryItem에 추가 설정
                // 예: inventoryItem.Initialize(newItem);
            }*/
        }
        else
        {
            Debug.Log("Inventory.InstantiateItem 올바른 인벤토리 Pos가 아닙니다.");
        }
    }

    public void AddNewItem(ItemData itemData)
    {
        // JSON에 저장
        ItemLoader.AddItem(itemData);

        // UI에 표현
        InstantiateItem(itemData);
    }

    /// <summary>
    /// 새로운 아이템 추가시 비어있는 셀을 찾아주는 메서드 입니다. 튜플로 결과를 구분지어서 반환해줍니다. 
    /// </summary>
    /// <returns>true 성공, false 실패</returns>
    public (bool success, Vector2 cellPosition) GetEmptyInventoryCellPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x,y].occupyingItem == null)
                {
                    return (true, new Vector2(x,y));
                }
            }
        }

        return (false, Vector2.zero);
    }

    public InventoryCell GetInventoryCellByPos(Vector2 cellPos)
    {
        return cells[(int)cellPos.x, (int)cellPos.y];
    }
}