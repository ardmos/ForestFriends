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
            // �巡�׵� �������� ���� ���� ��ġ
            draggedItem.transform.SetParent(inventoryCell.transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
    }
}
