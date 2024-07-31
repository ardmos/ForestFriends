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
        SetItem(draggedItem);
    }

    // �� �����۰� draggedItem�� ��ġ�� ��ȯ�Ѵ�
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.OnDrop(inventoryCell.occupyingItem);

        // �� ���� draggedItem�� ��ġ��Ų��
        SetItem(draggedItem);
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(inventoryCell.transform);
        inventoryItem.transform.localPosition = Vector3.zero;
        inventoryCell.occupyingItem = inventoryItem; // ���� ���� ������ ���� ����
        inventoryItem.GetItemData().currentCellPos = inventoryCell.cellPos; // �����ۿ� ����Ǵ� �� ��ġ ������ ������Ʈ
        inventoryItem.UpdateUI();
    }
}
