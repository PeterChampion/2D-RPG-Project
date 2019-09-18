﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSlayer : Quest
{
    private void Start()
    {
        QuestName = "Monster Slayer";
        QuestDescription = "Monsters have invested the cavern, a bounty has been placed on the destruction of a number of them";
        ExperienceReward = 100;
        ItemReward = FindItem("Tutorial Sword +1");

        Goals.Add(new KillGoal(this, AI.EnemyType.Bat, 5));
        Goals.Add(new KillGoal(this, AI.EnemyType.Goblin, 3));
        Goals.Add(new KillGoal(this, AI.EnemyType.Kobold, 2));
        Goals.Add(new KillGoal(this, AI.EnemyType.Ogre, 1));

        Goals.ForEach(goal => goal.Initialise());
    }
}