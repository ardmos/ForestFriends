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
            Debug.Log("����ִ� �κ��丮 ���� �����ϴ�.");
        }
        if (string.IsNullOrEmpty(itemNameInput.text))
        {
            Debug.Log("Item �̸��� ����ֽ��ϴ�.");
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