using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // �巡�׵� �������� ���� ���� ��ġ
    public void OnDrop(InventoryItem draggedItem)
    {
        draggedItem.transform.SetParent(inventoryCell.transform);
        draggedItem.transform.localPosition = Vector3.zero;
        inventoryCell.occupyingItem = draggedItem; // ���� ���� ������ ���� ����
        draggedItem.currentCellPos = inventoryCell.cellPos; // �����ۿ� ����Ǵ� �� ��ġ ������ ������Ʈ
        draggedItem.UpdateUI();
    }

    // �� �����۰� draggedItem�� ��ġ�� ��ȯ�Ѵ�
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.instance.GetInventoryCellByPos(draggedItem.currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.OnDrop(inventoryCell.occupyingItem);

        // �� ���� draggedItem�� ��ġ��Ų��
        OnDrop(draggedItem);
    }
}
