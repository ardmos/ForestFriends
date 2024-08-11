using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentAssets", menuName = "Game/Equipment Assets")]
public class EquipmentAssets : ScriptableObject
{
    [SerializeField] private Sprite[] equipmentImages;

    public Sprite GetEquipmentImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= equipmentImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return equipmentImages[itemSpecID];
    }
}