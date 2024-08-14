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

    private ItemData itemData;
    private Vector2 originalPosition;
    private Canvas mainCanvas;
    // 5x5 �迭 ���·� ������ ������ ���� ������ ���� ����
    private char[,] itemShapeArray;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
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

        // ����� ��ġ�� �̹� ��ġ�� �������� �ִ� ���
        if (eventData.pointerEnter.GetComponentInParent<InventoryItem>())
        {
            HandleOccupiedCellDrop(eventData.pointerEnter.GetComponentInParent<InventoryItem>());
        }
        // ����� ��ġ�� ��ġ ������ �κ��丮 ���� ���. (InventoryCell�̸� ���ÿ� BagSlot)
        else if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler) && inventoryCellDragHandler.inventoryCell.GetIsBagSlot())
        {
            HandleEmptyCellDrop(inventoryCellDragHandler);
        }
        // ����� ��ġ�� �� ������ �ƴ�����, �������� ��ġ�� �� ���� ������ ��� ���
        else
        {
            Debug.Log($"�������� ���� ��ġ: {eventData.pointerEnter.gameObject.name}");
            HandleInvalidDropLocation();
        }
    }

    // �������� ���� ��ġ�� �ǵ����� �޼���
    private void ReturnToOriginalPosition()
    {
        // ���� �������� ���� �θ�(��)�� ã��, ��ġ�� �ʱ�ȭ
        Transform originalParent = Inventory.Instance.GetInventoryCellByPos(itemData.currentCellPos).transform;
        transform.SetParent(Inventory.Instance.contents.transform);
        rectTransform.anchoredPosition = Inventory.Instance.GetInventoryCellByPos(itemData.currentCellPos).transform.localPosition;
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
        InitItemShapeArrayData();
        InitItemImage();
        InitTouchArea();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
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

    // ������ ���� ������ ������Ʈ�ϴ� �޼��� �Դϴ�
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

                //Debug.Log($"itemShapeArray[x,y]:{itemShapeArray[x, y]}");
                if (itemShapeArray[x,y] == '1')
                {
                    //Debug.Log($"���� ������{itemData.itemSpec.itemName} ���� ����! cellPos:{actualCellPos}");
                    // 1�� ��� �ش� ���� ���� ����
                    Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(this);
                }
                else
                {
                    //Debug.Log($"���� ������{itemData.itemSpec.itemName} ���� ����! cellPos:{actualCellPos}");
                    // 0�� ��� �ش� ���� ���� ����
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