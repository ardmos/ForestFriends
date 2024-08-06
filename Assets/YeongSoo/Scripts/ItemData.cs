using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemID;
    public ItemSpec itemSpec;
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

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