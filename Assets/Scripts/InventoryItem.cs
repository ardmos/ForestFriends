using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI itemNameText;

    private ItemData itemData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // �巡�� ���� �� �ٸ� ���� ��ȣ�ۿ��� �������� �ʵ��� ����
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

    public void SetItemData(ItemData itemData)
    {
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