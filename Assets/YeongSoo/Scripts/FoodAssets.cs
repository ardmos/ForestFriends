using UnityEngine;

[CreateAssetMenu(fileName = "FoodAssets", menuName = "Game/Food Assets")]
public class FoodAssets : ScriptableObject
{
    [SerializeField] private Sprite[] foodImages;

    public Sprite GetFoodImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= foodImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return foodImages[itemSpecID];
    }
}