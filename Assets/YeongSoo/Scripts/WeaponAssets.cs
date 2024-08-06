using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAssets", menuName = "Game/Weapon Assets")]
public class WeaponAssets : ScriptableObject
{
    [SerializeField] private Sprite[] weaponImages;

    public Sprite GetWeaponImageBySpecID(int itemSpecID)
    {
        // 예외 처리 로직 추가 (itemSpecID로 0은 사용하지 않습니다)
        if (itemSpecID < 1 || itemSpecID >= weaponImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return weaponImages[itemSpecID];
    }
}