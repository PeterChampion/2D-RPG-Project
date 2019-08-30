using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D RB;
    private int damageValue = 0;
    private Vector2 directionOfFlight;
    private float knockbackPower = 0;
    private float knockbackDuration = 0;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(15, 10, true); // Ignore collisions with enemies
        Physics2D.IgnoreLayerCollision(15, 11, true); // Ignore collisions with pickups
        Physics2D.IgnoreLayerCollision(15, 13, true); // Ignore collisions with invulnerable characters
    }

    private void Start()
    {
        RB.AddForce(directionOfFlight * 10, ForceMode2D.Impulse);

        Destroy(gameObject, 5);
    }

    public void AssignValues(int _damage, Vector2 _direction, float _knockbackPower, float _knockbackDuration)
    {
        damageValue = _damage;
        directionOfFlight = _direction;
        knockbackPower = _knockbackPower;
        knockbackDuration = _knockbackDuration;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            player.GetComponent<PlayerController>().TakeDamage(damageValue);
            player.GetComponent<PlayerController>().Knockback(new Vector2(directionOfFlight.x, 0.5f), knockbackPower, knockbackDuration);

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
