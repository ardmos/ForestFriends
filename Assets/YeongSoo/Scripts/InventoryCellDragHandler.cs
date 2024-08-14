using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 셀에 아이템이 드래그되었을때의 처리를 담당하는 스크립트
/// </summary>
public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // 셀에 아이템을 배치하는 메서드
    public void OnDrop(InventoryItem draggedItem)
    {
        // 기존 셀에 있는 occupied아이템 정보 제거
        Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).RemoveOccupyingItem();

        // 드래그된 아이템을 현재 셀에 배치
        SetItemOnCurrentCell(draggedItem);
    }

    // 현 아이템과 draggedItem의 위치를 교환하는 메서드. (드래그된 셀에 기존에 아이템이 존재할 때)
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem의 기존 위치에 현 아이템을 배치시킨다
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.SetItemOnCurrentCell(inventoryCell.GetOccupyingItem());

        // 현 셀에 draggedItem을 배치시킨다
        SetItemOnCurrentCell(draggedItem);
    }

    // 현재 셀에 아이템 데이터를 배치하는 메서드. (아이템 오브젝트의 인스턴스화는 Inventory 클래스에서 담당합니다. 여기서는 데이터의 수정만 처리.)
    public void SetItemOnCurrentCell(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(Inventory.Instance.contents.transform);
        inventoryItem.transform.localPosition = inventoryCell.transform.localPosition;
        inventoryItem.SetNewCurrentCellPos(inventoryCell.cellPos); // 아이템에 저장되는 셀 위치 정보를 업데이트                                                           
        inventoryItem.UpdateItemArea(); // 아이템 영역 업데이트. 
    }
}
