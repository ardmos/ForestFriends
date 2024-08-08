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
        var searchResult = Inventory.Instance.GetEmptyInventoryCellPos();

        if (!searchResult.success)
        {
            Debug.Log("����ִ� �κ��丮 ���� �����ϴ�.");
            return;
        }
        if (string.IsNullOrEmpty(itemSpecIDInputField.text))
        {
            Debug.Log("itemSpecIDInputField�� ����ֽ��ϴ�.");
            return;
        }

        // 1. SpecID������ ������ ���� �����͸� �˻�
        ItemSpec itemSpec = ItemSpecManager.GetItemSpecBySpecID(int.Parse(itemSpecIDInputField.text));
        if (itemSpec == null)
        {
            Debug.Log("Spec ������ �˻� ����. ������ �߰� �۾��� �����մϴ�.");
            return;
        }

        // 2. �˻��� ������ �������� �������� ���ο� �������� ����
        // ������ itemID���� �������ݴϴ�
        ItemData newItemData = new ItemData()
        {
            itemID = ItemDataManager.GenerateUniqueItemId(),
            itemSpec = itemSpec,
            currentCellPos = searchResult.cellPosition,
            targetCellPos = Vector2.zero
        };

        // ������ ������ JSON�� ���� �õ�. ���н� ������ �۾��� ������� �ʽ��ϴ�.
        if (!ItemDataManager.TryAddPlayerItem(newItemData)) return;

        // ������ �������� �κ��丮�� ��üȭ
        Inventory.Instance.InstantiateItem(newItemData);
    }
}
