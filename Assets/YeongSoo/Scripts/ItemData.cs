using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName = "";
    public Vector2 currentCellPos = new Vector2();
    public Vector2 targetCellPos = new Vector2();

    public override bool Equals(object obj)
    {
        if (obj is ItemData other)
        {
            return this.itemName == other.itemName &&
                   this.currentCellPos == other.currentCellPos &&
                   this.targetCellPos == other.targetCellPos;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (itemName, currentCellPos, targetCellPos).GetHashCode();
    }
}