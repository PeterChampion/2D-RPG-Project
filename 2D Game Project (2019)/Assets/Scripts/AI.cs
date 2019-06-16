using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Handles detection of walls/ledges in the direction of the AI's path to determine whether the AI should turn around or not, in the case of the player being within detection range the AI will instead 
// jump over these walls/ledges if detected.
public class AI : Character2D
{
    private GameObject Player;
    [SerializeField] private int detectionRange;
    [SerializeField] private int minimumRange;
    [SerializeField] private int attackRange;
    private float xdistanceFromPlayer;
    private float ydistanceFromPlayer;
    [SerializeField] Vector2 rayCastOffset;
    [SerializeField] private float timeBetweenJumps = 1.5f;
    private float jumpDelay = 0;    
    private bool playerInRange = false;
    private int movementDirection = -1;

    protected override void Awake()
    {
        base.Awake();
        Player = FindObjectOfType<PlayerController>().gameObject;
        Physics2D.IgnoreLayerCollision(10, 11, true);
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
            if (!knockedback)
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

    protected override void Attack()
    {
        if (xdistanceFromPlayer < attackRange && ydistanceFromPlayer < attackRange)
        {
            if (Time.time > attackDelay)
            {
                attackDelay = (Time.time + attackCooldown);
                base.Attack();
            }
        }
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

        if (movementDirection == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 1.5f, groundLayer);
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

            RaycastHit2D rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, 1f, groundLayer);
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
        }
        else
        {
            RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 1.5f, groundLayer);
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

            RaycastHit2D leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, 1f, groundLayer);
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
    }
     protected override void Jump()
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
}
