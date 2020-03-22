using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;
    public delegate void OnShopChanged(Item item);
    public OnShopChanged onShopChangedCallback;
    public int maxShopSpace = 20; // Possibly change later, allow endless, scrolling shop?
    public List<Item> shopItems = new List<Item>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public bool AddToShop(Item item)
    {
        if (shopItems.Count >= maxShopSpace)
        {
            return false;
        }
        else
        {
            shopItems.Add(item);
            onShopChangedCallback?.Invoke(item);
            return true;
        }
    }

    public void RemoveFromShop(Item item)
    {
        shopItems.Remove(item);
        onShopChangedCallback?.Invoke(item);
    }

    public void ClearShopContents()
    {
        List<Item> ItemsToRemove = new List<Item>();

        foreach (Item item in shopItems)
        {
            ItemsToRemove.Add(item);
            onShopChangedCallback?.Invoke(item);
        }

        foreach (Item item in ItemsToRemove)
        {
            RemoveFromShop(item);
            onShopChangedCallback?.Invoke(item);
        }
    }

    public bool IsItemInShop(Item item)
    {
        if (shopItems.Contains(item))
        {
            foreach (ShopSlot slot in ShopUI.instance.shopSlots)
            {
                if (slot.item == item)
                {
                    slot.quantityInStock++;
                    break;
                }
            }
            return true;
        }
        else
        {
            AddToShop(item);

            foreach (ShopSlot slot in ShopUI.instance.shopSlots)
            {
                if (slot.item == item)
                {
                    slot.quantityInStock = 1;
                    slot.gameObject.SetActive(true);
                    break;
                }
            }
            return false;
        }
    }
}
