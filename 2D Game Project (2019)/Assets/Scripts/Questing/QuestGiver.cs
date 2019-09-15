using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : DialogueTrigger
{
    private GameObject quests;
    [SerializeField] private string QuestToAssign;
    public Quest Quest { get; set; }

    public bool AssignedQuest { get; set; }
    public bool QuestHandedIn { get; set; }
    [SerializeField] protected Dialogue inProgressDialogue = new Dialogue();
    [SerializeField] protected Dialogue rewardDialogue = new Dialogue();
    [SerializeField] protected Dialogue completedDialogue = new Dialogue();

    private void Start()
    {
        quests = GameObject.Find("Quests");
    }

    protected override void CheckForInteraction()
    {
        // If the player is within the interaction range on the X axis AND the Y axis AND the dialogue IS NOT open...
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < interactionRange && Mathf.Abs(transform.position.y - player.transform.position.y) < interactionRange && dialogueOpen == false)
        {
            interactionPrompt.SetActive(true); // Toggle interactionPrompt to display
        }
        // If the player is outside of the interaction range on the X axis OR the Y axis AND the dialogue IS open...
        else if (Mathf.Abs(transform.position.x - player.transform.position.x) > interactionRange || Mathf.Abs(transform.position.y - player.transform.position.y) > interactionRange && dialogueOpen == true)
        {
            interactionPrompt.SetActive(false); // Hide interactionPrompt as we are outside of the interactionRange
            if (recentlyInteracted) // If we have been interacted with...
            {
                EndDialogue();
                dialogueOpen = false;
            }
            recentlyInteracted = false; // Flag as no longer having been interacted with - prevents the above if statement running multiple times
        }
        // If the player is within the interaction range on the X axis AND the Y axis AND presses the 'E' key...
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < interactionRange && Mathf.Abs(transform.position.y - player.transform.position.y) < interactionRange && Input.GetKeyDown(KeyCode.E))
        {
            interactionPrompt.SetActive(false); // Hide interactionPrompt as we are already interacting with the DialogueTrigger
            recentlyInteracted = true;            

            // If the dialogue isn't already open...
            if (!dialogueOpen)
            {
                if (!AssignedQuest && !QuestHandedIn)
                {
                    AssignQuest();
                    // Assign Quest Dialoague
                    TriggerDialogue(standardDialogue);
                }
                else if (AssignedQuest && !QuestHandedIn)
                {
                    // Check completion
                    // In Progress Dialogue                    
                    Inventory.instance.onItemChangedCallback(null);
                    CheckQuest();

                    if (Quest.IsCompleted)
                    {
                        // Completion Dialogue
                        TriggerDialogue(rewardDialogue);
                    }
                    else
                    {
                        // In Progress Dialogue
                        TriggerDialogue(inProgressDialogue);
                    }
                }
                else
                {
                    // Completed Dialogue
                    TriggerDialogue(completedDialogue);
                }

                dialogueOpen = true;
            }
            // Else if the dialogue is already open...
            else if (dialogueOpen)
            {
                // If there is no dialogue left...
                if (DialogueManager.instance.sentences.Count == 0)
                {
                    EndDialogue();
                    recentlyInteracted = false;
                    dialogueOpen = false;
                    interactionPrompt.SetActive(false);
                }
                // Else if there is dialogue left...
                else
                {
                    DialogueManager.instance.DisplayNextSentence();
                }
            }
        }
    }

    private void AssignQuest()
    {
        AssignedQuest = true;
        Quest = (Quest)quests.AddComponent(System.Type.GetType(QuestToAssign));
    }

    private void CheckQuest()
    {
        if (Quest.IsCompleted)
        {
            Quest.GiveReward();
            QuestHandedIn = true;
            Debug.Log("Quest completed!");
        }
    }
}
