using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopTrigger : Interactable
{
    public List<Item> itemsForSale = new List<Item>();
    public List<int> itemQuantitys = new List<int>();
    private GameObject inventoryPanel;
    private GameObject tooltip;
    private static bool shopOpen;

    protected override void Awake()
    {
        tooltip = GameObject.Find("TooltipPanel");
        inventoryPanel = GameObject.Find("InventoryPanel");
        base.Awake();
    }

    protected override void Effect()
    {
        GameManager.instance.shopWindow.SetActive(!GameManager.instance.shopWindow.activeSelf);
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        GameManager.instance.TogglePauseState();

        if (GameManager.instance.shopWindow.activeSelf)
        {
            shopOpen = true;

            for (int index = 0; index < itemsForSale.Count; index++)
            {
                Shop.instance.AddToShop(itemsForSale[index]);
                ShopUI.instance.shopSlots[index].quantityInStock = itemQuantitys[index];
                ShopUI.instance.shopSlots[index].gameObject.SetActive(true);
            }
        }
        else
        {
            shopOpen = false;
            UpdateStock(itemQuantitys.Count);
            Shop.instance.ClearShopContents();
        }

        if (tooltip.activeSelf)
        {
            tooltip.SetActive(false);
        }

        base.Effect();
    }

    private void UpdateStock(int numberToCheck)
    {
        List<Item> itemsToRemove = new List<Item>();

        for (int index = 0; index < itemsForSale.Count; index++)
        {
            itemQuantitys[index] = ShopUI.instance.shopSlots[index].quantityInStock;

            if (itemQuantitys[index] <= 0)
            {
                itemsToRemove.Add(itemsForSale[index]);
            }
        }

        foreach (Item item in itemsToRemove)
        {
            itemsForSale.Remove(item);
        }

        for (int index = numberToCheck - 1; index >= 0; index--)
        {
            print("Ran");

            if (itemQuantitys[index] <= 0)
            {
                itemQuantitys.Remove(itemQuantitys[index]);
                print("Removed");
            }
        }
    }
}
