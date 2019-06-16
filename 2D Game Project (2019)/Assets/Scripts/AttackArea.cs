using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used when a character attacks, handles the collision, knockback, damage amplification, etc. Does not allow for the same character to be damaged multiple times within a single attack.
public class AttackArea : MonoBehaviour
{
    private Vector2 knockbackDirection;
    public int damageValue = 10;
    public float knockbackPower = 5;
    public List<GameObject> affectedTargets;

    private void Update()
    {
        if (transform.position.x > transform.parent.position.x) // Facing right
        {
            knockbackDirection = Vector2.right;
        }
        else // Facing left
        {
            knockbackDirection = Vector2.left;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Character2D>())
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
                    other.gameObject.GetComponent<AI>().Knockback(knockbackDirection, knockbackPower, 0.5f);
                }
            }
            else if (other.gameObject.GetComponent<PlayerController>())
            {
                if (!targetAlreadyHit && other.gameObject.GetComponent<PlayerController>().dodging == false)
                {
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(damageValue);
                    other.gameObject.GetComponent<PlayerController>().Knockback(knockbackDirection, knockbackPower, 0.5f);
                }
            }
            affectedTargets.Add(other.gameObject);
        }
    }
}
