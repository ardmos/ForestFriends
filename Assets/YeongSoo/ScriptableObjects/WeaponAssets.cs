using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAssets", menuName = "Game/Weapon Assets")]
public class WeaponAssets : ScriptableObject
{
    public Sprite[] WeaponImages;

    public Sprite GetWeaponImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= WeaponImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return WeaponImages[itemSpecID];
    }
}