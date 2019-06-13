using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100;
    private int currentHealth;
    private GameObject Player;
    [SerializeField] private int detectionRange;
    [SerializeField] private int minimumRange;
    [SerializeField] private int attackRange;
    private float xdistanceFromPlayer;
    private float ydistanceFromPlayer;
    [SerializeField] Vector2 rayCastOffset;
    [SerializeField] LayerMask layermask;
    [SerializeField] private int speed = 5;
    [SerializeField] private int jumpStrength = 5;
    [SerializeField] private int jumpRaycastLength = 1;
    [SerializeField] private float timeBetweenJumps = 1.5f;
    private float jumpDelay = 0;
    private Rigidbody2D RB;
    private GameObject attackArea;
    private bool knockedBack = false;
    private float attackDelay;
    private float attackCooldown = 2;
    private bool playerInRange = false;

    private int movementDirection = -1;
    private bool grounded;

    void Start()
    {
        Player = FindObjectOfType<PlayerController>().gameObject;
        RB = GetComponent<Rigidbody2D>();
        attackArea = GetComponentInChildren<AttackArea>().gameObject;
        attackArea.gameObject.SetActive(false);
        currentHealth = maximumHealth;
    }

    void Update()
    {
        ComputeVelocity();
        CheckHealth();
        Attack();
    }

    private void FixedUpdate()
    {
        float xDirection = movementDirection * speed * Time.deltaTime; // Test

        // If we ARE NOT in minimum range AND ARE on the ground = chase
        // If we ARE in minimum range AND ARE on the ground = stop
        // If we ARE NOT in minimum range and ARE NOT on the ground = chase
        // If we ARE in minimum range and ARE NOT on the ground = chase

        // If we ARE in minimum range and ARE NOT on the ground
        if (Mathf.Abs(xdistanceFromPlayer) < minimumRange && playerInRange) // && IsGrounded() ????
        {
            if (!IsGrounded())
            {
                transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);
            }
        }
        else
        {
            if (!knockedBack)
            {
                if (movementDirection == 1) // Right
                {
                    transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                }
                else if (movementDirection == -1) // Left
                {
                    transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                }
            }            
        }   
    }

    private void Attack()
    {
        if (xdistanceFromPlayer < attackRange && ydistanceFromPlayer < attackRange)
        {
            if (Time.time > attackDelay)
            {
                attackDelay = (Time.time + attackCooldown);
                StartCoroutine(Attack(0.1f));
            }
        }
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            // Character died
            Die();
        }

        if (currentHealth > maximumHealth)
        {
            currentHealth = maximumHealth;
        }
    }

    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void ComputeVelocity()
    {
        xdistanceFromPlayer = Player.transform.position.x - transform.position.x;
        ydistanceFromPlayer = Player.transform.position.y - transform.position.y;        

        if (Mathf.Abs(xdistanceFromPlayer) < detectionRange && Mathf.Abs(ydistanceFromPlayer) < detectionRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (playerInRange)
        {
            if (xdistanceFromPlayer < 0)
            {
                movementDirection = -1;
            }
            else
            {
                movementDirection = 1;
            }
        }

        RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 1.5f, layermask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.right, Color.yellow);

        if (rightWall.collider != null)
        {
            if (!playerInRange)
            {
                movementDirection = -1;
                Debug.Log("Turn Around");
            }
            else
            {
                Jump();
            }
        }

        RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 1.5f, layermask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.left, Color.yellow);

        if (leftWall.collider != null)
        {
            if (!playerInRange)
            {
                movementDirection = 1;
                Debug.Log("Turn Around");
            }
            else
            {
                Jump();
            }
        }

        RaycastHit2D rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, 1f, layermask);
        Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), 1f * Vector2.down, Color.yellow);

        if (rightLedge.collider == null)
        {
            if (playerInRange)
            {
                Jump();
            }
            else
            {
                movementDirection = -1;
                Debug.Log("Turn Around");
            }
        }

        RaycastHit2D leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, 1f, layermask);
        Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), 1f * Vector2.down, Color.yellow);

        if (leftLedge.collider == null)
        {
            if (playerInRange)
            {
                Jump();
            }
            else
            {
                movementDirection = 1;
                Debug.Log("Turn Around");
            }
        }
    }
    private void Jump()
    {
        if (Time.time > jumpDelay)
        {
            if (IsGrounded())
            {
                jumpDelay = Time.time + timeBetweenJumps;
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
    }
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, layermask);
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

    private IEnumerator Attack(float duration)
    {
        attackArea.GetComponent<AttackArea>().affectedTargets.Clear();
        attackArea.SetActive(true);
        yield return new WaitForSeconds(duration);
        attackArea.SetActive(false);
    }

    public IEnumerator KnockBackEffect(Vector2 knockbackDirection, float knockbackStrength, float movementLockoutDuration)
    {
        knockedBack = true;
        RB.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        Debug.Log("Knockback force applied!");
        yield return new WaitForSeconds(movementLockoutDuration);
        knockedBack = false;
    }
}
