using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �κ��丮 ������ ���� ����� �����ϴ� ��ũ��Ʈ
/// </summary>
public class InventoryItemRemover : MonoBehaviour
{
    public Vector2 cellPosToDelete;
    public Button deleteItemButton;

    private void Start()
    {
        deleteItemButton.onClick.AddListener(OnDeleteItem);
    }

    private void OnDeleteItem()
    {
        // �ش� �������� �� �����͸� ������
        InventoryCell inventoryCell = Inventory.Instance.GetInventoryCellByPos(cellPosToDelete);
        Debug.Log($"cellPosToDelete:{cellPosToDelete}, occupyingItemData:{inventoryCell.GetOccupyingItem()}, cellPosOnItemData:{inventoryCell.cellPos}");
        // ���� �����ϰ��ִ� �������� �ִ��� Ȯ��
        if (!inventoryCell.GetOccupyingItem())
        {
            Debug.Log("�ش� �������� ���� ����ֽ��ϴ�");
            return; // �������� �������� ���� ��� �ش� ���� ����
        }

        // JSON �����ͻ󿡼� ���� ���н� ������ �۾��� �������� �ʽ��ϴ�.
        if (!ItemDataManager.TryRemoveItem(inventoryCell.GetOccupyingItem().GetItemData())) return;

        // �������� ������ ������Ʈ ����
        Destroy(inventoryCell.GetOccupyingItem().gameObject);
        // �������� ������ ������ ����
        inventoryCell.SetOccupyingItem(null);
    }
}
