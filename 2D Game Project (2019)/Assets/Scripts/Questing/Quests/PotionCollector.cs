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
        ExperienceReward = 100;
        ItemReward = ItemReward;
        Goals.Add(new CollectionGoal(this, "Potion of Health", "Collect a health potion", false, 0, 1));

        Goals.ForEach(goal => goal.Initialise());
    }

    public override void GiveReward()
    {
        base.GiveReward();
        RemoveFromInventory("Potion of Health", 1);
    }

    public void RemoveFromInventory(string itemToRemove, int amountToRemove)
    {
        Inventory inventory = Inventory.instance;

        for (int i = 0; i < inventory.inventoryItems.Count; i++)
        {
            if (inventory.inventoryItems[i].itemName == itemToRemove)
            {
                inventory.RemoveFromInventory(inventory.inventoryItems[i]);
                amountToRemove--;
            }

            if (amountToRemove <= 0)
            {
                return;
            }
        }
    }
}
