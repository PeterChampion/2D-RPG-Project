using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCollector : Quest
{
    [SerializeField] private Item itemReward;

    private void Start()
    {
        QuestName = "Goblin Slayer";
        QuestDescription = "Slay some Goblins!";
        ExperienceReward = 500;
        ItemReward = ItemReward;
        Goals.Add(new CollectionGoal(this, "Potion of Health", "Collect a health potion", false, 0, 2));

        Goals.ForEach(goal => goal.Initialise());
    }

    public override void GiveReward()
    {
        RemoveFromInventory("Potion of Health", 2);
        base.GiveReward();
    }

    public void RemoveFromInventory(string itemToRemove, int amountToRemove)
    {
        Inventory inventory = Inventory.instance;
        List<Item> itemsToRemove = new List<Item>();

        for (int i = 0; i < inventory.inventoryItems.Count; i++)
        {
            if (i <= inventory.inventoryItems.Count)
            {
                if (inventory.inventoryItems[i] != null)
                {
                    if (inventory.inventoryItems[i].itemName == itemToRemove)
                    {
                        itemsToRemove.Add(inventory.inventoryItems[i]);
                        print("Removed an item");
                        amountToRemove--;
                    }
                }
            }                
        }        

        foreach (Item item in itemsToRemove)
        {
            inventory.RemoveFromInventory(item);
        }        
    }
}
