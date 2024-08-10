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
        Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).RemoveOccupyingItem();

        // �巡�׵� �������� ���� ���� ��ġ
        SetItemOnCurrentCell(draggedItem);
    }

    // �巡�׵� ���� ������ �������� ������ ��. �� �����۰� draggedItem�� ��ġ�� ��ȯ�մϴ�
    public void OnSwapItems(InventoryItem draggedItem)
    {
        // draggedItem�� ���� ��ġ�� �� �������� ��ġ��Ų��
        InventoryCellDragHandler draggedItemDragHandler = Inventory.Instance.GetInventoryCellByPos(draggedItem.GetItemData().currentCellPos).inventoryCellDragHandler;
        draggedItemDragHandler.SetItemOnCurrentCell(inventoryCell.GetOccupyingItem());

        // �� ���� draggedItem�� ��ġ��Ų��
        SetItemOnCurrentCell(draggedItem);
    }

    // ���� ���� ������ �����͸� ��ġ�ϴ� �޼���. (������ ������Ʈ�� �ν��Ͻ�ȭ�� Inventory Ŭ�������� ����մϴ�. ���⼭�� �������� ������ ó��.)
    public void SetItemOnCurrentCell(InventoryItem inventoryItem)
    {
        inventoryItem.transform.SetParent(inventoryCell.transform);
        inventoryItem.transform.localPosition = Vector3.zero;
        inventoryItem.SetNewCurrentCellPos(inventoryCell.cellPos); // �����ۿ� ����Ǵ� �� ��ġ ������ ������Ʈ                                                           
        inventoryItem.UpdateItemArea(); // ������ ���� ������Ʈ. 
    }
}
