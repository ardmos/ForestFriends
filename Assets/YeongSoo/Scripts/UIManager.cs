using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Inventory inventory;
    public TMP_InputField itemNameInput;
    public Button addButton;

    private void Awake()
    {
        addButton.onClick.AddListener(() =>
        {
            OnAddItemButtonClicked();
        });
    }

    public void OnAddItemButtonClicked()
    {
        var searchResult = inventory.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("비어있는 인벤토리 셀이 없습니다.");
        }
        if (string.IsNullOrEmpty(itemNameInput.text))
        {
            Debug.Log("Item 이름이 비어있습니다.");
        }

        ItemData newItemData = new ItemData()
        {
            itemName = itemNameInput.text,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        inventory.AddNewItem(newItemData);
    }
}