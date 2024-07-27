using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IDropHandler
{
    public ItemData itemData = null; // ���� ���� �ִ� ������

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (draggedItem != null)
            {
                // �巡�׵� �������� ���� ���� ��ġ
                draggedItem.transform.SetParent(transform);
                draggedItem.transform.localPosition = Vector3.zero;
            }
        }
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