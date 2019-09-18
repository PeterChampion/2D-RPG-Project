using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSlot : MonoBehaviour
{
    public Quest quest;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI questProgress;
    public TextMeshProUGUI questReward;

    private void Update()
    {
        if (!quest.IsRewardCollected)
        {
            questName.text = quest.QuestName;
            questDescription.text = quest.QuestDescription;
            questProgress.text = QuestGoalsProgress();
            questReward.text = quest.ExperienceReward + "xp";

            if (quest.ItemReward != null)
            {
                questReward.text = questReward.text + "\n" + quest.ItemReward.itemName;
            }
        }

        if (quest.IsRewardCollected)
        {
            Destroy(gameObject);
        }
    }

    private string QuestGoalsProgress()
    {
        string text = string.Empty;

        foreach (Goal goal in quest.Goals)
        {
            if (goal is CollectionGoal)
            {
                text = text + "\n" + goal.CurrentAmount + "/" + goal.RequiredAmount + " " + goal.RetrieveGoalInfo("ItemName");
                if (goal.RequiredAmount > 1)
                {
                    text = text + "s";
                }
            }
            else if (goal is KillGoal)
            {
                text = text + "\n" + goal.CurrentAmount + "/" + goal.RequiredAmount + " " + goal.RetrieveGoalInfo("EnemyType");
                if (goal.RequiredAmount > 1)
                {
                    text = text + "s";
                }
            }
        }

        return text;
    }
}
