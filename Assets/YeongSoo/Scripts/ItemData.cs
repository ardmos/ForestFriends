using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemID;
    public ItemSpec itemSpec;
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    // - ������ ���� �ؽð� Ȯ���� ���� �κ� -
    // Equals �޼��� �������̵�
    public override bool Equals(object obj)
    {
        return Equals(obj as ItemData);
    }

    // IEquatable<T> �������̽� ����
    public bool Equals(ItemData other)
    {
        if (other == null)
            return false;

        return this.itemID == other.itemID;
    }

    // GetHashCode �޼��� �������̵�
    public override int GetHashCode()
    {
        return itemID.GetHashCode();
    }
}