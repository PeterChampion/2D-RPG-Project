using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private GameObject InventoryPanel;
    private Transform itemsArea;
    private Inventory inventory;
    private InventorySlot[] inventorySlots;

    private void Awake()
    {
        InventoryPanel = GameObject.Find("InventoryPanel");
        itemsArea = GameObject.Find("ItemsArea").transform;
        inventorySlots = itemsArea.GetComponentsInChildren<InventorySlot>();
    }

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        UpdateUI(null);
    }

    private void UpdateUI(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.inventoryItems.Count)
            {
                inventorySlots[i].AddItem(inventory.inventoryItems[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }
}
