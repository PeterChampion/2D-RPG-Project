using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected int interactionRange = 2;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected GameObject interactionPrompt = null;

    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        interactionPrompt.SetActive(false);
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
    }

    private void Update()
    {
        Interaction();
    }

    private void Interaction()
    {
        // If the player is INSIDE the interaction range on the X AND Y axis...
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < interactionRange && Mathf.Abs(transform.position.y - player.transform.position.y) < interactionRange)
        {
            interactionPrompt.SetActive(true); // Toggle interactionPrompt to display
        }
        // If the player is OUTSIDE the interaction range on the X OR Y axis...
        else if (Mathf.Abs(transform.position.x - player.transform.position.x) > interactionRange || Mathf.Abs(transform.position.y - player.transform.position.y) > interactionRange)
        {
            interactionPrompt.SetActive(false); // Toggle interactionPrompt to display
        }

        // If the interactionPrompt is active and the 'E' key is pressed...
        if (interactionPrompt.activeSelf == true && Input.GetKeyDown(KeyCode.E))
        {
            Effect();
        }
    }

    protected virtual void Effect()
    {
        // Intended to be overwritten by derived classes
    }
}
