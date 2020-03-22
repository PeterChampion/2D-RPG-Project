using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCollector : Quest
{
    private void Start()
    {
        QuestName = "Potion Collector";
        QuestDescription = "Collect health and stamina potions to prepare yourself for the journey ahead.";
        ExperienceReward = 2500;
        GoldReward = 100;
        ItemReward = FindItem("Tutorial Sheild +1");
        Goals.Add(new CollectionGoal(this, "Health Potion", 2));
        Goals.Add(new CollectionGoal(this, "Stamina Potion", 2));

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
