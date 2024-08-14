using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public GameObject touchAreas;
    public GameObject touchAreaPrefab;

    [SerializeField] private bool isBag;
    private ItemData itemData;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas mainCanvas;
    // 5x5 배열 형태로 가공된 아이템 형상 정보를 담을 변수
    private char[,] itemShapeArray;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false; // 드래그 중일 때 다른 셀의 상호작용을 방해하지 않도록 설정

        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling(); // 부모 오브젝트 내의 Hiearachy상 가장 아래로 이동
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCanvas == null)
            return;

        Vector2 localPoint;
        // 마우스의 화면 좌표를 캔버스의 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // 마우스 이동에 따라 아이템 이동
            // 로컬 좌표를 UI 요소의 위치로 설정
            rectTransform.anchoredPosition = localPoint;
        }
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


        // 마우스 위치에 레이캐스트 발사
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        InventoryCellDragHandler cellDragHandler = null;
        InventoryCell cell = null;

        foreach (RaycastResult result in results)
        {
            if(eventData.pointerEnter.TryGetComponent<InventoryCellDragHandler>(out InventoryCellDragHandler inventoryCellDragHandler))
            {
                cellDragHandler = inventoryCellDragHandler;
                cell = inventoryCellDragHandler.inventoryCell;
            }
        }

        if(cell == null)
        {
            Debug.Log("현 드랍 위치에 셀이 검색되지 않았습니다.");
            HandleInvalidDropLocation();
            return;
        }

        // 가방 영역 아이템 배치. 기능 구현중 중간 저장
/*
        // 현 아이템이 가방인 경우
        if (isBag)
        {
            // 점유중인 가방이 자신인 경우 선택된 셀로 중심 이동 ( 추후 이동불가지역 필터링시 조건 추가 필요)
            if (cell.GetOccupyingBag() == this)
            {
                Debug.Log("현 가방 자신입니다.");
                HandleEmptyCellDrop(cellDragHandler);
            }
            // 점유중인 가방이 없는 경우
            else if (cell.GetOccupyingBag() == null)
            {
                HandleEmptyCellDrop(cellDragHandler);
            }
            // 점유중인 가방이 있는 경우
            else
            {
                HandleOccupiedCellDrop(cell.GetOccupyingBag());
            }
        }
        // 가방 이외의 아이템인 경우
        else
        {
            // 드롭한 위치가 가방의 영역인지 확인
            if (cell.GetIsBagSlot())
            {
                // 점유중인 아이템이 현 아이템인 경우 드랍된 셀의 위치로 중심 이동
                if (cell.GetOccupyingItem() == this)        
                {
                    Debug.Log("현 아이템 자신입니다.");
                    HandleEmptyCellDrop(cellDragHandler);
                }
                // 비어있는 가방 슬롯인 경우
                else if (cell.GetOccupyingItem() == null)
                {
                    HandleEmptyCellDrop(cellDragHandler);
                }
                // 현 아이템 이외의 점유중인 아이템이 있는 경우
                else
                {
                    HandleOccupiedCellDrop(cell.GetOccupyingItem());
                }
            }
            // 드롭한 위치가 빈 공간은 아니지만, 아이템을 배치할 수 없는 공간인 모든 경우
            else
            {
                Debug.Log($"아이템이 놓인 위치: {eventData.pointerEnter.gameObject.name}");
                HandleInvalidDropLocation();
            }
        }*/
    }

    // 아이템을 원래 위치로 되돌리는 메서드
    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    // 빈 셀에 드롭했을 때 처리하는 메서드
    private void HandleEmptyCellDrop(InventoryCellDragHandler inventoryCellDragHandler)
    {
        inventoryCellDragHandler.OnDrop(this);
    }

    // 이미 아이템이 있는 셀에 드롭했을 때 처리하는 메서드
    private void HandleOccupiedCellDrop(InventoryItem occupyingItem)
    {
        // 아이템 크기가 동일한 경우만 교환 처리
        if (occupyingItem.GetItemData().itemSpec.itemShape != itemData.itemSpec.itemShape)
        {
            Debug.Log($"아이템 모양이 다르기 때문에 위치 교환이 진행되지 않습니다");
            ReturnToOriginalPosition();
            return;
        }

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
        isBag = itemData.itemSpec.sheetName == GoogleSheetLoader.Sheets.BAG;
        InitItemShapeArrayData();
        InitItemImage();
        InitTouchArea();
    }

    public ItemData GetItemData()
    {
        return this.itemData;
    }

    public bool GetIsBag()
    {
        return isBag;
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
        itemImage.sprite = GameAssetManager.Instance.GetItemImageBySpecID(itemData.itemSpec.sheetName, itemData.itemSpec.itemSpecID);
        // 가방 추가 기능.  여기까지 함.
    }

    // 아이템 점유 사실을 해당 영역 셀들에게 업데이트하는 메서드 입니다
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

                // 1일 경우 해당 셀에 점유 설정
                if (itemShapeArray[x,y] == '1')
                {
                    if(isBag)
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingBag(this);
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.SetOccupyingItem(this);
                }
                // 0일 경우 해당 셀에 점유 해제
                else
                {                 
                    if (isBag)
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingBag();
                    else
                        Inventory.Instance.GetInventoryCellByPos(actualCellPos)?.RemoveOccupyingItem();
                }
            }
        }
    }

    // 아이템 터치 가능 영역을 초기화하는 메서드 입니다
    private void InitTouchArea()
    {
        int areaCenterOffset = 200;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (itemShapeArray[x, y] == '1')
                {
                    // 1일 경우 해당 포지션에 터치 가능 영역 생성
                    GameObject touchAreaObject = Instantiate(touchAreaPrefab, touchAreas.transform);
                    touchAreaObject.transform.localPosition = new Vector2((x * Inventory.CELL_SIZE - areaCenterOffset), -(y * Inventory.CELL_SIZE - areaCenterOffset));
                }
            }
        }
    }
}