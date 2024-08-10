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
    private const float CELL_SIZE = 100f;

    public int width; // �κ��丮�� �ʺ�
    public int height; // �κ��丮�� ����
    public GameObject cellPrefab; // �� �������� �Ҵ��ϴ� ����
    public GameObject itemPrefab; // ������ �������� �Ҵ��ϴ� ����
    public Canvas mainCanvas;

    private InventoryCell[,] cells; // �κ��丮 ���� �����ϴ� 2���� �迭
    private Vector2 gridOffset; // �׸��尡 ������Ʈ�� �߾ӿ� �ΰ� �����ǵ��� ��ġ�� �������ִ� ����
    private List<ItemData> playerItems;

    private void Awake()
    {
        Instance = this;
        playerItems = new List<ItemData>();
    }

    private async void Start()
    {
        // �κ��丮 �׸���� �� �迭�� �ʱ�ȭ
        cells = new InventoryCell[width, height];
        CalculateGridOffset(); // �׸��� ������ ���
        CreateGrid(); // �׸��� ���� 

        // 1. ���۽�Ʈ���� ������ ���� ������ �ε��Ѵ�.  <�����δ� ���� ���۽� �ε��ؾ���>
        await ItemSpecManager.LoadItemSpecs();

        // 2. �ε��� ������ �������� �������� �ε��Ѵ�.
        LoadItems(); // ������ �ε�
    }

    private void OnDestroy()
    {
        // ���� ���� �� ���� �κ��丮 �� ������ ���� �ڵ� ����
        List<ItemData> itemDataList = new List<ItemData>();

        foreach (InventoryCell inventoryCell in cells)
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

            Debug.Log($"playerItems.Count {playerItems.Count}");

            foreach (ItemData itemData in playerItems)
            {
                Debug.Log(itemData.itemSpec.itemName);
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
        float gridWidth = width * CELL_SIZE;
        float gridHeight = height * CELL_SIZE;
        gridOffset = new Vector2(-gridWidth / 2f + CELL_SIZE / 2f, gridHeight / 2f - CELL_SIZE / 2f);
    }

    // �κ��丮 �׸��带 �����ϴ� �޼���
    private void CreateGrid()
    {
        // �׸����� ��ü ũ�� ���
        float gridWidth = width * CELL_SIZE;
        float gridHeight = height * CELL_SIZE;

        // �׸����� ���� ��ġ ��� (�߾ӿ��� �׸����� ���� ũ�⸸ŭ �̵�)
        Vector2 startPosition = new Vector2(-gridWidth / 2f + 25f, gridHeight / 2f - 25f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // �� �������� �ν��Ͻ�ȭ�ϰ� ��ġ ����
                GameObject cellObject = Instantiate(cellPrefab, transform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = startPosition + new Vector2(x * CELL_SIZE, -y * CELL_SIZE);
                //Debug.Log($"�׸��� ���� �׸��ϴ�. cell(x:{x},y{y}), anchoredPos : {cellObject.GetComponent<RectTransform>().anchoredPosition}");
                InventoryCell cell = cellObject.GetComponent<InventoryCell>();
                cells[x, y] = cell;
                cell.cellPos = new Vector2(x, y);
            }
        }
    }

    public void InstantiateItem(ItemData newItemData)
    {
        Debug.Log($"InstantiateItem. itemName:{newItemData.itemSpec.itemName}, cellPos:{newItemData.currentCellPos}");
        int x = (int)newItemData.currentCellPos.x;
        int y = (int)newItemData.currentCellPos.y;

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            GameObject itemObject = Instantiate(itemPrefab, cells[x, y].transform);

            if (itemObject.TryGetComponent<InventoryItem>(out InventoryItem inventoryItem))
            {
                // ������ ���� �ʱ�ȭ
                inventoryItem.InitInventoryItem(newItemData, mainCanvas);
                // �ش� ���� ������ ������Ʈ ��ġ & ������ ���� ����
                cells[x, y].inventoryCellDragHandler.SetItemOnCurrentCell(inventoryItem);
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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cells[x, y].GetOccupyingItem() == null)
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
        if (cellPos.x < 0 || cellPos.x >= cells.GetLength(0) || cellPos.y < 0 || cellPos.y >= cells.GetLength(1))
        {
            // ��ȿ���� ���� ��� null ��ȯ
            return null;
        }

        // ��ȿ�� ��� �� ��ȯ
        return cells[(int)cellPos.x, (int)cellPos.y];
    }
}