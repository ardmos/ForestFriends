using UnityEngine;

[CreateAssetMenu(fileName = "GemAssets", menuName = "Game/Gem Assets")]
public class GemAssets : ScriptableObject
{
    [SerializeField] private Sprite[] gemImages;

    public Sprite GetGemImageBySpecID(int itemSpecID)
    {
        // ���� ó�� ���� �߰� (itemSpecID�� 0�� ������� �ʽ��ϴ�)
        if (itemSpecID < 1 || itemSpecID >= gemImages.Length)
        {
            Debug.LogWarning($"Invalid itemSpecID: {itemSpecID}");
            return null;
        }
        return gemImages[itemSpecID];
    }
}