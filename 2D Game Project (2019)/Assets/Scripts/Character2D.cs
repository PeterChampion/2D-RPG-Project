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

    // Combat
    protected GameObject attackArea;
    protected float attackDelay;
    [SerializeField] protected int damageValue = 5;
    [SerializeField] protected float knockbackPower = 5;
    [SerializeField] protected float attackCooldown = 1;
    

    protected virtual void Awake() // Set References & Variable set up
    {
        RB = GetComponent<Rigidbody2D>();
        attackArea = GetComponentInChildren<AttackArea>().gameObject;
        attackArea.GetComponent<AttackArea>().damageValue = damageValue;
        attackArea.GetComponent<AttackArea>().knockbackPower = knockbackPower;
        attackArea.gameObject.SetActive(false);
        currentHealth = maximumHealth;
    }

    protected virtual void Attack()
    {
        StartCoroutine(Attack(0.1f));
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
    protected IEnumerator Attack(float duration)
    {
        attackArea.GetComponent<AttackArea>().affectedTargets.Clear();
        attackArea.SetActive(true);
        yield return new WaitForSeconds(duration);
        attackArea.SetActive(false);
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
