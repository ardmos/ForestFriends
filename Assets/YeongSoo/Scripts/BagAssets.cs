using UnityEngine;

[CreateAssetMenu(fileName = "BagAssets", menuName = "Game/Bag Assets")]
public class BagAssets : ScriptableObject
{
    [SerializeField] private Sprite[] bagImages;

    public Sprite GetBagImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= bagImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return bagImages[itemSpecID];
    }
}
