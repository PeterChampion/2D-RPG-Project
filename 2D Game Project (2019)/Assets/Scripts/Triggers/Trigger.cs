using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> ObjectsToTrigger = new List<GameObject>();
    [SerializeField] private GameObject TriggerActivator;
    [SerializeField] private bool deactivateOnTrigger;
    [SerializeField] private float activationDelay;
    private bool triggered;

    private void Start()
    {
        if (!deactivateOnTrigger)
        {
            foreach (GameObject obj in ObjectsToTrigger)
            {
                obj.SetActive(false);
            }
        }
    }

    private void ActivateTrigger()
    {
        if (deactivateOnTrigger)
        {
            foreach (GameObject obj in ObjectsToTrigger)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in ObjectsToTrigger)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered)
        {
            triggered = true;
            if (collision.gameObject == TriggerActivator)
            {
                Invoke("ActivateTrigger", activationDelay);
                Destroy(this, activationDelay + 1);
            }
        }
    }    
}
