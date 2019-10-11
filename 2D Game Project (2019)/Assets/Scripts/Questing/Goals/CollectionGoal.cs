using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : Goal
{
    public string ItemName { get; set; }

    public CollectionGoal(Quest _quest, string _itemName, int _requiredAmount)
    {
        Quest = _quest;
        ItemName = _itemName;
        RequiredAmount = _requiredAmount;
        CurrentAmount = 0;
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

    public override string RetrieveGoalInfo(string variableNameToReturn)
    {
        string value = string.Empty;

        switch (variableNameToReturn)
        {
            case "ItemName":
                value = ItemName;
                break;
            default:
                break;
        }

        return value;
    }
}
