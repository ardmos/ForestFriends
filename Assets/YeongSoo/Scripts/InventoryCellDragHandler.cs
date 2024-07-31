using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // 비어있는 셀에 아이템이 배치될 때
    public void OnDrop(InventoryItem draggedItem)
    {
        // 기존 셀에 있는 occupied아이템 정보 제거
        Inventory.instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).occupyingItem = null;

        // 드래그된 아이템을 현재 셀에 배치
        SetItem(draggedItem);
    }

    // 드래그된 셀에 기존에 아이템이 존재할 때. 현 아이템과 draggedItem의 위치를 교환합니다
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem의 기존 위치에 현 아이템을 배치시킨다
        InventoryCellDragHandler draggedItemDragHandler = Inventory.instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.SetItem(inventoryCell.occupyingItem);

        // 현 셀에 draggedItem을 배치시킨다
        SetItem(draggedItem);
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(inventoryCell.transform);
        inventoryItem.transform.localPosition = Vector3.zero;
        inventoryCell.occupyingItem = inventoryItem; // 현재 셀에 아이템 정보 저장
        inventoryItem.GetItemData().currentCellPos = inventoryCell.cellPos; // 아이템에 저장되는 셀 위치 정보를 업데이트
        inventoryItem.UpdateUI();
    }
}
