using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : Goal
{
    public string ItemName { get; set; }

    public CollectionGoal(Quest _quest, string _itemName, string _description, bool _completed, int _currentAmount, int _requiredAmount)
    {
        Quest = _quest;
        ItemName = _itemName;
        Description = _description;
        IsCompleted = _completed;
        RequiredAmount = _requiredAmount;
        CurrentAmount = _currentAmount;
    }

    public override void Initialise()
    {
        base.Initialise();
        Inventory.instance.onItemChangedCallback += CheckInventory;
    }

    private void CheckInventory(Item item)
    {
        int itemCount = 0;

        foreach (Item inventoryItem in Inventory.instance.inventoryItems)
        {
            if (inventoryItem.itemName == ItemName)
            {
                itemCount++;
            }
        }

        if (item.itemName == ItemName)
        {
            Debug.Log("Correct Item Found");
            CurrentAmount++;
            Evaluate();
        }

        CurrentAmount = itemCount;
    }


}
