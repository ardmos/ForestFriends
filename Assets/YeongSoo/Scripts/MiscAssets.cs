using UnityEngine;

[CreateAssetMenu(fileName = "MiscAssets", menuName = "Game/Misc Assets")]
public class MiscAssets : ScriptableObject
{
    [SerializeField] private Sprite[] miscImages;

    public Sprite GetGemImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= miscImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return miscImages[itemSpecID];
    }
}