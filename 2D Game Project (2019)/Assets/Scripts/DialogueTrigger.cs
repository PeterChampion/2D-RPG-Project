using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // Dialogue of the DialogueTrigger
    private GameObject player;
    [SerializeField] private float interactionRange = 2.5F;
    public static bool dialogueOpen; // Used so that multiple DialogueTrigger's can exist in the same scene without conflicting based on their different distances to the player
    public bool recentlyInteracted; // Used to flag which specific DialogueTrigger is being interacted with
    public GameObject interactionPrompt;

    // Set up references
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
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
                TriggerDialogue();
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
    public void TriggerDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    // Uses 'DialogueManager' singleton to end dialogue
    private void EndDialogue()
    {
        DialogueManager.instance.EndDialogue();
    }
}
