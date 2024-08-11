using UnityEngine;

[CreateAssetMenu(fileName = "BagAssets", menuName = "Game/Bag Assets")]
public class BagAssets : ScriptableObject
{
    [SerializeField] private Sprite[] bagImages;

    public Sprite GetBagImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= bagImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return bagImages[itemSpecID];
    }
}
