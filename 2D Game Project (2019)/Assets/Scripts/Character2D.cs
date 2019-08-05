using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all 2D characters, gives all characters the capability of moving, attacking, ground checking, dying, etc.
public abstract class Character2D : MonoBehaviour
{
    // Health
    [SerializeField] protected int maximumHealth = 100;
    protected int currentHealth;

    // Movement
    [SerializeField] protected int speed = 5;
    [SerializeField] protected int jumpStrength = 8;
    [SerializeField] protected float jumpRaycastLength = 1;
    [SerializeField] protected LayerMask groundLayer;
    protected bool grounded;
    protected bool knockedback;
    protected Rigidbody2D RB;
    protected Vector2 movementDirection;

    // Combat
    protected float attackDelay;
    [SerializeField] protected float attackRange = 2;
    [SerializeField] protected int damage = 5;
    public int Damage { get { return damage; } set { damage = value; } }
    [SerializeField] protected float knockbackPower = 5;
    [SerializeField] protected float attackCooldown = 1;
    [SerializeField] private int armour = 0;
    public int Armour { get { return armour; } set { armour = value; } }
    [SerializeField] private int magicResist = 0;
    public int MagicResist { get { return magicResist; } set { magicResist = value; } }
    [SerializeField] private LayerMask enemyLayer = new LayerMask();
    public LayerMask EnemyLayer { get { return enemyLayer; } }


    protected virtual void Awake() // Set References & Variable set up
    {
        RB = GetComponent<Rigidbody2D>();
        currentHealth = maximumHealth;
    }

    protected virtual void Attack()
    {
        Debug.DrawRay(transform.position, movementDirection * attackRange, Color.blue, 0.5f);
        RaycastHit2D[] newHits = Physics2D.RaycastAll(transform.position, movementDirection, attackRange, enemyLayer);
        List<RaycastHit2D> beenHits = new List<RaycastHit2D>();

        foreach (RaycastHit2D newHit in newHits)
        {
            foreach (RaycastHit2D beenHit in beenHits)
            {
                if (newHit == beenHit)
                {
                    break;
                }
            }
            beenHits.Add(newHit);
            newHit.collider.GetComponent<Character2D>().TakeDamage(damage);
            newHit.collider.GetComponent<Character2D>().Knockback(movementDirection, knockbackPower, 0.5f);
            Debug.Log(damage + " damage dealt to " + newHit.collider.gameObject.name + "!");
        }
    }

    public virtual void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
    }

    protected virtual void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void Jump()
    {
        if (IsGrounded())
        {
            RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
        }
    }
    protected bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * jumpRaycastLength, Color.red, 5f);

        if (hit.collider != null)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    public void Knockback(Vector2 knockbackDirection, float knockbackStrength, float movementLockoutDuration)
    {
        StartCoroutine(KnockBackEffect(knockbackDirection, knockbackStrength, movementLockoutDuration));
    }

    public IEnumerator KnockBackEffect(Vector2 knockbackDirection, float knockbackStrength, float movementLockoutDuration)
    {
        knockedback = true;
        RB.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        Debug.Log("Knockback force applied!");
        yield return new WaitForSeconds(movementLockoutDuration);
        knockedback = false;
    }
}
