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

    // ������ ID�� �б� �������� ����
    public int ID => itemID;

    public void SetID(int newID)
    {
        itemID = newID;
    }

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