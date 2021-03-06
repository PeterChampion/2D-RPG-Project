﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string QuestDescription { get; set; }
    public int ExperienceReward { get; set; }
    public int GoldReward { get; set; }
    public Item ItemReward { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRewardCollected { get; set; }
    public List<GameObject> questTriggers = new List<GameObject>();

    public void CheckGoals()
    {
        IsCompleted = Goals.All(goal => goal.IsCompleted);      
    }

    public virtual void GiveReward()
    {
        if (ItemReward != null)
        {
            Inventory.instance.AddToInventory(ItemReward);
        }
        Inventory.instance.gold += GoldReward;
        GameManager.instance.player.Experience += ExperienceReward;
        GameManager.instance.player.DisplayQuestReward(ExperienceReward, GoldReward);
        GameManager.instance.player.OnExperienceGainCallback.Invoke();
        IsRewardCollected = true;

        TriggerOnCompletion();
    }

    protected Item FindItem(string itemName)
    {
        Item itemFound = null;

        foreach (Item item in Item.allItems)
        {
            if (item.itemName == itemName)
            {
                itemFound = item;
            }
        }

        return itemFound;
    }

    protected void TriggerOnCompletion()
    {
        if (questTriggers.Count > 0)
        {
            foreach (GameObject obj in questTriggers)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}
