using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles displaying the inventory when the inventory key is pressed, loops through the inventory to display the contents and which slots are filled/empty.
public class InventoryUI : MonoBehaviour
{
    private GameObject inventoryPanel;
    private Transform itemsArea;
    private Inventory inventory;
    private InventorySlot[] inventorySlots;
    private GameObject tooltip;

    private void Awake()
    {
        inventoryPanel = GameObject.Find("InventoryPanel");
        itemsArea = GameObject.Find("ItemsArea").transform;
        inventorySlots = itemsArea.GetComponentsInChildren<InventorySlot>();
        tooltip = GameObject.Find("TooltipPanel");
    }

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false);
        tooltip.SetActive(false);
        inventory.open = false;
        UpdateUI(null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && DialogueTrigger.dialogueOpen == false && !GameManager.instance.pausePanel.activeSelf && !GameManager.instance.questLog.activeSelf && !GameManager.instance.shopWindow.activeSelf)
        {
            tooltip.SetActive(false);
            GameManager.instance.TogglePauseState();
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            inventory.open = !inventory.open;
        }
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
