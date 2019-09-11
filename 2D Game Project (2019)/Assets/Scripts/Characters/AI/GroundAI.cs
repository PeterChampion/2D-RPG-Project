using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAI : AI
{
    private enum NPCType { Melee, Ranged }
    [SerializeField] private float initialAttackWindUp = 0.5f;
    [SerializeField] private NPCType attackType;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float raycastXSize = 1;
    [SerializeField] private float raycastYSize = 1;
    [SerializeField] private float heightDifferenceThreshhold = 0.5f;

    protected override void Update()
    {
        base.Update();
        Attack();

        if (RB.velocity.y > 3f)
        {
            characterAnim.SetBool("IsJumping", true);
        }
        else
        {
            characterAnim.SetBool("IsJumping", false);
        }
    }

    private void Attack()
    {
        if (Mathf.Abs(xDistanceFromPlayer) < attackRange && Mathf.Abs(yDistanceFromPlayer) < attackRange && !Stunned && !isDead)
        {
            if (Time.time > attackDelay)
            {
                attackDelay = (Time.time + attackCooldown);

                if (attackType == NPCType.Melee)
                {
                    CancelInvoke("StandardAttack");
                    Invoke("StandardAttack", Random.Range(0, initialAttackWindUp));
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

        if (playerInRange && Mathf.Abs(yDistanceFromPlayer) > heightDifferenceThreshhold && Mathf.Abs(xDistanceFromPlayer) < attackRange)
        {
            StartCoroutine(JumpWithDelay(0.2f));
        }

        // If we are moving to the right...
        if (directionOfMovement == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, raycastXSize, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right * raycastXSize, Color.yellow);

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

            RaycastHit2D rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, raycastYSize, jumpableLayers);
            Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down * raycastYSize, Color.yellow);

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
            RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, raycastXSize, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left * raycastXSize, Color.yellow);

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

            RaycastHit2D leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, raycastYSize, jumpableLayers);
            Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down * raycastYSize, Color.yellow);

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

        characterAnim.SetFloat("Speed", Mathf.Abs(RB.velocity.x));
    }

    protected override void Jump()
    {
        if (Time.time > jumpDelay)
        {
            if (IsGrounded())
            {
                jumpDelay = Time.time + jumpCooldown;
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
                characterAnim.SetBool("IsJumping", true);
            }
        }
    }

    IEnumerator JumpWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Jump();
    }

    private void ToggleAttackAnimation()
    {
        if (!characterAnim.GetBool("IsAttacking"))
        {
            characterAnim.SetBool("IsAttacking", true);
        }
        else
        {
            characterAnim.SetBool("IsAttacking", false);
        }
    }
}
