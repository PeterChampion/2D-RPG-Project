using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLever : Interactable
{
    private Animator animator;
    [SerializeField] private List<GameObject> Doors = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void Effect()
    {
        base.Effect();

        foreach (GameObject door in Doors)
        {
            if (door.activeSelf)
            {
                door.SetActive(false);
            }
            else
            {
                door.SetActive(true);
            }
        }
    }
}
