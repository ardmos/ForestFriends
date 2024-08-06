[System.Serializable]
public class ItemSpec
{
    // ������ ����
    public int itemSpecID; // ������ ���� �������� ���� ��
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
    public string itemShape; // �������� ����. 5x5 ������ ǥ���ȴ�. ex) �� : 00000 00100 00100 00100 00000 

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
