using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �κ��丮�� �ִ� �� ���� ���� ������ �����ϴ� ��ũ��Ʈ
/// </summary>
public class InventoryCell : MonoBehaviour
{
    [SerializeField] private InventoryItem occupyingItem = null; // ���� ���� �ִ� ������
    public InventoryCellDragHandler inventoryCellDragHandler;
    public Vector2 cellPos = Vector2.zero;

    public InventoryItem GetOccupyingItem() { return occupyingItem; }
    public void SetOccupyingItem(InventoryItem occupyingItem) 
    {
        //Debug.Log($"cell:{cellPos}�� occupingItem �����Ͱ� �����˴ϴ�");
        this.occupyingItem = occupyingItem; 
    }
    public void RemoveOccupyingItem()
    {
        occupyingItem = null;
    }

    // ���̶���Ʈ ���
    /*// ���� ��� �̹����� ��Ÿ���� ����
    public Image backgroundImage;

    // ���� ���� ���¸� �����ϴ� �޼���
    public void SetHighlight(bool highlight)
    {
        // ���� ���¿� ���� ���� ������ ����
        backgroundImage.color = highlight ? Color.green : Color.white;
    }*/
}