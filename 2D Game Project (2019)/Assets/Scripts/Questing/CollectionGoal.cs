using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : Goal
{
    public string ItemName { get; set; }

    public CollectionGoal(Quest _quest, string _itemName, string _description, int _currentAmount, int _requiredAmount)
    {
        Quest = _quest;
        ItemName = _itemName;
        Description = _description;
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
                Debug.Log("Matching Item Found");
                itemCount++;
            }
        }
        CurrentAmount = itemCount;
        Evaluate();
    }
}
