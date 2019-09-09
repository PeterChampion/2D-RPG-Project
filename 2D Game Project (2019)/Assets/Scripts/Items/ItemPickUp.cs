using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles picking up a item from a 2D sprite that the player character is within range of, presents a prompt to the player that an interaction is available.
public class ItemPickUp : Interactable
{
    [SerializeField] private Item item = null;

    protected override void Effect()
    {
        base.Effect();
        if (Inventory.instance.AddToInventory(item))
        {
            interactionPrompt.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            interactionRange = 0;
            Destroy(gameObject, 1);
        }
    }
}
