using UnityEngine;

/*[System.Serializable]
public class ItemData*/
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int itemID;
    public string itemName;
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    // 아이템 ID를 읽기 전용으로 제공
    public int ID => itemID;

    public void SetID(int newID)
    {
        itemID = newID;
    }

    // - 아이템 고유 해시값 확인을 위한 부분 -
    // Equals 메서드 오버라이드
    public override bool Equals(object obj)
    {
        return Equals(obj as ItemData);
    }

    // IEquatable<T> 인터페이스 구현
    public bool Equals(ItemData other)
    {
        if (other == null)
            return false;

        return this.itemID == other.itemID;
    }

    // GetHashCode 메서드 오버라이드
    public override int GetHashCode()
    {
        return itemID.GetHashCode();
    }
}