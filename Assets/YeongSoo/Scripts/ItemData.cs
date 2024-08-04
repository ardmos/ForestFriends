using UnityEngine;

[System.Serializable]
public class ItemData
{
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    // 아이템 스펙
    public int itemID; // 아이템의 고유 값
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
    public string itemShape; // 아이템의 형태. 5x5 구조로 표현된다. ex) 검 : 00000 00100 00100 00100 00000 

    // 아이템 고유 해시값 확인을 위한 부분
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