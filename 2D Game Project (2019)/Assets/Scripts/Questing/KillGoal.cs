using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGoal : Goal
{
    public AI.EnemyType EnemyType { get; set; }

    public KillGoal(Quest _quest, AI.EnemyType _enemyType, int _requiredAmount)
    {
        Quest = _quest;
        EnemyType = _enemyType;
        RequiredAmount = _requiredAmount;
        CurrentAmount = 0;
    }

    public override void Initialise()
    {
        base.Initialise();
        GameManager.instance.OnCharacterDeathCallback += EnemyDied;
    }

    private void EnemyDied(AI.EnemyType type)
    {
        if (type == EnemyType)
        {
            Debug.Log("Progress made!");
            CurrentAmount++;
            Evaluate();
        }
    }



    public override string RetrieveGoalInfo(string variableNameToReturn)
    {
        string value = string.Empty;

        switch (variableNameToReturn)
        {
            case "EnemyType":
                value = EnemyType.ToString();
                break;
            default:
                break;
        }

        return value;
    }
}
