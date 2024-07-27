using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IDropHandler
{
    public ItemData itemData = null; // 현재 셀에 있는 아이템

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (draggedItem != null)
            {
                // 드래그된 아이템을 현재 셀에 배치
                draggedItem.transform.SetParent(transform);
                draggedItem.transform.localPosition = Vector3.zero;
            }
        }
    }

    // 하이라이트 기능
    /*// 셀의 배경 이미지를 나타내는 변수
    public Image backgroundImage;

    // 셀의 강조 상태를 설정하는 메서드
    public void SetHighlight(bool highlight)
    {
        // 강조 상태에 따라 셀의 색상을 변경
        backgroundImage.color = highlight ? Color.green : Color.white;
    }*/
}