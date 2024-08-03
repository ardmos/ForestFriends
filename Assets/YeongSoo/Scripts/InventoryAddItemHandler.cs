using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAddItemManager : MonoBehaviour
{
    public TMP_InputField itemNameInputField;
    public Button addItemButton;


    private void Start()
    {
        addItemButton.onClick.AddListener(OnAddItem);
    }

    private void OnAddItem() 
    {
        var searchResult = Inventory.instance.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("비어있는 인벤토리 셀이 없습니다.");
        }
        if (string.IsNullOrEmpty(itemNameInputField.text))
        {
            Debug.Log("Item 이름이 비어있습니다.");
        }

        ItemData newItemData = new ItemData()
        {
            itemName = itemNameInputField.text,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        Inventory.instance.AddNewItem(newItemData);
    }
}
