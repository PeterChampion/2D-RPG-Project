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
    [SerializeField] protected int minimumRange = 2;
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

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().gameObject;
        Physics2D.IgnoreLayerCollision(10, 10, true); // Ignore collisions with other enemies
    }

    protected virtual void Update()
    {
        AIMovement();
        CheckHealth();
        if (!knockedback)
        {
            ClampVelocity();
        }        
    }

    protected virtual void FixedUpdate()
    {
        // If we ARE NOT in minimum range AND ARE on the ground = chase
        // If we ARE in minimum range AND ARE on the ground = stop
        // If we ARE NOT in minimum range and ARE NOT on the ground = chase
        // If we ARE in minimum range and ARE NOT on the ground = chase

        if (!knockedback)
        {
            if (directionOfMovement == 1) // Right
            {
                //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);                
                xMovementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
            }
            else if (directionOfMovement == -1) // Left
            {
                //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);                
                xMovementDirection = Vector2.left;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
            }

            if (Mathf.Abs(xDistanceFromPlayer) < minimumRange && Mathf.Abs(yDistanceFromPlayer) < minimumRange && IsGrounded())
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }
            else
            {
                //RB.velocity = new Vector2(xMovementDirection.x * speed, RB.velocity.y);
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
            Debug.Log("PLAYER IN RANGE");
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
                //RB.velocity = new Vector2(0, RB.velocity.y);
            }
            else
            {
                directionOfMovement = 1; // Player is on the right so set directionOfMovement to match
                //RB.velocity = new Vector2(0, RB.velocity.y);
            }
        }
    }

    protected override void Jump()
    {
        if (Time.time > jumpDelay)
        {
            if (IsGrounded())
            {
                jumpDelay = Time.time + jumpCooldown;
                Debug.Log("Jump");
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

            Debug.Log(patrolpointIndex);

            if (Mathf.Abs(xDistanceFromPoint) < 1)
            {
                Debug.Log("Patrol point reached");
                if (patrolpointIndex + 1 > patrolPoints.Count - 1)
                {
                    patrolpointIndex = 0;
                    Debug.Log("Reset patrol index");
                }
                else
                {
                    patrolpointIndex++;
                    Debug.Log("Increment patrol index");
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
}
