using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAI : AI
{
    protected override void Update()
    {
        base.Update();
        Attack();
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

    protected override void ClampVelocity()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -1, 1), Mathf.Clamp(RB.velocity.y, -15, 15));
    }

    protected override void AIMovement()
    {
        if (IsGrounded())
        {
            base.AIMovement();
        }       

        // If we are moving to the right...
        if (directionOfMovement == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 1.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.right, Color.yellow);

            if (rightWall.collider != null)
            {
                if (!playerInRange && IsGrounded())
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
}
