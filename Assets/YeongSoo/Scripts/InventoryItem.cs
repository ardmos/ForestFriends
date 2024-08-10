using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

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
        rectTransform.anchoredPosition += eventData.delta; // ���콺 �̵��� ���� ������ �̵�
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

        // ����� ��ġ�� �� �κ��丮 ������ Ȯ��
        if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
        {
            HandleEmptyCellDrop(inventoryCellDragHandler);
        }
        // ����� ��ġ�� �������� �����ϴ��� Ȯ��
        else if (eventData.pointerEnter.TryGetComponent<InventoryItem>(out InventoryItem occupyingItem))
        {
            HandleOccupiedCellDrop(occupyingItem);
        }
        // ����� ��ġ�� �κ��丮 ���� �ƴ� ���
        else
        {
            HandleInvalidDropLocation();
        }
    }

    // �������� ���� ��ġ�� �ǵ����� �޼���
    private void ReturnToOriginalPosition()
    {
        // ���� �������� ���� �θ�(��)�� ã��, ��ġ�� �ʱ�ȭ
        Transform originalParent = Inventory.Instance.GetInventoryCellByPos(itemData.currentCellPos).transform;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    // �� ���� ������� �� ó���ϴ� �޼���
    private void HandleEmptyCellDrop(InventoryCellDragHandler inventoryCellDragHandler)
    {
        inventoryCellDragHandler.OnDrop(this);
    }

    // �̹� �������� �ִ� ���� ������� �� ó���ϴ� �޼���
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem)
    {
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

    public void SetItemData(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        SetItemShapeArrayData();
        InitItemImage();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    /// <summary>
    /// itemSpec�� ��� itemShape �����͸� 5x5 �迭�� ��ȯ�� �����ϴ� �޼����Դϴ�.
    /// </summary>
    private void SetItemShapeArrayData()
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
                itemShapeArray[i, j] = shapeData[i * chunkSize + j];
            }
        }
    }

    // �������� �̹����� �ʱ�ȭ�ϴ� �޼��� �Դϴ�
    private void InitItemImage()
    {
        itemImage.sprite = GameAssetManager.Instance.weaponAssets.GetWeaponImageBySpecID(itemData.itemSpec.itemSpecID);
    }
}