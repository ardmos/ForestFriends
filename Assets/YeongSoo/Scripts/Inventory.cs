using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// �κ��丮�� ������ ����� ����ϴ� ��ũ��Ʈ
/// </summary>
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public const float CELL_SIZE = 100f;

    public int widthCellCount; // �κ��丮�� �ʺ�
    public int heightCellCount; // �κ��丮�� ����
    public GameObject cellPrefab; // �� �������� �Ҵ��ϴ� ����
    public GameObject itemPrefab; // ������ �������� �Ҵ��ϴ� ����
    public Canvas mainCanvas;
    public GameObject cells; // ������ �θ� ������Ʈ
    public GameObject contents; // ������ ���������� �θ� ������Ʈ
    public GameObject bags; // ���� �����۵��� �θ� ������Ʈ

    private InventoryCell[,] cellArray; // �κ��丮 ���� �����ϴ� 2���� �迭
    private Vector2 gridOffset; // �׸��尡 ������Ʈ�� �߾ӿ� �ΰ� �����ǵ��� ��ġ�� �������ִ� ����
    private List<ItemData> playerItems;

    private void Awake()
    {
        Instance = this;
        playerItems = new List<ItemData>();
    }

    private async void Start()
    {
        // �κ��丮 �� �迭�� �ʱ�ȭ
        cellArray = new InventoryCell[widthCellCount, heightCellCount];
        // �׸��� ���� 
        CreateGrid(); 
        // 1. ���۽�Ʈ���� ������ ���� ������ �ε��Ѵ�.  <�����δ� ���� ���۽� �ε��ؾ���>
        await ItemSpecManager.LoadItemSpecs();
        // 2. �ε��� ������ �������� �������� �ε��Ѵ�.
        LoadItems(); // ������ �ε�
    }

    private void OnDestroy()
    {
        // ���� ���� �� ���� �κ��丮 �� ������ ���� �ڵ� ����
        List<ItemData> itemDataList = new List<ItemData>();

        foreach (InventoryCell inventoryCell in cellArray)
        {
            // ������ cell�� ���, �ش� cell�� �������� �������� �߽� �����ǿ� �ִ� cell�� �����մϴ�
            if (inventoryCell.GetOccupyingItem() && inventoryCell.cellPos == inventoryCell.GetOccupyingItem().GetItemData().currentCellPos)
                itemDataList.Add(inventoryCell.GetOccupyingItem().GetItemData());
        }

        //Debug.Log($"�κ��丮 OnDestroy! �ڵ� ���� ����!");
        ItemDataManager.UpdateItemDataListToJson(itemDataList);
        //Debug.Log($"�κ��丮 OnDestroy! �ڵ� ���� �Ϸ�");
    }

    // ����� ������ ����Ʈ�� �ε��ؼ� �κ��丮�� ��ġ�ϴ� �޼���
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

    // �׸����� ��ġ �������� ����ϴ� �޼���
    private void CalculateGridOffset()
    {
        // �׸����� ��ü ũ�� ���
        float gridWidth = widthCellCount * CELL_SIZE;
        float gridHeight = heightCellCount * CELL_SIZE;
        // �׸����� ���� ��ġ ��� (�߾ӿ��� �׸����� ���� ũ�⸸ŭ �̵�)
        gridOffset = new Vector2(-gridWidth / 2f + CELL_SIZE / 2, gridHeight / 2f - CELL_SIZE / 2);
    }

    // �κ��丮 �׸��带 �����ϴ� �޼���
    private void CreateGrid()
    {
        CalculateGridOffset(); // �׸��� ������ ���

        for (int x = 0; x < widthCellCount; x++)
        {
            for (int y = 0; y < heightCellCount; y++)
            {
                // �� �������� �ν��Ͻ�ȭ�ϰ� ��ġ ����
                GameObject cellObject = Instantiate(cellPrefab, cells.transform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = gridOffset + new Vector2(x * CELL_SIZE, -y * CELL_SIZE);
                //Debug.Log($"�׸��� ���� �׸��ϴ�. cell(x:{x},y{y}), anchoredPos : {cellObject.GetComponent<RectTransform>().anchoredPosition}");
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
                // ������ ���� �ʱ�ȭ
                inventoryItem.InitInventoryItem(newItemData, mainCanvas);

                // ����� �Ϲ� �����۵��� �θ� ������Ʈ�� �и������ν� �̹��� ���̾� �и�
                if (inventoryItem.GetIsBag())
                    inventoryItem.transform.SetParent(Inventory.Instance.bags.transform);
                else
                    inventoryItem.transform.SetParent(Inventory.Instance.contents.transform);
                // ������ ������Ʈ ��ġ �ʱ�ȭ
                inventoryItem.transform.localPosition = cellArray[x, y].transform.localPosition;
            }
        }
        else
        {
            Debug.Log("Inventory.InstantiateItem �ùٸ� �κ��丮 Pos�� �ƴմϴ�.");
        }
    }

    /// <summary>
    /// ���ο� ������ �߰��� ����ִ� ���� ã���ִ� �޼��� �Դϴ�. Ʃ�÷� ����� ������� ��ȯ���ݴϴ�. 
    /// </summary>
    /// <returns>true ����, false ����</returns>
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

    // �Ķ���ͷ� ���޹��� ��ġ�� �� ������ ��ȯ�ϴ� �޼���
    public InventoryCell GetInventoryCellByPos(Vector2 cellPos)
    {
        // ��ȿ�� �ε������� Ȯ��
        if (cellPos.x < 0 || cellPos.x >= cellArray.GetLength(0) || cellPos.y < 0 || cellPos.y >= cellArray.GetLength(1))
        {
            // ��ȿ���� ���� ��� null ��ȯ
            return null;
        }

        // ��ȿ�� ��� �� ��ȯ
        return cellArray[(int)cellPos.x, (int)cellPos.y];
    }

    // �κ��丮 Ư�� ���� �������� ����Ǿ��� ���� ó���� ����ϴ� �޼���
    public void OnDrop(InventoryCell cell, InventoryItem droppedItem)
    {
        if(droppedItem == null) { return; }
        if(cell == null) { return; } 

        // ���� ���θ� Ȯ�� �� ����
        if(droppedItem.GetIsBag())
        {
            // ����� ���� ������ ���� ���
            if (cell.GetOccupyingBag() == null)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // �ش� ��ġ�� ���
            }
            // �ִ� ������ droppedItem �ڽ��� ���
            else if (cell.GetOccupyingBag() == droppedItem)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // �ش� ��ġ�� ���
            }
            // �ٸ� ������ �ִ� ���
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingBag(), droppedItem); // ��ġ ��ü
            }
        }
        // droppedItem�� ���� �̿��� �������� ��� ó��
        else
        {
            // ����� ���� BagSlot�� �ƴ� ��� ��� �۾� ���
            if (!cell.GetIsBagSlot())
            {
                HandleInvalidDropLocation(droppedItem);
                return;
            }


            // ����� ���� �ٸ� �������� ���� ���
            if(cell.GetOccupyingItem() == null)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // �ش� ��ġ�� ���
            }
            // �ִ� �������� droppedItem �ڽ��� ���
            else if(cell.GetOccupyingItem() == droppedItem)
            {
                HandleEmptyCellDrop(cell.inventoryCellDragHandler, droppedItem); // �ش� ��ġ�� ���
            }
            // �ٸ� �������� �ִ� ���
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingItem(), droppedItem); // ��ġ ��ü
            }
        }
    }

    // �� ���� ������� �� ó���ϴ� �޼���
    private void HandleEmptyCellDrop(InventoryCellDragHandler emptyCell, InventoryItem droppedItem)
    {
        if (droppedItem.GetIsBag())
        {
            // ���� ���� �ִ� occupied���� ���� ����
            Inventory.Instance.GetInventoryCellByPos(droppedItem.GetItemData().currentCellPos).RemoveOccupyingBag();
        }
        else
        {
            // ���� ���� �ִ� occupied������ ���� ����
            Inventory.Instance.GetInventoryCellByPos(droppedItem.GetItemData().currentCellPos).RemoveOccupyingItem();
        }

        emptyCell.OnDrop(droppedItem);
    }

    // �̹� �������� �ִ� ���� ������� �� ó���ϴ� �޼���
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem, InventoryItem droppedItem)
    {
        if(occupyingItem == null || droppedItem == null) { return; }

        // ������ ũ�Ⱑ ������ ��츸 ��ȯ ó��
        if (occupyingItem.GetItemData().itemSpec.itemShape != droppedItem.GetItemData().itemSpec.itemShape)
        {
            Debug.Log($"������ ����� �ٸ��� ������ ��ġ ��ȯ�� ������� �ʽ��ϴ�");
            ReturnToOriginalPosition(droppedItem);
            return;
        }

        // ���� �����۰� ��ȯ�� ���� ã�Ƽ� ������ ��ȯ ó��
        InventoryCellDragHandler occupiedCell = Inventory.Instance.GetInventoryCellByPos(occupyingItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        occupiedCell.OnSwapItems(droppedItem);
    }

    // �������� ���� ��ġ�� �ǵ����� �޼���
    private void ReturnToOriginalPosition(InventoryItem droppedItem)
    {
        droppedItem.transform.SetParent(droppedItem.originalParent);
        droppedItem.rectTransform.anchoredPosition = droppedItem.originalPosition;
    }

    // ����� ��ġ�� ��ȿ���� ���� ��� ó���ϴ� �޼���
    private void HandleInvalidDropLocation(InventoryItem droppedItem)
    {
        // ��ȿ���� ���� ��� ��ġ�� ���� �α� ���
        Debug.Log($"Invalid drop location detected!");
        // ���� ��ġ�� ���ư�
        ReturnToOriginalPosition(droppedItem);
    }

    // ������ ���� ����� �ش� ���� ���鿡�� ������Ʈ�ϴ� �޼��� �Դϴ�
    // ���Ŀ� �� �޼��� ���� ���� ��ġ ���� ���ε� �������� �մϴ�. 
    public void UpdateItemArea(InventoryItem inventoryItem)
    {
        // itemShapeArray ���忡�� [2,2]�� �� �������� �߽�. 
        // ���� �巡�� ����� �� ������ (-2,-2) �̵��� ��ġ�� itemShapeArray�� [0,0]. ������ ���� �˴ϴ�.
        // itemShapeArray�� ���� 1�� �������� ���鿡 ������ ���� ������ ����� �մϴ�. 0�� ��� ���� ����

        // �켱 �� ���� pos�� ���ɴϴ�
        Vector2 currentCellPos = inventoryItem.GetItemData().currentCellPos;
        // �� ���� �����ǰ� itemShapeArray �ε��� ������ ������ �� ���ϱ�
        Vector2 offset = currentCellPos - new Vector2(2, 2);

        // ���� �κ��丮�� ���鿡 ������ ���� ������ ���ֱ�.
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                // offset�� ����� ���� �������� ���� Vector2 ����
                Vector2 actualCellPos = offset + new Vector2(x, y); // offest�� ����� ���� �������� ����ϴ�

                // 1�� ��� �ش� ���� ���� ����
                if (inventoryItem.GetItemShapeArray()[x, y] == '1')
                {
                    if (inventoryItem.GetIsBag())
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingBag(inventoryItem);
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(inventoryItem);
                }
                // 0�� ��� �ش� ���� ���� ����
                else
                {
                    if (inventoryItem.GetIsBag())
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingBag();
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingItem();
                }
            }
        }

        Debug.Log($"�� �׸���� ������ ���� ������Ʈ �Ϸ�!");
    }
}