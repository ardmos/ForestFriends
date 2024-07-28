using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCellDragHandler : MonoBehaviour, IDropHandler
{
    public InventoryCell inventoryCell;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (eventData.pointerDrag.TryGetComponent<InventoryItem>(out InventoryItem draggedItem))
        {
            // 드래그된 아이템을 현재 셀에 배치
            draggedItem.transform.SetParent(inventoryCell.transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
    }
}
