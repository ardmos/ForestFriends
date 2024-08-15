using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2 originalPosition;
    public Transform originalParent;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public GameObject touchAreas;
    public GameObject touchAreaPrefab;

    [SerializeField] private bool isBag;
    private ItemData itemData;
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
            // ����ġ�� ����
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        Inventory.Instance.OnDrop(cell, this); // �κ��丮�� ��� �۾� ����
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