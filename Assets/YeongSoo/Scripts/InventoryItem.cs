using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    private ItemData itemData;
    private Vector2 originalPosition;
    private Canvas mainCanvas;
    // 5x5 배열 형태로 가공된 아이템 형상 정보를 담을 변수
    private char[,] itemShapeArray;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // 드래그 중일 때 다른 셀의 상호작용을 방해하지 않도록 설정

        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling(); // 부모 오브젝트 내의 Hiearachy상 가장 아래로 이동
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta; // 마우스 이동에 따라 아이템 이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 드래그가 끝나면 다시 감지 가능하도록 설정

        // 빈 셀이었을 경우
        if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
        {
            inventoryCellDragHandler.OnDrop(this);         
        }
        // 아이템이 존재하는 셀이었을 경우
        else if (eventData.pointerEnter.TryGetComponent<InventoryItem>(out InventoryItem occupyingItem)) 
        {
            InventoryCellDragHandler occupiedCell =  Inventory.Instance.GetInventoryCellByPos(occupyingItem.itemData.currentCellPos).inventoryCellDragHandler;
            occupiedCell.OnSwapItems(this);
        }
        // 인벤토리 셀이 아니었을 경우
        else
        {
            Debug.Log($"드롭한 위치가 유효하지 않습니다!. 감지된 오브젝트 : {eventData.pointerEnter.gameObject.name}");
            // 드롭한 위치가 유효하지 않으면 원래 위치(currentCellPos)로 돌아감
            transform.SetParent(Inventory.Instance.GetInventoryCellByPos(itemData.currentCellPos).transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void SetItemData(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        UpdateUI();
        SetItemShapeArrayData();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    public void UpdateUI()
    {
        itemNameText.text = itemData.itemSpec.itemName;
        itemImage.sprite = GameAssetManager.Instance.weaponAssets.GetWeaponImageBySpecID(itemData.itemSpec.itemSpecID);
    }

    /// <summary>
    /// itemSpec에 적힌 아이템 형태 정보에 따라 실제 아이템의 형태를 설정해주는 메서드 입니다 
    /// 이 정보를 활용해 아이템의 이동과 배치 가능 여부를 결정합니다.
    /// </summary>
    private void SetItemShapeArrayData()
    {
        // 5x5 형태의 그리드 데이터가 일렬로 담긴 문자열 데이터
        string shapeData = itemData.itemSpec.itemShape;

        // 5x5 배열 형태로 가공된 아이템 형상 정보를 담을 변수
        itemShapeArray = new char[5, 5];
        int chunkSize = 5;

        // 문자열을 5x5 배열로 변환
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                // 각 행의 시작 인덱스는 i * chunkSize
                // 열 인덱스는 j
                itemShapeArray[i, j] = shapeData[i * chunkSize + j];
            }
        }
    }
}