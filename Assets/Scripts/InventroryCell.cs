using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour
{
    public InventoryItem occupyingItem = null; // ���� ���� �ִ� ������
    public InventoryCellDragHandler inventoryCellDragHandler;
    public Vector2 cellPos = Vector2.zero;

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