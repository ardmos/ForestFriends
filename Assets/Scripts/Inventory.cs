using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const float CELL_SIZE = 100f;

    public int width = 10; // �κ��丮�� �ʺ�
    public int height = 10; // �κ��丮�� ����
    public GameObject cellPrefab; // �� �������� �Ҵ��ϴ� ����
    public GameObject itemPrefab; // ������ �������� �Ҵ��ϴ� ����
    public Canvas mainCanvas;
    
    private InventoryCell[,] cells; // �κ��丮 ���� �����ϴ� 2���� �迭
    private Vector2 gridOffset; // �׸��尡 ������Ʈ�� �߾ӿ� �ΰ� �����ǵ��� ��ġ�� �������ִ� ����


    void Start()
    {
        // �κ��丮 �׸���� �� �迭�� �ʱ�ȭ
        cells = new InventoryCell[width, height];
        CalculateGridOffset();
        CreateGrid(); // �׸��� ����

        // ������ �ε�
        List<ItemData> itemDataList = ItemLoader.LoadItemsFromJson();

        Debug.Log($"itemDataList.Count {itemDataList.Count}");

        foreach (ItemData itemData in itemDataList)
        {
            InstantiateItem(itemData);
        }

    }

    private void CalculateGridOffset()
    {
        float gridWidth = width * CELL_SIZE;
        float gridHeight = height * CELL_SIZE;
        gridOffset = new Vector2(-gridWidth / 2f + CELL_SIZE / 2f, gridHeight / 2f - CELL_SIZE / 2f);
    }

    // �κ��丮 �׸��带 �����ϴ� �޼���
    void CreateGrid()
    {
        // �׸����� ��ü ũ�� ���
        float gridWidth = width * CELL_SIZE; // 50�� �׸��� ���� ũ��
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
                cells[x, y] = cellObject.GetComponent<InventoryCell>();
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
            // ���� ������ �Ҵ�
            cells[x, y].itemData = newItemData;

            // ������ �������� �ν��Ͻ�ȭ�ϰ� ���� ��ġ
            GameObject itemObject = Instantiate(itemPrefab, cells[x, y].transform);
            itemObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            // ������ UI ���� �ʱ�ȭ
            if (itemObject.TryGetComponent<InventoryItem>(out InventoryItem inventoryItem))
            {
                inventoryItem.SetItemData(newItemData, mainCanvas);
            }

            // InventoryItem ������Ʈ�� �ִٸ� �߰� ������ �� �� �ֽ��ϴ�.
/*            InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                // �ʿ��� ��� InventoryItem�� �߰� ����
                // ��: inventoryItem.Initialize(newItem);
            }*/
        }
        else
        {
            Debug.Log("Inventory.InstantiateItem �ùٸ� �κ��丮 Pos�� �ƴմϴ�.");
        }
    }

    public void ReplaceItem(ItemData moveItemData)
    {
        int targetCellPosX = (int)moveItemData.item.targetCellPos.x;
        int targetCellPosY = (int)moveItemData.item.targetCellPos.y;

        ItemData existingItemData = cells[targetCellPosX, targetCellPosY].itemData;

        // ���� �̹� �������� ���� ���. ��ġ ��ȯ
        if (existingItemData != null)
        {
            existingItemData = new ItemData()
            {
                item = new Item()
                {
                    itemName = existingItemData.item.itemName,
                    currentCellPos = moveItemData.item.currentCellPos,
                    targetCellPos = moveItemData.item.currentCellPos
                }
            };
        }

        // ���ο� ������ ���� ��ġ
        InstantiateItem(moveItemData);

        // ���� �������� �־��� ���
        if(existingItemData != null) 
        {
            InstantiateItem(existingItemData);
        }
    }

    public void AddNewItem(ItemData itemData)
    {
        // JSON�� ����
        ItemLoader.AddItem(itemData);

        // UI�� ǥ��
        InstantiateItem(itemData);
    }

    /// <summary>
    /// Ʃ�÷� ����� ������� ��ȯ.
    /// </summary>
    /// <returns></returns>
    public (bool success, Vector2 cellPosition) GetEmptyInventoryCellPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x,y].itemData == null)
                {
                    return (true, new Vector2(x,y));
                }
            }
        }

        return (false, Vector2.zero);
    }
}