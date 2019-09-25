using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal
{
    public Quest Quest { get; set; }
    public bool IsCompleted { get; set; }
    public int RequiredAmount { get; set; }
    public int CurrentAmount { get; set; }

    public virtual void Initialise()
    {
        // Intended to be over written by derived classes
    }

    public void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
        else
        {
            IsCompleted = false;
            Quest.CheckGoals();
        }
    }

    public void Complete()
    {
        Debug.Log("Goal Completed!");
        IsCompleted = true;
        Quest.CheckGoals();
    }

    public virtual string RetrieveGoalInfo(string variableNameToReturn)
    {
        string value = string.Empty;
        return value;
        // Intended to be over written by derived classes
    }
}
