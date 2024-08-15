using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2 originalPosition;
    public Transform originalParent;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public GameObject touchAreas;
    public GameObject touchAreaPrefab;

    [SerializeField] private bool isBag;
    private ItemData itemData;
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
            // 원위치로 복귀
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        Inventory.Instance.OnDrop(cell, this); // 인벤토리에 드랍 작업 실행
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