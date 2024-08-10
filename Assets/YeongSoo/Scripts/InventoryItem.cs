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

    // ������ ���� �ʱ�ȭ
    public void InitInventoryItem(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        InitItemShapeArrayData();
        InitItemImage();
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
        itemImage.sprite = GameAssetManager.Instance.weaponAssets.GetWeaponImageBySpecID(itemData.itemSpec.itemSpecID);
    }

    // ������ ���� ������ ������Ʈ�ϴ� �޼���
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
}