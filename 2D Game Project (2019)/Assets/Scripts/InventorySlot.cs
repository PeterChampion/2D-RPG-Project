using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image icon;
    private Button removeButton;
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.sprite;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.RemoveFromInventory(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.UseItem();
        }
    }
}
