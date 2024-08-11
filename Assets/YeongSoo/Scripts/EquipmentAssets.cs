using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentAssets", menuName = "Game/Equipment Assets")]
public class EquipmentAssets : ScriptableObject
{
    [SerializeField] private Sprite[] equipmentImages;

    public Sprite GetEquipmentImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= equipmentImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return equipmentImages[itemSpecID];
    }
}