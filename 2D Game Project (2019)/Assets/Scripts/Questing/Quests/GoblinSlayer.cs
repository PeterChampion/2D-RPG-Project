using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSlayer : Quest
{
    private void Start()
    {
        QuestName = "Goblin Slayer";
        QuestDescription = "Kill dem Gobbos!";
        ExperienceReward = 10000;

        Goals.Add(new KillGoal(this, AI.EnemyType.Goblin, 5));

        Goals.ForEach(goal => goal.Initialise());
    }
}
