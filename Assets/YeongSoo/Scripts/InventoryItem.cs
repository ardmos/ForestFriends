using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public GameObject touchAreas;
    public GameObject touchAreaPrefab;

    [SerializeField] private bool isBag;
    private ItemData itemData;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas mainCanvas;
    // 5x5 �迭 ���·� ������ ������ ���� ������ ���� ����
    private char[,] itemShapeArray;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false; // �巡�� ���� �� �ٸ� ���� ��ȣ�ۿ��� �������� �ʵ��� ����

        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling(); // �θ� ������Ʈ ���� Hiearachy�� ���� �Ʒ��� �̵�
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCanvas == null)
            return;

        Vector2 localPoint;
        // ���콺�� ȭ�� ��ǥ�� ĵ������ ���� ��ǥ�� ��ȯ
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // ���콺 �̵��� ���� ������ �̵�
            // ���� ��ǥ�� UI ����� ��ġ�� ����
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�װ� ������ �ٽ� ����ĳ��Ʈ�� ������ �� �ֵ��� ����
        canvasGroup.blocksRaycasts = true;

        // ����� ��ġ�� �� ������ ���
        if (eventData.pointerEnter == null)
        {
            HandleInvalidDropLocation();
            return;
        }


        // ���콺 ��ġ�� ����ĳ��Ʈ �߻�
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        InventoryCellDragHandler cellDragHandler = null;
        InventoryCell cell = null;

        foreach (RaycastResult result in results)
        {
            if(eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
            {
                cellDragHandler = inventoryCellDragHandler;
                cell = inventoryCellDragHandler.inventoryCell;
            }
        }

        if(cell == null)
        {
            Debug.Log("�� ��� ��ġ�� ���� �˻����� �ʾҽ��ϴ�.");
            HandleInvalidDropLocation();
            return;
        }

        // ���� ���� ������ ��ġ. ��� ������ �߰� ����
/*
        // �� �������� ������ ���
        if (isBag)
        {
            // �������� ������ �ڽ��� ��� ���õ� ���� �߽� �̵� ( ���� �̵��Ұ����� ���͸��� ���� �߰� �ʿ�)
            if (cell.GetOccupyingBag() == this)
            {
                Debug.Log("�� ���� �ڽ��Դϴ�.");
                HandleEmptyCellDrop(cellDragHandler);
            }
            // �������� ������ ���� ���
            else if (cell.GetOccupyingBag() == null)
            {
                HandleEmptyCellDrop(cellDragHandler);
            }
            // �������� ������ �ִ� ���
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingBag());
            }
        }
        // ���� �̿��� �������� ���
        else
        {
            // ����� ��ġ�� ������ �������� Ȯ��
            if (cell.GetIsBagSlot())
            {
                // �������� �������� �� �������� ��� ����� ���� ��ġ�� �߽� �̵�
                if (cell.GetOccupyingItem() == this)        
                {
                    Debug.Log("�� ������ �ڽ��Դϴ�.");
                    HandleEmptyCellDrop(cellDragHandler);
                }
                // ����ִ� ���� ������ ���
                else if (cell.GetOccupyingItem() == null)
                {
                    HandleEmptyCellDrop(cellDragHandler);
                }
                // �� ������ �̿��� �������� �������� �ִ� ���
                else
                {
                    HandleOccupiedCellDrop(cell.GetOccupyingItem());
                }
            }
            // ����� ��ġ�� �� ������ �ƴ�����, �������� ��ġ�� �� ���� ������ ��� ���
            else
            {
                Debug.Log($"�������� ���� ��ġ: {eventData.pointerEnter.gameObject.name}");
                HandleInvalidDropLocation();
            }
        }*/
    }

    // �������� ���� ��ġ�� �ǵ����� �޼���
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    // �� ���� ������� �� ó���ϴ� �޼���
    private void HandleEmptyCellDrop(InventoryCellDragHandler inventoryCellDragHandler)
    {
        inventoryCellDragHandler.OnDrop(this);
    }

    // �̹� �������� �ִ� ���� ������� �� ó���ϴ� �޼���
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem)
    {
        // ������ ũ�Ⱑ ������ ��츸 ��ȯ ó��
        if (occupyingItem.GetItemData().itemSpec.itemShape != itemData.itemSpec.itemShape)
        {
            Debug.Log($"������ ����� �ٸ��� ������ ��ġ ��ȯ�� ������� �ʽ��ϴ�");
            ReturnToOriginalPosition();
            return;
        }

        // ���� �����۰� ��ȯ�� ���� ã�Ƽ� ������ ��ȯ ó��
        InventoryCellDragHandler occupiedCell = Inventory.Instance.GetInventoryCellByPos(occupyingItem.itemData.currentCellPos).inventoryCellDragHandler;
        occupiedCell.OnSwapItems(this);
    }

    // ����� ��ġ�� ��ȿ���� ���� ��� ó���ϴ� �޼���
    private void HandleInvalidDropLocation()
    {
        // ��ȿ���� ���� ��� ��ġ�� ���� �α� ���
        Debug.Log($"Invalid drop location detected!");
        // ���� ��ġ�� ���ư�
        ReturnToOriginalPosition();
    }

    // ������ ���� �ʱ�ȭ
    public void InitInventoryItem(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        isBag = itemData.itemSpec.sheetName == GoogleSheetLoader.Sheets.BAG;
        InitItemShapeArrayData();
        InitItemImage();
        InitTouchArea();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    public bool GetIsBag()
    {
        return isBag;
    }

    public void SetNewCurrentCellPos(Vector2 newPos)
    {
        itemData.currentCellPos = newPos;
    }

    /// <summary>
    /// itemSpec�� ��� itemShape �����͸� 5x5 �迭�� ��ȯ�� �ʱ�ȭ�ϴ� �޼����Դϴ�.
    /// </summary>
    private void InitItemShapeArrayData()
    {
        // 5x5 ������ �׸��� �����Ͱ� �Ϸķ� ��� ���ڿ� ������
        string shapeData = itemData.itemSpec.itemShape;

        // 5x5 �迭 ���·� ������ ������ ���� ������ ���� ����
        itemShapeArray = new char[5, 5];
        int chunkSize = 5;

        // ���ڿ��� 5x5 �迭�� ��ȯ
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                // �� ���� ���� �ε����� i * chunkSize
                // �� �ε����� j
                itemShapeArray[j, i] = shapeData[i * chunkSize + j];
            }
        }
    }

    // �������� �̹����� �ʱ�ȭ�ϴ� �޼��� �Դϴ�
    private void InitItemImage()
    {
        itemImage.sprite = GameAssetManager.Instance.GetItemImageBySpecID(itemData.itemSpec.sheetName, itemData.itemSpec.itemSpecID);
        // ���� �߰� ���.  ������� ��.
    }

    // ������ ���� ����� �ش� ���� ���鿡�� ������Ʈ�ϴ� �޼��� �Դϴ�
    public void UpdateItemArea()
    {
        // itemShapeArray ���忡�� [2,2]�� �� �������� �߽�. 
        // ���� �巡�� ����� �� ������ (-2,-2) �̵��� ��ġ�� itemShapeArray�� [0,0]. ������ ���� �˴ϴ�.
        // itemShapeArray�� ���� 1�� �������� ���鿡 ������ ���� ������ ����� �մϴ�. 0�� ��� ���� ����

        // �켱 �� ���� pos�� ���ɴϴ�
        Vector2 currentCellPos = itemData.currentCellPos;
        // �� ���� �����ǰ� itemShapeArray �ε��� ������ ������ �� ���ϱ�
        Vector2 offset = currentCellPos - new Vector2(2, 2);

        // ���� �κ��丮�� ���鿡 ������ ���� ������ ���ֱ�.
        for(int x = 0; x < 5; x++)
        {
            for(int y = 0; y < 5; y++)
            {
                // offset�� ����� ���� �������� ���� Vector2 ����
                Vector2 actualCellPos = offset + new Vector2(x, y); // offest�� ����� ���� �������� ����ϴ�

                // 1�� ��� �ش� ���� ���� ����
                if (itemShapeArray[x,y] == '1')
                {
                    if(isBag)
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingBag(this);
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(this);
                }
                // 0�� ��� �ش� ���� ���� ����
                else
                {                 
                    if (isBag)
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingBag();
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingItem();
                }
            }
        }
    }

    // ������ ��ġ ���� ������ �ʱ�ȭ�ϴ� �޼��� �Դϴ�
    private void InitTouchArea()
    {
        int areaCenterOffset = 200;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (itemShapeArray[x, y] == '1')
                {
                    // 1�� ��� �ش� �����ǿ� ��ġ ���� ���� ����
                    GameObject touchAreaObject = Instantiate(touchAreaPrefab, touchAreas.transform);
                    touchAreaObject.transform.localPosition = new Vector2((x * Inventory.CELL_SIZE - areaCenterOffset), -(y * Inventory.CELL_SIZE - areaCenterOffset));
                }
            }
        }
    }
}