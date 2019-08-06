using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Handles detection of walls/ledges in the direction of the AI's path to determine whether the AI should turn around or not, in the case of the player being within detection range the AI will instead 
// jump over these walls/ledges if detected.
public class AI : Character2D
{
    private GameObject Player;
    [SerializeField] private int detectionRange = 5;
    [SerializeField] private int minimumRange = 2;
    private float xdistanceFromPlayer;
    private float ydistanceFromPlayer;
    [SerializeField] Vector2 rayCastOffset = new Vector2(1,0);
    [SerializeField] private float jumpCooldown = 1.5f;
    private float jumpDelay = 0;
    private bool playerInRange = false;
    private int directionOfMovement = -1;
    [SerializeField] private LayerMask wallLayer = new LayerMask();
    
    protected override void Awake()
    {
        base.Awake();
        Player = FindObjectOfType<PlayerController>().gameObject;
        Physics2D.IgnoreLayerCollision(10, 11, true);
    }

    void Update() // Update is every frame, e.g 101 FPS = 101 times ran a second
    {
        AIMovement();
        CheckHealth();
        Attack();
    }

    private void FixedUpdate() // Fixed Update is every Physics Update, e.g Physics updates happene every 16th frame
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
                movementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                RB.velocity = new Vector2(movementDirection.x * speed, RB.velocity.y);
            }
            else if (directionOfMovement == -1) // Left
            {
                //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);                
                movementDirection = Vector2.left;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                RB.velocity = new Vector2(movementDirection.x * speed, RB.velocity.y);
            }

            if (Mathf.Abs(xdistanceFromPlayer) < minimumRange)
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }
        }       
    }

    protected override void Attack()
    {
        if (xdistanceFromPlayer < attackRange && ydistanceFromPlayer < attackRange)
        {
            attackDelay = (Time.time + attackCooldown);
            if (Time.time > attackDelay)
            {
                base.Attack();
            }
        }
    }

    protected virtual void AIMovement()
    {
        xdistanceFromPlayer = Player.transform.position.x - transform.position.x; // Calculate distance on X axis
        ydistanceFromPlayer = Player.transform.position.y - transform.position.y; // Calculate distance on Y axis

        // If the player is within the detection range...
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
                directionOfMovement = -1; // Player is on the left so set directionOfMovement to match
            }
            else
            {
                directionOfMovement = 1; // Player is on the right so set directionOfMovement to match
            }
        }

        // If we are moving to the right...
        if (directionOfMovement == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 1.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.right, Color.yellow);

            if (rightWall.collider != null)
            {
                if (!playerInRange && IsGrounded() )
                {
                    directionOfMovement = -1;
                    Debug.Log("Turn Around, I hit a wall RIGHT");
                }
                else if (xdistanceFromPlayer > minimumRange)
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
                else if (IsGrounded())
                {
                    directionOfMovement = -1;
                    Debug.Log("Turn Around, no ground ahead RIGHT");
                }
            }
        }
        else
        {
            RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 1.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.left, Color.yellow);

            if (leftWall.collider != null)
            {
                if (!playerInRange && IsGrounded())
                {
                    directionOfMovement = 1;
                    Debug.Log("Turn Around, I hit a wall LEFT");
                }
                else if (xdistanceFromPlayer * -1 > minimumRange)
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
                else if (IsGrounded())
                {
                    directionOfMovement = 1;
                    Debug.Log("Turn Around, no ground ahead LEFT");
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
                jumpDelay = Time.time + jumpCooldown;
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
    }
}
