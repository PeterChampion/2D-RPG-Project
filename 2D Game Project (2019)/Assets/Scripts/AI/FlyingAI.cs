using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : AI
{
    private Vector2 yMovementDirection;
    [SerializeField] private LayerMask platformLayer;

    protected override void FixedUpdate()
    {
        if (!knockedback)
        {
            if (playerInRange && xdistanceFromPlayer < 6)
            {
                if (player.transform.position.y > transform.position.y && yMovementDirection != Vector2.up)
                {
                    yMovementDirection = Vector2.up;
                    RB.velocity = new Vector2(RB.velocity.x, 0);                    
                }
                else if (player.transform.position.y < transform.position.y && yMovementDirection != Vector2.down)
                {
                    yMovementDirection = Vector2.down;
                    RB.velocity = new Vector2(RB.velocity.x, 0);                    
                }
            }
            else
            {
                yMovementDirection = Vector2.zero;
                RB.velocity = new Vector2(RB.velocity.x, 0);
            }

            if (directionOfMovement == 1) // Right
            {              
                xMovementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                //RB.velocity = new Vector2(xMovementDirection.x * speed, yMovementDirection.y * speed);
            }
            else if (directionOfMovement == -1) // Left
            {             
                xMovementDirection = Vector2.left;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);                
            }

            RB.AddForce(new Vector2(xMovementDirection.x * speed, yMovementDirection.y * speed));
        }
    }

    protected override void ClampVelocity()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -5, 5), Mathf.Clamp(RB.velocity.y, -5, 5));
    }

    protected override void AIMovement()
    {
        base.AIMovement();

        // If we are moving to the right...
        if (directionOfMovement == 1)
        {
            RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 1.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.right, Color.yellow);

            if (rightWall.collider != null)
            {
                if (!playerInRange)
                {
                    directionOfMovement = -1;
                    Debug.Log("Turn Around, I hit a wall RIGHT");
                }
                else 
                {
                    // Boost upwards
                    RB.AddForce(new Vector2(0, 2), ForceMode2D.Impulse);
                    Debug.Log("Upwards boost applied");
                }
            }
        }
        else
        {
            RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 1.5f, wallLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.left, Color.yellow);

            if (leftWall.collider != null)
            {
                if (!playerInRange)
                {
                    directionOfMovement = 1;
                    Debug.Log("Turn Around, I hit a wall LEFT");
                }
                else
                {
                    // Boost upwards
                    RB.AddForce(new Vector2(0, 1), ForceMode2D.Impulse);
                    Debug.Log("Upwards boost applied");
                }
            }            
        }

        if (ydistanceFromPlayer >= 0.2)
        {
            RaycastHit2D abovePlatform = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.up, 1.5f, platformLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.up, Color.yellow);

            if (abovePlatform.collider != null)
            {
                StartCoroutine(MoveAroundObstruction(1));
            }
        }
        else if (ydistanceFromPlayer <= -0.2)
        {
            RaycastHit2D belowPlatform = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.down, 1.5f, platformLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 1.5f * Vector2.down, Color.yellow);

            if (belowPlatform.collider != null)
            {
                StartCoroutine(MoveAroundObstruction(1f));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player)
        {
            player.GetComponent<PlayerController>().TakeDamage(damage);
            player.GetComponent<PlayerController>().Knockback(xMovementDirection, knockbackPower, knockbackDuration);
            StartCoroutine(MoveOnCollision(0.5f));
            Debug.Log("Collision!");
        }
    }

    IEnumerator MoveOnCollision(float duration)
    {
        knockedback = true;
        RB.AddForce(new Vector2(-directionOfMovement * 10, 10), ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        knockedback = false;
    }

    IEnumerator MoveAroundObstruction(float duration)
    {
        knockedback = true;
        RB.AddForce(new Vector2(xMovementDirection.x * 2, 0), ForceMode2D.Impulse);
        Debug.Log("Move cunt");
        yield return new WaitForSeconds(duration);
        knockedback = false;
    }
}
