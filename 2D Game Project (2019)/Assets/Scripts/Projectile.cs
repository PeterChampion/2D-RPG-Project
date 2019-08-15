using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D RB;
    private int damageValue = 0;
    public int DamageValue { get { return damageValue; } set { damageValue = value; } }
    private Vector2 directionOfFlight;
    public Vector2 DirectionOfFlight { get { return directionOfFlight; } set { directionOfFlight = value; } }
    private float knockbackPower = 0;
    public float KnockbackPower { get { return knockbackPower; } set { knockbackPower = value; } }
    private float knockbackDuration = 0;
    public float KnockbackDuration { get { return knockbackDuration; } set { knockbackDuration = value; } }

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
