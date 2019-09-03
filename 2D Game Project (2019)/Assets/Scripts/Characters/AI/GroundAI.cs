using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAI : AI
{
    private enum NPCType { Melee, Ranged }
    [SerializeField] private NPCType attackType;
    [SerializeField] private GameObject projectile;

    protected override void Update()
    {
        base.Update();
        Attack();
    }

    protected override void Attack()
    {
        if (Mathf.Abs(xDistanceFromPlayer) < attackRange && Mathf.Abs(yDistanceFromPlayer) < attackRange)
        {
            if (Time.time > attackDelay)
            {
                attackDelay = (Time.time + attackCooldown);

                if (attackType == NPCType.Melee)
                {
                    base.Attack();
                }
                else
                {
                    GameObject projectileFired = Instantiate(projectile, transform.position + new Vector3(directionOfMovement, 0, 0), Quaternion.identity);
                    projectileFired.GetComponent<Projectile>().AssignValues(damage, xMovementDirection, knockbackPower, knockbackDuration);
                }
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

        if (playerInRange && Mathf.Abs(yDistanceFromPlayer) > 0.5f && Mathf.Abs(xDistanceFromPlayer) < attackRange)
        {
            StartCoroutine(JumpWithDelay(0.2f));
        }

        // If we are moving to the right...
        if (directionOfMovement == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 2.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2.5f * Vector2.right, Color.yellow);

            if (rightWall.collider != null)
            {
                if (!playerInRange && !setToPatrol && IsGrounded())
                {
                    directionOfMovement = -1;
                }
                else if (Mathf.Abs(xDistanceFromPlayer) > minimumRange)
                {
                    Jump();
                }
            }

            RaycastHit2D rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, 1f, jumpableLayers);
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
                }
            }
        }
        else
        {
            RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 2.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2.5f * Vector2.left, Color.yellow);

            if (leftWall.collider != null)
            {
                if (!playerInRange && !setToPatrol && IsGrounded())
                {
                    directionOfMovement = 1;
                }
                else if (Mathf.Abs(xDistanceFromPlayer) > minimumRange)
                {
                    Jump();
                }
            }

            RaycastHit2D leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, 1f, jumpableLayers);
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
                }
            }
        }
    }

    IEnumerator JumpWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Jump();
    }
}
