using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �κ��丮 ������ �߰� ����� �����ϴ� ��ũ��Ʈ
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
        // ����ִ� ���� �˻�
        var searchResult = Inventory.instance.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("����ִ� �κ��丮 ���� �����ϴ�.");
        }
        if (string.IsNullOrEmpty(itemSpecIDInputField.text))
        {
            Debug.Log("itemSpecIDInputField�� ����ֽ��ϴ�.");
        }

        // ���ο� �������� �����͸� ����
        ItemData newItemData = new ItemData()
        {
            name = itemSpecIDInputField.text,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        // ������ ������ JSON�� ����
        ItemDataManager.AddItem(newItemData);
        // ������ �������� �κ��丮�� ��üȭ
        Inventory.instance.InstantiateItem(newItemData);
    }
}
