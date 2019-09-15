using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base item class, provides each item with a name, sprite & gold value, as well as the functionality to be used, removed from inventory and gather the relevant information needed for a tooltip for future
// derived classes to use.
public abstract class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite sprite = null;
    public int goldValue = 0;

    private void OnEnable()
    {
        GameManager.instance.ItemsInGame.Add(this);
        Debug.Log(itemName + " added to the list!");
    }

    public virtual void Use()
    {
        // Intended to be overwritten
        RemoveFromInventory();
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.RemoveFromInventory(this);
    }

    public virtual string GetTooltipInfo()
    {
        // Intended to be overwritten
        string tooltipInfo = "Default Tooltip Information";
        return tooltipInfo;
    }
}
