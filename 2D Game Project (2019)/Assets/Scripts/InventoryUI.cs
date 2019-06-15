using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private GameObject InventoryPanel;
    private Transform itemsArea;
    private Inventory inventory;
    private InventorySlot[] inventorySlots;
    private GameObject tooltip;

    private void Awake()
    {
        InventoryPanel = GameObject.Find("InventoryPanel");
        itemsArea = GameObject.Find("ItemsArea").transform;
        inventorySlots = itemsArea.GetComponentsInChildren<InventorySlot>();
        tooltip = GameObject.Find("TooltipPanel");
    }

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        InventoryPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && DialogueTrigger.dialogueOpen == false)
        {
            tooltip.SetActive(false);
            GameManager.instance.TogglePauseState();
            InventoryPanel.SetActive(!InventoryPanel.activeSelf);
        }
    }

    private void UpdateUI()
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
