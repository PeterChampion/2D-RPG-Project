using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSlayer : Quest
{
    [SerializeField] private Item itemReward;

    private void Start()
    {
        QuestName = "Goblin Slayer";
        QuestDescription = "Slay some Goblins!";
        ExperienceReward = 100;
        ItemReward = ItemReward;

        Goals.Add(new KillGoal(this, AI.EnemyType.Goblin, "Kill 3 Goblins", 3));

        Goals.ForEach(goal => goal.Initialise());
    }
}
