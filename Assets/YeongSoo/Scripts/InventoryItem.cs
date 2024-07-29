using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI itemNameText;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Vector2 currentCellPos;

    private ItemData itemData;
    private Vector2 originalPosition;
    private Canvas mainCanvas;

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
        canvasGroup.blocksRaycasts = true; // �巡�װ� ������ �ٽ� ���� �����ϵ��� ����

        // �� ���̾��� ���
        if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
        {
            inventoryCellDragHandler.OnDrop(this);         
        }
        // �������� �����ϴ� ���̾��� ���
        else if (eventData.pointerEnter.TryGetComponent<InventoryItem>(out InventoryItem occupyingItem)) 
        {
            InventoryCellDragHandler occupiedCell =  Inventory.instance.GetInventoryCellByPos(occupyingItem.currentCellPos).inventoryCellDragHandler;
            occupiedCell.OnSwapItems(this);
        }
        // �κ��丮 ���� �ƴϾ��� ���
        else
        {
            Debug.Log($"����� ��ġ�� ��ȿ���� �ʽ��ϴ�!. ������ ������Ʈ : {eventData.pointerEnter.gameObject.name}");
            // ����� ��ġ�� ��ȿ���� ������ ���� ��ġ(currentCellPos)�� ���ư�
            transform.SetParent(Inventory.instance.GetInventoryCellByPos(currentCellPos).transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void SetItemData(ItemData itemData, Canvas canvas, Vector2 cellPos)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        this.currentCellPos = cellPos;
        UpdateUI();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    public void UpdateUI()
    {
        itemNameText.text = itemData.item.currentCellPos.ToString(); // itemData.item.itemName;
    }
}