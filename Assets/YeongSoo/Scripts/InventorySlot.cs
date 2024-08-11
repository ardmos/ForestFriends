using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private bool isSlotOn;
    public Image image;
    public Sprite onSprite;
    public Sprite offSprite;
    public Vector2 slotPos = Vector2.zero;

    public void SlotOn()
    {
        isSlotOn = true;
        image.sprite = onSprite;
    }

    public void SlotOff() 
    { 
        isSlotOn = false;
        image.sprite = offSprite;
    }

    public bool IsSlotOn()
    {
        return isSlotOn;
    }

}
