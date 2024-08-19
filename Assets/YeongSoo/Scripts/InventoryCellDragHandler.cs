using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �κ��丮 ���� �������� �巡�׵Ǿ������� ó���� ����ϴ� ��ũ��Ʈ
/// </summary>
public class InventoryCellDragHandler : MonoBehaviour
{
    [SerializeField] private InventoryCell inventoryCell;

    // ���� �������� ��ġ�ϴ� �޼���
    public void OnDrop(InventoryItem draggedItem)
    {
        if (draggedItem == null || inventoryCell == null || Inventory.Instance == null) return;

        // ���� ���� draggedItem�� ��ġ
        Inventory.Instance.UpdateItemArea(draggedItem);
    }

    // �� �����۰� draggedItem�� ��ġ�� ��ȯ�ϴ� �޼���. (�巡�׵� ���� ������ �������� ������ ��)
    public void OnSwapItems(InventoryItem draggedItem)
    {
        if (draggedItem == null || inventoryCell == null || Inventory.Instance == null) return;

        // draggedItem�� ���� ��ġ�� �� �������� ��ġ
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.OnDrop(inventoryCell.GetOccupyingItem());

        // �� ���� draggedItem�� ��ġ
        Inventory.Instance.UpdateItemArea(draggedItem);
    }   

    public InventoryCell GetInventoryCell() { return inventoryCell; }
}
