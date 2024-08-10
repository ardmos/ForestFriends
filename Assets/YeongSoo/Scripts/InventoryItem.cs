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
        // 드래그가 끝나면 다시 레이캐스트를 감지할 수 있도록 설정
        canvasGroup.blocksRaycasts = true;

        // 드롭한 위치가 빈 공간인 경우
        if (eventData.pointerEnter == null)
        {
            HandleInvalidDropLocation();
            return;
        }

        // 드롭한 위치가 빈 인벤토리 셀인지 확인
        if (eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
        {
            HandleEmptyCellDrop(inventoryCellDragHandler);
        }
        // 드롭한 위치에 아이템이 존재하는지 확인
        else if (eventData.pointerEnter.TryGetComponent<InventoryItem>(out InventoryItem occupyingItem))
        {
            HandleOccupiedCellDrop(occupyingItem);
        }
        // 드롭한 위치가 인벤토리 셀이 아닌 경우
        else
        {
            HandleInvalidDropLocation();
        }
    }

    // 아이템을 원래 위치로 되돌리는 메서드
    private void ReturnToOriginalPosition()
    {
        // 현재 아이템의 원래 부모(셀)를 찾고, 위치를 초기화
        Transform originalParent = Inventory.Instance.GetInventoryCellByPos(itemData.currentCellPos).transform;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    // 빈 셀에 드롭했을 때 처리하는 메서드
    private void HandleEmptyCellDrop(InventoryCellDragHandler inventoryCellDragHandler)
    {
        inventoryCellDragHandler.OnDrop(this);
    }

    // 이미 아이템이 있는 셀에 드롭했을 때 처리하는 메서드
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem)
    {
        // 현재 아이템과 교환할 셀을 찾아서 아이템 교환 처리
        InventoryCellDragHandler occupiedCell = Inventory.Instance.GetInventoryCellByPos(occupyingItem.itemData.currentCellPos).inventoryCellDragHandler;
        occupiedCell.OnSwapItems(this);
    }

    // 드롭한 위치가 유효하지 않은 경우 처리하는 메서드
    private void HandleInvalidDropLocation()
    {
        // 유효하지 않은 드롭 위치에 대한 로그 출력
        Debug.Log($"Invalid drop location detected!");
        // 원래 위치로 돌아감
        ReturnToOriginalPosition();
    }

    // 아이템 정보 초기화
    public void InitInventoryItem(ItemData itemData, Canvas canvas)
    {
        this.mainCanvas = canvas;
        this.itemData = itemData;
        InitItemShapeArrayData();
        InitItemImage();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    public void SetNewCurrentCellPos(Vector2 newPos)
    {
        itemData.currentCellPos = newPos;
    }

    /// <summary>
    /// itemSpec에 담긴 itemShape 데이터를 5x5 배열로 변환해 초기화하는 메서드입니다.
    /// </summary>
    private void InitItemShapeArrayData()
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
                itemShapeArray[j, i] = shapeData[i * chunkSize + j];
            }
        }
    }

    // 아이템의 이미지를 초기화하는 메서드 입니다
    private void InitItemImage()
    {
        itemImage.sprite = GameAssetManager.Instance.weaponAssets.GetWeaponImageBySpecID(itemData.itemSpec.itemSpecID);
    }

    // 아이템 점유 영역을 업데이트하는 메서드
    public void UpdateItemArea()
    {
        // itemShapeArray 입장에선 [2,2]가 현 아이템의 중심. 
        // 따라서 드래그 드랍이 된 셀에서 (-2,-2) 이동한 위치가 itemShapeArray의 [0,0]. 오프셋 값이 됩니다.
        // itemShapeArray의 값이 1인 포지션의 셀들에 아이템 점유 설정을 해줘야 합니다. 0일 경우 점유 해제

        // 우선 현 셀의 pos를 얻어옵니다
        Vector2 currentCellPos = itemData.currentCellPos;
        // 현 셀의 포지션과 itemShapeArray 인덱스 사이의 오프셋 값 구하기
        Vector2 offset = currentCellPos - new Vector2(2, 2);

        // 실제 인벤토리의 셀들에 아이템 점유 설정을 해주기.
        for(int x = 0; x < 5; x++)
        {
            for(int y = 0; y < 5; y++)
            {
                // offset이 적용된 최종 포지션을 담을 Vector2 변수
                Vector2 actualCellPos = offset + new Vector2(x, y); // offest이 적용된 최정 포지션을 담습니다

                //Debug.Log($"itemShapeArray[x,y]:{itemShapeArray[x, y]}");
                if (itemShapeArray[x,y] == '1')
                {
                    //Debug.Log($"셀에 아이템{itemData.itemSpec.itemName} 점유 설정! cellPos:{actualCellPos}");
                    // 1일 경우 해당 셀에 점유 설정
                    Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(this);
                }
                else
                {
                    //Debug.Log($"셀에 아이템{itemData.itemSpec.itemName} 점유 해제! cellPos:{actualCellPos}");
                    // 0일 경우 해당 셀에 점유 해제
                    Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingItem();
                }
            }
        }
    }
}