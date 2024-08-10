using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리에 있는 한 개의 셀의 정보를 관리하는 스크립트
/// </summary>
public class InventoryCell : MonoBehaviour
{
    [SerializeField] private InventoryItem occupyingItem = null; // 현재 셀에 있는 아이템
    public InventoryCellDragHandler inventoryCellDragHandler;
    public Vector2 cellPos = Vector2.zero;

    public InventoryItem GetOccupyingItem() { return occupyingItem; }
    public void SetOccupyingItem(InventoryItem occupyingItem) 
    {
        //Debug.Log($"cell:{cellPos}의 occupingItem 데이터가 설정됩니다");
        this.occupyingItem = occupyingItem; 
    }
    public void RemoveOccupyingItem()
    {
        occupyingItem = null;
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