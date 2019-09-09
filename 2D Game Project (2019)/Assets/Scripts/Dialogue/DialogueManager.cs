using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Central class of the dialogue system, handles the dialogue box information and capability of storing dialogue sentences into a queue which is used to display dialogue in order.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance; // Singleton
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator dialogueAnimator;
    public Queue<string> sentences;
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] dialogueAudioClips;

    private void Awake()
    {
        if (instance == null) // If there is no singleton...
        {
            instance = this; // Set reference to this class
        }
        else if (instance != this) // If a singleton already exists...
        {
            Destroy(this); // Destroy this instance of this class to prevent duplicates
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        sentences = new Queue<string>(); // Initialise list
    }

    // Gains reference to all sentences that need to be displayed, begins the opening animation
    public void StartDialogue(Dialogue dialogue)
    {
        dialogueAnimator.SetBool("IsOpen", true); // Animation
        nameText.text = dialogue.name; // Reference to the name of who is speaking the dialogue
        sentences.Clear(); // Clears the queue of any previous sentences 

        // Queue up the sentences in their respective order
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    // Checks for if there is another sentence to display, if so, displays it, otherwise ends dialogue
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) // If there is no more dialogue to display...
        {
            EndDialogue();
            DialogueTrigger.dialogueOpen = false; // Flag 'DialogueTrigger's static bool as false
            return;
        }

        string sentence = sentences.Dequeue(); // Remove the top string from the queue and store a reference to it
        StopAllCoroutines(); // Stop any coroutines running as we are about to begin a new one
        StartCoroutine(TypeSentence(sentence)); // Type out the sentence to the screen using a coroutine
    }

    // Stops any coroutines running on the behaviour and performs the closing animation
    public void EndDialogue()
    {
        StopAllCoroutines();
        dialogueAnimator.SetBool("IsOpen",false);
    }

    private void DialogueSounds()
    {

    }

    // IEnumerator used for a coroutine, takes the current sentence and converts it into a char array - Each letter is printed to the screen with a short delay
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = ""; // Set dialogue text to be clear
        yield return new WaitForSeconds(0.6f); // Initial brief delay, used to attempt to sync up the first text displaying along with the opening animation

        foreach (char letter in sentence.ToCharArray()) // Conversion of string to char array
        {
            dialogueText.text += letter; // Add the letter to the end of the current dialogue
            int clipToPlay = Random.Range(0, dialogueAudioClips.Length);
            audioSource.clip = dialogueAudioClips[clipToPlay];
            audioSource.Play();
            yield return null; // Wait for a frame
        }
    }
}
