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

    // ����ִ� ���� �������� ��ġ�� ��
    public void OnDrop(InventoryItem draggedItem)
    {
        // ���� ���� �ִ� occupied������ ���� ����
        Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).SetOccupyingItem(null);

        // �巡�׵� �������� ���� ���� ��ġ
        SetItem(draggedItem);
    }

    // �巡�׵� ���� ������ �������� ������ ��. �� �����۰� draggedItem�� ��ġ�� ��ȯ�մϴ�
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.SetItem(inventoryCell.GetOccupyingItem());

        // �� ���� draggedItem�� ��ġ��Ų��
        SetItem(draggedItem);
    }

    // ���� ���� ������ �����͸� ��ġ�ϴ� �޼���. (������ ������Ʈ�� �ν��Ͻ�ȭ�� Inventory Ŭ�������� ����մϴ�. ���⼭�� �������� ������ ó��.)
    public void SetItem(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(inventoryCell.transform);
        inventoryItem.transform.localPosition = Vector3.zero;
        inventoryCell.SetOccupyingItem(inventoryItem); // ���� ���� ������ ���� ����
        inventoryItem.GetItemData().currentCellPos = inventoryCell.cellPos; // �����ۿ� ����Ǵ� �� ��ġ ������ ������Ʈ
        inventoryItem.UpdateUI();
    }
}
