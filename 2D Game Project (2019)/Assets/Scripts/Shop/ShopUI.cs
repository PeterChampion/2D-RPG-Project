using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopUI : MonoBehaviour
{
    public static ShopUI instance;

    private GameObject shopPanel;
    private Transform shopItemsArea;
    private Shop shop;
    public List<ShopSlot> shopSlots;
    private GameObject tooltip;

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

        shopPanel = GameObject.Find("ShopPanel");
        shopItemsArea = GameObject.Find("ShopItemsArea").transform;
        shopSlots = shopItemsArea.GetComponentsInChildren<ShopSlot>().ToList();
        tooltip = GameObject.Find("TooltipPanel");
    }

    // Start is called before the first frame update
    void Start()
    {
        shop = Shop.instance;
        shop.onShopChangedCallback += UpdateUI;
        shopPanel.SetActive(false);
        tooltip.SetActive(false);
        UpdateUI(null);
    }

    private void UpdateUI(Item item)
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            if (i < shop.shopItems.Count)
            {
                shopSlots[i].AddItem(shop.shopItems[i]);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }
}
