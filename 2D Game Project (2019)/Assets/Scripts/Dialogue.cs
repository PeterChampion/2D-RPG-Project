using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles storing local dialogue, removing/clearing/replacing dialogue at runtime.
[System.Serializable] // Allows the class to be displayed in the Unity IDE
public class Dialogue
{
    public string name;
    [TextArea(3, 10)] // Increases the text area in the Unity IDE
    public List<string> sentences;

    // Adds a new string of dialogue to the sentences list which will be passed to a 'DialogueTrigger' to be displayed
    public void AddNewDialogue(string Dialogue)
    {
        sentences.Add(Dialogue);
    }

    // Replaces existing dialogue on a 'DialogueTrigger' - Useful for when dialogue needs to be updated after certain actions/completion of a task
    public void ReplaceDialogue(string Dialogue)
    {
        ClearDialogue();
        sentences.Add(Dialogue);
    }

    // Clears the existing dialogue of all sentences
    public void ClearDialogue()
    {
        for (int i = 0; i < sentences.Count; i++)
        {
            sentences.Clear();
        }
    }
}
