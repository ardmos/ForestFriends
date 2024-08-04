using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemID;
    public string name;
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    // ������ ���� �ؽð� Ȯ���� ���� �κ�
    public override bool Equals(object obj)
    {
        if (obj is ItemData other)
        {
            return this.itemID == other.itemID;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (name, currentCellPos, targetCellPos).GetHashCode();
    }
}