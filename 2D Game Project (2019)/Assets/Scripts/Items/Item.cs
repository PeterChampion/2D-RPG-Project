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
    public enum RarityTier { Common, Uncommon, Rare, VeryRare, Legendary };
    public RarityTier rarity;
    public static List<Item> allItems = new List<Item>();
    public static List<Item> commonItems = new List<Item>();
    public static List<Item> uncommonItems = new List<Item>();
    public static List<Item> rareItems = new List<Item>();
    public static List<Item> veryRareItems = new List<Item>();
    public static List<Item> legendaryItems = new List<Item>();

    private void OnEnable()
    {
        allItems.Add(this);

        switch (rarity)
        {
            case RarityTier.Common:
                commonItems.Add(this);
                break;
            case RarityTier.Uncommon:
                uncommonItems.Add(this);
                break;
            case RarityTier.Rare:
                rareItems.Add(this);
                break;
            case RarityTier.VeryRare:
                veryRareItems.Add(this);
                break;
            case RarityTier.Legendary:
                legendaryItems.Add(this);
                break;
        }
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
