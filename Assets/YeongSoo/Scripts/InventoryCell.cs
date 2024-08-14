using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �κ��丮�� �ִ� �� ���� ���� ������ �����ϴ� ��ũ��Ʈ
/// </summary>
public class InventoryCell : MonoBehaviour
{
    // ���� ���� ��ġ�Ȱ��� �������� ������������ ���� �Ʒ� ������ ������ �����˴ϴ�.
    [SerializeField] private InventoryItem occupyingItem = null; // ���� ���� �ִ� ������
    [SerializeField] private InventoryItem occupyingBag = null; // ���� ���� �ִ� ����
    [SerializeField] private bool isBagSlot = false; // �� ���� ������ �������� ���θ� �����ϴ� ����
    public InventoryCellDragHandler inventoryCellDragHandler;
    public Vector2 cellPos = Vector2.zero;

    public InventoryItem GetOccupyingItem() 
    { 
        return occupyingItem; 
    }
    public void SetOccupyingItem(InventoryItem newItem) 
    {
        occupyingItem = newItem;
    }
    public void RemoveOccupyingItem()
    {
        occupyingItem = null;
    }

    public InventoryItem GetOccupyingBag()
    {
        return occupyingBag;
    }
    public void SetOccupyingBag(InventoryItem newBag)
    {
        occupyingBag = newBag;
        isBagSlot = true;      
    }
    public void RemoveOccupyingBag()
    {
        occupyingBag = null;
        isBagSlot = false;
    }

    public void SetIsBagSlot(bool value)
    {
        isBagSlot = value;
    }

    public bool GetIsBagSlot() 
    {  
        return isBagSlot; 
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