using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Handles detection of walls/ledges in the direction of the AI's path to determine whether the AI should turn around or not, in the case of the player being within detection range the AI will instead 
// jump over these walls/ledges if detected.
public abstract class AI : Character2D
{
    protected GameObject player;
    [SerializeField] protected int detectionRange = 5;
    [SerializeField] protected float minimumRange = 2;
    protected float xDistanceFromPlayer;
    protected float yDistanceFromPlayer;
    [SerializeField] protected Vector2 rayCastOffset = new Vector2(1,0);
    [SerializeField] protected float jumpCooldown = 1.5f;
    protected float jumpDelay = 0;
    protected bool playerInRange = false;
    protected int directionOfMovement = -1;
    [SerializeField] protected LayerMask wallLayer = new LayerMask();
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    private int patrolpointIndex = 0;
    [SerializeField] protected bool setToPatrol = false;
    public bool isDead;
    private Transform healthBar;
    private Coroutine healthBarCoroutine;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().gameObject;
        healthBar = transform.Find("Healthbar");
        healthBar.localScale = new Vector3(currentHealth / 100, healthBar.localScale.y, healthBar.localScale.z);
        healthBar.GetComponent<SpriteRenderer>().enabled = false;
        Physics2D.IgnoreLayerCollision(10, 10, true); // Ignore collisions with other enemies
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            AIMovement();
        }

        CheckHealth();

        if (!Stunned)
        {
            ClampVelocity();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!Stunned)
        {
            if (directionOfMovement == 1) // Right
            {              
                xMovementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, 180, 0, 0);
            }
            else if (directionOfMovement == -1) // Left
            {               
                xMovementDirection = Vector2.left;
                transform.rotation = new Quaternion(transform.rotation.x, 0, 0, 0);
            }

            if (Mathf.Abs(xDistanceFromPlayer) < minimumRange && Mathf.Abs(yDistanceFromPlayer) < minimumRange && IsGrounded())
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }
            else
            {
                RB.AddForce(new Vector2(xMovementDirection.x * speed, 0), ForceMode2D.Impulse);
            }
        }       
    }

    protected virtual void AIMovement()
    {
        xDistanceFromPlayer = player.transform.position.x - transform.position.x; // Calculate distance on X axis
        yDistanceFromPlayer = player.transform.position.y - transform.position.y; // Calculate distance on Y axis

        // If the player is within the detection range...
        if (Mathf.Abs(xDistanceFromPlayer) < detectionRange && Mathf.Abs(yDistanceFromPlayer) < detectionRange)
        {
            playerInRange = true;
        }
        else // Otherwise if the player is not...
        {
            playerInRange = false;
            if (setToPatrol)
            {
                Patrol();
            }
        }

        if (playerInRange)
        {
            if (xDistanceFromPlayer < 0)
            {
                directionOfMovement = -1; // Player is on the left so set directionOfMovement to match
            }
            else
            {
                directionOfMovement = 1; // Player is on the right so set directionOfMovement to match
            }
        }
    }

    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);

        if (currentHealth <= 0)
        {
            healthBar.localScale = new Vector3(0, healthBar.localScale.y, healthBar.localScale.z);
        }
        else
        {
            healthBar.localScale = new Vector3(currentHealth / 100, healthBar.localScale.y, healthBar.localScale.z);
        }

        if (healthBarCoroutine != null)
        {
            print("Stopping");
            StopCoroutine(healthBarCoroutine);
        }
        healthBarCoroutine = StartCoroutine(DisplayHealthBar(3));
    }

    protected override void Die()
    {
        isDead = true;
        base.Die();
    }

    protected override void Jump()
    {
        if (Time.time > jumpDelay)
        {
            if (IsGrounded())
            {
                jumpDelay = Time.time + jumpCooldown;
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
    }

    private void Patrol()
    {
        // If player is not in range, and we are set to patrol, then we patrol between points.
        // Look at current index of list
        // Compare x positions to determine direction to travel to
        // Once difference between x positions is negligible, increment index
        // If index would exceed List, wrap back to 0.
        // Repeat

        if (patrolPoints.Count > 0)
        {
            float xDistanceFromPoint = patrolPoints[patrolpointIndex].position.x - transform.position.x;

            if (Mathf.Abs(xDistanceFromPoint) < 1)
            {
                if (patrolpointIndex + 1 > patrolPoints.Count - 1)
                {
                    patrolpointIndex = 0;
                }
                else
                {
                    patrolpointIndex++;
                }
            }

            if (xDistanceFromPoint < 0)
            {
                directionOfMovement = -1; // Point is on the left so set directionOfMovement to match
            }
            else
            {
                directionOfMovement = 1; // Point is on the right so set directionOfMovement to match
            }
        }        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ApplyDirectionalForce(Vector2 direction, float force)
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(direction * (force / 2), ForceMode2D.Impulse);
    }

    private IEnumerator DisplayHealthBar(float duration)
    {
        SpriteRenderer healthbarSpriterender = healthBar.GetComponent<SpriteRenderer>();
        healthbarSpriterender.enabled = true;

        yield return new WaitForSeconds(duration);

        healthbarSpriterender.enabled = false;
        healthBarCoroutine = null;
    }
}
