using UnityEngine;

[System.Serializable]
public class ItemData
{
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    // ������ ����
    public int itemID; // �������� ���� ��
    public string itemName = "";
    public int itemPrice;
    public string itemDescription;
    public float attack;
    public float defence;
    public float attackSpeed;
    public float healingAmount;
    public string itemType;
    public string typeMainStat;
    public string typeSubStat;
    public string itemShape; // �������� ����. 5x5 ������ ǥ���ȴ�. ex) �� : 00000 00100 00100 00100 00000 

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
        return (itemName, currentCellPos, targetCellPos).GetHashCode();
    }
}