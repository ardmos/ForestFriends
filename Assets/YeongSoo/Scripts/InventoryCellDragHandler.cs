using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // 드래그된 아이템을 현재 셀에 배치
    public void OnDrop(InventoryItem draggedItem)
    {
        draggedItem.transform.SetParent(inventoryCell.transform);
        draggedItem.transform.localPosition = Vector3.zero;
        inventoryCell.occupyingItem = draggedItem; // 현재 셀에 아이템 정보 저장
        draggedItem.currentCellPos = inventoryCell.cellPos; // 아이템에 저장되는 셀 위치 정보를 업데이트
        draggedItem.UpdateUI();
    }

    // 현 아이템과 draggedItem의 위치를 교환한다
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem의 기존 위치에 현 아이템을 배치시킨다
        InventoryCellDragHandler draggedItemDragHandler = Inventory.instance.GetInventoryCellByPos(draggedItem.currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.OnDrop(inventoryCell.occupyingItem);

        // 현 셀에 draggedItem을 배치시킨다
        OnDrop(draggedItem);
    }
}
