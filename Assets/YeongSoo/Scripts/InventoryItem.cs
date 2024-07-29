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
        canvasGroup.blocksRaycasts = false; // 드래그 중일 때 다른 셀의 상호작용을 방해하지 않도록 설정

        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling(); // 부모 오브젝트 내의 Hiearachy상 가장 아래로 이동
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta; // 마우스 이동에 따라 아이템 이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 드래그가 끝나면 다시 감지 가능하도록 설정

        // 빈 셀이었을 경우
        if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
        {
            inventoryCellDragHandler.OnDrop(this);         
        }
        // 아이템이 존재하는 셀이었을 경우
        else if (eventData.pointerEnter.TryGetComponent<InventoryItem>(out InventoryItem occupyingItem)) 
        {
            InventoryCellDragHandler occupiedCell =  Inventory.instance.GetInventoryCellByPos(occupyingItem.currentCellPos).inventoryCellDragHandler;
            occupiedCell.OnSwapItems(this);
        }
        // 인벤토리 셀이 아니었을 경우
        else
        {
            Debug.Log($"드롭한 위치가 유효하지 않습니다!. 감지된 오브젝트 : {eventData.pointerEnter.gameObject.name}");
            // 드롭한 위치가 유효하지 않으면 원래 위치(currentCellPos)로 돌아감
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