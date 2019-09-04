using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all 2D characters, gives all characters the capability of moving, attacking, ground checking, dying, etc.
public abstract class Character2D : MonoBehaviour
{
    // Health
    [SerializeField] protected float maximumHealth = 100;
    public float MaximumHealth { get { return maximumHealth; } set { maximumHealth = value; } }
    protected float currentHealth;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    // Movement
    [SerializeField] protected int speed = 5;
    [SerializeField] protected int jumpStrength = 8;
    [SerializeField] protected float jumpRaycastLength = 1;
    [SerializeField] protected LayerMask jumpableLayers;
    protected bool grounded;
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
    //[SerializeField] private LayerMask enemyLayer = new LayerMask();
    //public LayerMask EnemyLayer { get { return enemyLayer; } }
    [SerializeField] protected float knockbackDuration = 0;
    private int originalLayer = 0;
    private bool stunned;
    public bool Stunned { get { return stunned; } set { stunned = value; } }
    [SerializeField] GameObject attackArea;
    private Coroutine stunCoroutine;

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

    protected virtual void StandardAttack()
    {
        AttackArea attack = Instantiate(attackArea, (Vector2)transform.position + xMovementDirection, Quaternion.identity, transform).GetComponent<AttackArea>();
        attack.Damage = damage;
        attack.KnockbackDirection = xMovementDirection;
        attack.KnockbackPower = knockbackPower;
        attack.KnockbackDuration = knockbackDuration;

        //Debug.DrawRay(transform.position, xMovementDirection * attackRange, Color.blue, 0.5f);
        //RaycastHit2D[] newHits = Physics2D.RaycastAll(transform.position, xMovementDirection, attackRange, enemyLayer);
        //List<RaycastHit2D> beenHits = new List<RaycastHit2D>();

        //foreach (RaycastHit2D newHit in newHits)
        //{
        //    foreach (RaycastHit2D beenHit in beenHits)
        //    {
        //        if (newHit == beenHit)
        //        {
        //            break;
        //        }
        //    }
        //    beenHits.Add(newHit);
        //    newHit.collider.GetComponent<Character2D>().TakeDamage(damage);
        //    newHit.collider.GetComponent<Character2D>().Knockback(new Vector2(xMovementDirection.x, 0.5f), knockbackPower, knockbackDuration);
        //    Debug.Log(damage + " damage dealt to " + newHit.collider.gameObject.name + "!");
        //}
    }

    protected virtual void HeavyAttack()
    {
        AttackArea attack = Instantiate(attackArea, (Vector2)transform.position + xMovementDirection, Quaternion.identity, transform).GetComponent<AttackArea>();
        attack.Damage = damage * 2;
        attack.KnockbackDirection = xMovementDirection;
        attack.KnockbackPower = knockbackPower * 2;
        attack.KnockbackDuration = knockbackDuration * 1.5f;
        attack.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, jumpableLayers);
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
        Stunned = true;        
        gameObject.layer = 13;
        RB.velocity = new Vector2(0, RB.velocity.y);
        RB.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        yield return new WaitForSeconds(movementLockoutDuration);

        gameObject.layer = originalLayer;
        Stunned = false;
    }

    public void ApplyStunEffect(float stunDuration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        stunCoroutine = StartCoroutine(StunEffect(stunDuration));
    }

    private IEnumerator StunEffect(float stunDuration)
    {
        Stunned = true;
        yield return new WaitForSeconds(stunDuration);
        Stunned = false;
    }
}
