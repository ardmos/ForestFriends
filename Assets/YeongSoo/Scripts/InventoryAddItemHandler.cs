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
            Debug.Log("����ִ� �κ��丮 ���� �����ϴ�.");
        }
        if (string.IsNullOrEmpty(itemNameInputField.text))
        {
            Debug.Log("Item �̸��� ����ֽ��ϴ�.");
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
