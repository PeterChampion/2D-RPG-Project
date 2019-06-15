using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button removeButton;
    private Item item;

    public void AddItem(Item newItem)
    {
        Debug.Log("Adding item to UI...");
        item = newItem;
        icon.sprite = item.sprite;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        Debug.Log("Clearing item from UI...");
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Debug.Log("Removing item from inventory...");
        Inventory.instance.RemoveFromInventory(item);
    }

    public void UseItem()
    {
        Debug.Log("Attempting to use item...");
        if (item != null)
        {
            Debug.Log("Item used!");
            item.UseItem();
        }
    }
}
