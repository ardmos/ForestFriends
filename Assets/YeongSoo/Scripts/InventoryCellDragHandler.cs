using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCellDragHandler : MonoBehaviour
{
    public InventoryCell inventoryCell;

    // ����ִ� ���� �������� ��ġ�� ��
    public void OnDrop(InventoryItem draggedItem)
    {
        // ���� ���� �ִ� occupied������ ���� ����
        Inventory.instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).occupyingItem = null;

        // �巡�׵� �������� ���� ���� ��ġ
        SetItem(draggedItem);
    }

    // �巡�׵� ���� ������ �������� ������ ��. �� �����۰� draggedItem�� ��ġ�� ��ȯ�մϴ�
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.SetItem(inventoryCell.occupyingItem);

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
