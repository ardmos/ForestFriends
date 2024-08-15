using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �κ��丮 ���� �������� �巡�׵Ǿ������� ó���� ����ϴ� ��ũ��Ʈ
/// </summary>
public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // ���� �������� ��ġ�ϴ� �޼���
    public void OnDrop(InventoryItem draggedItem)
    {
        // �巡�׵� �������� ���� ���� ��ġ
        //SetItemOnCurrentCell(draggedItem);
    }

    // �� �����۰� draggedItem�� ��ġ�� ��ȯ�ϴ� �޼���. (�巡�׵� ���� ������ �������� ������ ��)
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        //draggedItemDragHandler.SetItemOnCurrentCell(inventoryCell.GetOccupyingItem());

        // �� ���� draggedItem�� ��ġ��Ų��
        //SetItemOnCurrentCell(draggedItem);
    }   
}
