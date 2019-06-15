using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
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
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < interactionRange && Mathf.Abs(transform.position.y - player.transform.position.y) < interactionRange)
        {

        }
    }
}
