using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName = "New Item";
    public string description = "Description";
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
}
