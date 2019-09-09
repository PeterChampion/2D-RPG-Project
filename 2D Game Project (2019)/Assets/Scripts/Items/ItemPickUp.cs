using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles picking up a item from a 2D sprite that the player character is within range of, presents a prompt to the player that an interaction is available.
public class ItemPickUp : Interactable
{
    [SerializeField] private Item item = null;

    protected override void Effect()
    {
        if (Inventory.instance.AddToInventory(item))
        {
            Destroy(gameObject);
        }
    }
}
