using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverInfestedCaverns : Quest
{
    private void Start()
    {
        QuestName = "Explore Infested Caverns";
        QuestDescription = "Exploration is fun! Especially in the direction of danger!";
        ExperienceReward = 10000;

        Goals.Add(new ExploreGoal(this, "Tutorial Infested Cavern", 1));

        Goals.ForEach(goal => goal.Initialise());
    }
}
