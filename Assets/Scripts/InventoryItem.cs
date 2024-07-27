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
        canvasGroup.blocksRaycasts = false; // 드래그 중일 때 다른 셀의 상호작용을 방해하지 않도록 설정
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta; // 마우스 이동에 따라 아이템 이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 드래그가 끝나면 다시 감지 가능하도록 설정

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventoryCell>() == null)
        {
            // 드롭한 위치가 유효하지 않으면 원래 위치로 돌아감
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