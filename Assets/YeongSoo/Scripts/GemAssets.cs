using UnityEngine;

[CreateAssetMenu(fileName = "GemAssets", menuName = "Game/Gem Assets")]
public class GemAssets : ScriptableObject
{
    [SerializeField] private Sprite[] gemImages;

    public Sprite GetGemImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= gemImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return gemImages[itemSpecID];
    }
}