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
        var searchResult = Inventory.Instance.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("비어있는 인벤토리 셀이 없습니다.");
            return;
        }
        if (string.IsNullOrEmpty(itemSpecIDInputField.text))
        {
            Debug.Log("itemSpecIDInputField가 비어있습니다.");
            return;
        }

        // 1. SpecID값으로 아이템 스펙 데이터를 검색
        ItemSpec itemSpec = ItemSpecManager.GetItemSpecBySpecID(int.Parse(itemSpecIDInputField.text));
        if (itemSpec == null)
        {
            Debug.Log("Spec 데이터 검색 실패. 아이템 추가 작업을 중지합니다.");
            return;
        }

        // 2. 검색한 아이템 스펙을를 바탕으로 새로운 아이템을 생성
        // 고유한 itemID값을 배정해줍니다
        ItemData newItemData = new ItemData()
        {
            itemID = ItemDataManager.GenerateUniqueItemId(),
            itemSpec = itemSpec,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        // 생성된 정보를 JSON에 저장 시도. 실패시 나머지 작업은 진행되지 않습니다.
        if (!ItemDataManager.TryAddPlayerItem(newItemData)) return;

        // 생성된 아이템을 인벤토리에 실체화
        Inventory.Instance.InstantiateItem(newItemData);
    }
}
