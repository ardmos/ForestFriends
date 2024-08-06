[System.Serializable]
public class ItemSpec
{
    // 아이템 스펙
    public int itemSpecID; // 아이템 스펙 데이터의 고유 값
    public string itemName;
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

    public ItemSpec()
    {
        itemSpecID = -1;
        itemName = string.Empty;
        itemPrice = 0;
        itemDescription = string.Empty;
        attack = 0;
        defence = 0;
        attackSpeed = 0;
        healingAmount = 0;
        itemType = string.Empty;
        typeMainStat = string.Empty;
        typeSubStat = string.Empty;
        itemShape = string.Empty;
    }
}
