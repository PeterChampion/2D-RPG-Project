using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private int damage = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.GetComponent<Character2D>() && collidedObject.layer != 13)
        {
            Character2D character = collidedObject.GetComponent<Character2D>();
            character.TakeDamage(damage);
            character.Knockback(new Vector2(-character.XMovementDirection.x, 1), 10, 1f);
        }
    }
}
