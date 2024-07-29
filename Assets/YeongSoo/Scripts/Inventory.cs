using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private const float CELL_SIZE = 100f;

    public int width = 10; // �κ��丮�� �ʺ�
    public int height = 10; // �κ��丮�� ����
    public GameObject cellPrefab; // �� �������� �Ҵ��ϴ� ����
    public GameObject itemPrefab; // ������ �������� �Ҵ��ϴ� ����
    public Canvas mainCanvas;
    
    private InventoryCell[,] cells; // �κ��丮 ���� �����ϴ� 2���� �迭
    private Vector2 gridOffset; // �׸��尡 ������Ʈ�� �߾ӿ� �ΰ� �����ǵ��� ��ġ�� �������ִ� ����

    private void Awake()
    {
        instance = this;
    }

    private void Start()
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

    private void OnDestroy()
    {
        // ���� ���� �� ���� �κ��丮 �� ������ ���� �ڵ� ����
        List<ItemData> itemDataList = new List<ItemData>();

        foreach(InventoryCell inventoryCell in cells)
        {
            if(inventoryCell.occupyingItem)
                itemDataList.Add(inventoryCell.occupyingItem.GetItemData());
        }

        ItemLoader.SaveAllItems(itemDataList);
        Debug.Log($"�κ��丮 OnDestroy! �ڵ� ���� ����!");
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
                // �ش� ���� ������ ������Ʈ ��ġ & ������ ���� ����
                cells[x, y].inventoryCellDragHandler.OnDrop(inventoryItem);
                // ������ UI ���� �ʱ�ȭ
                inventoryItem.SetItemData(newItemData, mainCanvas, new Vector2(x,y));           
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

    public void AddNewItem(ItemData itemData)
    {
        // JSON�� ����
        ItemLoader.AddItem(itemData);

        // UI�� ǥ��
        InstantiateItem(itemData);
    }

    /// <summary>
    /// ���ο� ������ �߰��� ����ִ� ���� ã���ִ� �޼��� �Դϴ�. Ʃ�÷� ����� ������� ��ȯ���ݴϴ�. 
    /// </summary>
    /// <returns>true ����, false ����</returns>
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