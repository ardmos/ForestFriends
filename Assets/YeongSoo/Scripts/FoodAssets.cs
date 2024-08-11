using UnityEngine;

[CreateAssetMenu(fileName = "FoodAssets", menuName = "Game/Food Assets")]
public class FoodAssets : ScriptableObject
{
    [SerializeField] private Sprite[] foodImages;

    public Sprite GetFoodImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= foodImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return foodImages[itemSpecID];
    }
}