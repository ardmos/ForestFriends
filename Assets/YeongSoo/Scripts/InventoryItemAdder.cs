using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 아이템 추가 기능을 관리하는 스크립트
/// </summary>
public class InventoryItemAdder : MonoBehaviour
{
    public TMP_InputField itemSpecIDInputField;
    public Button addItemButton;

    private void Start()
    {
        addItemButton.onClick.AddListener(OnAddItem);
    }

    private void OnAddItem() 
    {
        // 비어있는 셀을 검색
        var searchResult = Inventory.instance.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("비어있는 인벤토리 셀이 없습니다.");
        }
        if (string.IsNullOrEmpty(itemSpecIDInputField.text))
        {
            Debug.Log("itemSpecIDInputField가 비어있습니다.");
        }

        // 새로운 아이템의 데이터를 생성
        ItemData newItemData = new ItemData()
        {
            name = itemSpecIDInputField.text,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        // 생성된 정보를 JSON에 저장
        ItemDataManager.AddItem(newItemData);
        // 생성된 아이템을 인벤토리에 실체화
        Inventory.instance.InstantiateItem(newItemData);
    }
}
