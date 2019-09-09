using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodlock : MonoBehaviour
{
    [SerializeField] private List<GameObject> ObjectsToTrigger = new List<GameObject>();
    [SerializeField] private List<GameObject> BloodlockObjects = new List<GameObject>();
    [SerializeField] private bool deactivateOnTrigger;
    [SerializeField] private float activationDelay;
    private bool triggered;

    private void Start()
    {
        InvokeRepeating("CheckBloodlockCount", 0, 1);

        if (!deactivateOnTrigger)
        {
            foreach (GameObject obj in ObjectsToTrigger)
            {
                obj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (BloodlockObjects.Count <= 0 && !triggered)
        {
            triggered = true;
            Invoke("ActivateBloodlock", activationDelay);
        }
    }

    private int CheckBloodlockCount()
    {
        if (!triggered)
        {
            for (int i = 0; i < BloodlockObjects.Count; i++)
            {
                if (BloodlockObjects[i] == null)
                {
                    BloodlockObjects.Remove(BloodlockObjects[i]);
                }
            }
        }
        return BloodlockObjects.Count;
    }    

    private void ActivateBloodlock()
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
        Destroy(this, activationDelay + 1);
    }
}
