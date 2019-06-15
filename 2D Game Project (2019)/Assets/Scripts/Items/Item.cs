using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite sprite = null;
    public int goldValue = 0;

    public virtual void UseItem()
    {
        // Intended to be overwritten
        RemoveFromInventory();
    }

    public void RemoveFromInventory()
    {
        // To be done
    }

    public virtual string GetTooltipInfo()
    {
        // Intended to be overwritten
        string tooltipInfo = "Default Tooltip Information";
        return tooltipInfo;
    }
}
