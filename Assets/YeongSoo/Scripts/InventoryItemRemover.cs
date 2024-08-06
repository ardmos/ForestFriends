using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 아이템 제거 기능을 관리하는 스크립트
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
        // 해당 포지션의 셀 데이터를 가져옴
        InventoryCell inventoryCell = Inventory.Instance.GetInventoryCellByPos(cellPosToDelete);
        Debug.Log($"cellPosToDelete:{cellPosToDelete}, occupyingItemData:{inventoryCell.GetOccupyingItem()}, cellPosOnItemData:{inventoryCell.cellPos}");
        // 셀을 점유하고있는 아이템이 있는지 확인
        if (!inventoryCell.GetOccupyingItem())
        {
            Debug.Log("해당 포지션의 셀은 비어있습니다");
            return; // 점유중인 아이템이 없을 경우 해당 로직 종료
        }

        // JSON 데이터상에서 제거 실패시 나머지 작업은 진행하지 않습니다.
        if (!ItemDataManager.TryRemoveItem(inventoryCell.GetOccupyingItem().GetItemData())) return;

        // 점유중인 아이템 오브젝트 제거
        Destroy(inventoryCell.GetOccupyingItem().gameObject);
        // 점유중인 아이템 데이터 제거
        inventoryCell.SetOccupyingItem(null);
    }
}
