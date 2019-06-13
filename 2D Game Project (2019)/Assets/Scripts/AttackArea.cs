﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private Vector2 knockbackDirection;
    [SerializeField] private int damageValue = 10;
    [SerializeField] private float knockbackStrength = 5;
    public List<GameObject> affectedTargets;

    private void Update()
    {
        if (transform.position.x > transform.parent.position.x)
        {
            // Facing right
            knockbackDirection = Vector2.right;
        }
        else
        {
            // Facing left
            knockbackDirection = Vector2.left;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<AI>() || other.gameObject.GetComponent<PlayerController>())
        {
            bool targetAlreadyHit = false;
            foreach (GameObject target in affectedTargets)
            {
                if (target == other.gameObject)
                {
                    targetAlreadyHit = true;
                }
            }

            if (other.gameObject.GetComponent<AI>())
            {
                if (!targetAlreadyHit)
                {
                    other.gameObject.GetComponent<AI>().TakeDamage(damageValue);
                    other.gameObject.GetComponent<AI>().Knockback(knockbackDirection, knockbackStrength, 0.5f);
                }
            }
            else if (other.gameObject.GetComponent<PlayerController>())
            {
                if (!targetAlreadyHit && other.gameObject.GetComponent<PlayerController>().dodging == false)
                {
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(damageValue);
                    other.gameObject.GetComponent<PlayerController>().Knockback(knockbackDirection, knockbackStrength, 0.5f);
                }
            }
            affectedTargets.Add(other.gameObject);
        }
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.GetComponent<AI>() || other.GetComponent<PlayerController>())
    //    {
    //        bool targetAlreadyHit = false;
    //        foreach (GameObject target in affectedTargets)
    //        {
    //            if (target == other.gameObject)
    //            {
    //                targetAlreadyHit = true;
    //            }
    //        }

    //        if (other.GetComponent<AI>())
    //        {
    //            if (!targetAlreadyHit)
    //            {
    //                other.GetComponent<AI>().TakeDamage(damageValue);
    //                other.GetComponent<AI>().Knockback(knockbackDirection, knockbackStrength, 0.5f);
    //            }
    //        }
    //        else if (other.GetComponent<PlayerController>())
    //        {
    //            if (!targetAlreadyHit)
    //            {
    //                other.GetComponent<PlayerController>().TakeDamage(damageValue);
    //                other.GetComponent<PlayerController>().Knockback(knockbackDirection, knockbackStrength, 0.5f);
    //            }
    //        }
    //        affectedTargets.Add(other.gameObject);            
    //    }
    //}
}
