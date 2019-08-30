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
    public Rigidbody2D RigidBody { get { return RB; } }
    protected Vector2 xMovementDirection;

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
    [SerializeField] protected float knockbackDuration = 0;
    private int originalLayer = 0;

    protected virtual void Awake() // Set References & Variable set up
    {
        RB = GetComponent<Rigidbody2D>();
        currentHealth = maximumHealth;
        originalLayer = gameObject.layer;
        Physics2D.IgnoreLayerCollision(10, 11, true); // Ignore collisions with pickups
        Physics2D.IgnoreLayerCollision(10, 13, true); // Ignore collisions with invulnerable characters
        Physics2D.IgnoreLayerCollision(13, 13, true); // Ignore collisions with invulnerable characters while also invulnerable
        Physics2D.IgnoreLayerCollision(13, 11, true); // Ignore collisions with pickups when invulnerable
    }
    
    protected virtual void ClampVelocity()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -3, 3), Mathf.Clamp(RB.velocity.y, -15, 15));
    }

    protected virtual void Attack()
    {
        Debug.DrawRay(transform.position, xMovementDirection * attackRange, Color.blue, 0.5f);
        RaycastHit2D[] newHits = Physics2D.RaycastAll(transform.position, xMovementDirection, attackRange, enemyLayer);
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
            newHit.collider.GetComponent<Character2D>().Knockback(new Vector2(xMovementDirection.x, 0.5f), knockbackPower, knockbackDuration);
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
    protected virtual bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * jumpRaycastLength, Color.red, 0.2f);

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
        gameObject.layer = 13;
        RB.velocity = new Vector2(0, RB.velocity.y);
        RB.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        yield return new WaitForSeconds(movementLockoutDuration);
        gameObject.layer = originalLayer;
        knockedback = false;
    }
}
