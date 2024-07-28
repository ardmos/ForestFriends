using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI itemNameText;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

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

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventoryCell>() == null)
        {
            // ����� ��ġ�� ��ȿ���� ������ ���� ��ġ�� ���ư�
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void SetItemData(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        UpdateUI();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    private void UpdateUI()
    {
        itemNameText.text = itemData.item.itemName;
    }
}