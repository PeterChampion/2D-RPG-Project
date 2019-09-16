using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal
{
    public Quest Quest { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int RequiredAmount { get; set; }
    public int CurrentAmount { get; set; }

    public virtual void Initialise()
    {
        // Default init stuff
    }

    public void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Debug.Log("Goal Completed!");
        IsCompleted = true;
        Quest.CheckGoals();
    }
}
