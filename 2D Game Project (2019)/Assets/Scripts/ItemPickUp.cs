﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int interactionRange = 2;
    private GameObject player;
    [SerializeField] private GameObject interactionPrompt;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        interactionPrompt.SetActive(false);
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
            if (Inventory.instance.AddToInventory(item))
            {
                Destroy(gameObject);
            }
        }
    }
}
