using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Quest : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string QuestDescription { get; set; }
    public int ExperienceReward { get; set; }
    public Item ItemReward { get; set; }
    public bool IsCompleted { get; set; }

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

        // TO DO: Add experience reward to player
    }
}
