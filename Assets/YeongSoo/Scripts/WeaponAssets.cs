using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAssets", menuName = "Game/Weapon Assets")]
public class WeaponAssets : ScriptableObject
{
    [SerializeField] private Sprite[] weaponImages;

    public Sprite GetWeaponImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= weaponImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return weaponImages[itemSpecID];
    }
}