using UnityEngine;

[CreateAssetMenu(fileName = "MiscAssets", menuName = "Game/Misc Assets")]
public class MiscAssets : ScriptableObject
{
    [SerializeField] private Sprite[] miscImages;

    public Sprite GetGemImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= miscImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return miscImages[itemSpecID];
    }
}