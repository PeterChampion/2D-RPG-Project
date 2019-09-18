using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A trigger for dialogue that runs on a 2D element, when the player is within the interaction range a prompt is displayed, when the interaction key is pressed the dialogue specific to the object is shown and looped through.
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] protected new string name;
    [SerializeField] protected Dialogue standardDialogue = new Dialogue(); // Dialogue of the DialogueTrigger
    protected GameObject player;
    [SerializeField] protected float interactionRange = 2.5F;
    public static bool dialogueOpen; // Used so that multiple DialogueTrigger's can exist in the same scene without conflicting based on their different distances to the player
    protected bool recentlyInteracted; // Used to flag which specific DialogueTrigger is being interacted with
    [SerializeField] protected GameObject interactionPrompt = null;

    // Set up references
    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        CheckForInteraction();
    }

    protected virtual void CheckForInteraction()
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
                TriggerDialogue(standardDialogue);
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

    // Uses 'DialogueManager' singleton to start dialogue
    protected void TriggerDialogue(Dialogue dialogue)
    {
        DialogueManager.instance.StartDialogue(dialogue, name);
    }

    // Uses 'DialogueManager' singleton to end dialogue
    protected void EndDialogue()
    {
        DialogueManager.instance.EndDialogue();
    }
}
