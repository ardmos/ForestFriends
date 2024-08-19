using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 셀에 아이템이 드래그되었을때의 처리를 담당하는 스크립트
/// </summary>
public class InventoryCellDragHandler : MonoBehaviour
{
    [SerializeField] private InventoryCell inventoryCell;

    // 셀에 아이템을 배치하는 메서드
    public void OnDrop(InventoryItem draggedItem)
    {
        if (draggedItem == null || inventoryCell == null || Inventory.Instance == null) return;

        // 현재 셀에 draggedItem을 배치
        Inventory.Instance.UpdateItemArea(draggedItem);
    }

    // 현 아이템과 draggedItem의 위치를 교환하는 메서드. (드래그된 셀에 기존에 아이템이 존재할 때)
    public void OnSwapItems(InventoryItem draggedItem)
    {
        if (draggedItem == null || inventoryCell == null || Inventory.Instance == null) return;

        // draggedItem의 기존 위치에 현 아이템을 배치
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.OnDrop(inventoryCell.GetOccupyingItem());

        // 현 셀에 draggedItem을 배치
        Inventory.Instance.UpdateItemArea(draggedItem);
    }   

    public InventoryCell GetInventoryCell() { return inventoryCell; }
}
