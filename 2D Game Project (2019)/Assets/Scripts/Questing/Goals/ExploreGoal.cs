using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreGoal : Goal
{
    public string LocationToExplore { get; set; }

    public ExploreGoal(Quest _quest, string _locationToExplore, int _requiredAmount)
    {
        Quest = _quest;
        LocationToExplore = _locationToExplore;
        RequiredAmount = _requiredAmount;
        CurrentAmount = 0;
    }

    public override void Initialise()
    {
        base.Initialise();
        GameManager.instance.OnLocationEnteredCallback += LocationReached;
    }

    private void LocationReached(string locationName)
    {
        if (locationName == LocationToExplore)
        {
            CurrentAmount++;
            Evaluate();
        }
    }

    public override string RetrieveGoalInfo(string variableNameToReturn)
    {
        string value = string.Empty;

        switch (variableNameToReturn)
        {
            case "LocationToExplore":
                value = LocationToExplore;
                break;
            default:
                break;
        }

        return value;
    }
}
