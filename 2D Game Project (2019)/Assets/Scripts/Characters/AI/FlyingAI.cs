using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
public class FlyingAI : AI
{
    private Vector2 yMovementDirection;
    [SerializeField] private LayerMask platformLayer;
    private bool lookingForPlatforms = true;
    private bool coroutineRunning = false;
    private Coroutine flightCoroutine = null;
    private bool descending = false;

    protected override void FixedUpdate()
    {
        if (!Stunned)
        {
            if (playerInRange && Mathf.Abs(xDistanceFromPlayer) < 6)
            {
                descending = true;
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
                descending = false;
                yMovementDirection = Vector2.zero;
                //RB.velocity = new Vector2(RB.velocity.x, 0);
            }

            if (directionOfMovement == 1) // Right
            {              
                xMovementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
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

        if (!coroutineRunning && !descending)
        {
            flightCoroutine = StartCoroutine(UpAndDownFlight(1f));
        }        

        if (!playerInRange || !descending)
        {
            RaycastHit2D ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.down, 4f, jumpableLayers);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 4f * Vector2.down, Color.green);

            if (ground.collider != null)
            {
                RB.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            }
        }

        if (lookingForPlatforms)
        {
            // If we are moving to the right...
            if (directionOfMovement == 1)
            {
                RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 2f, wallLayer);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2f * Vector2.right, Color.yellow);

                if (rightWall.collider != null)
                {
                    if (!playerInRange)
                    {
                        directionOfMovement = -1;
                        RB.velocity = new Vector2(RB.velocity.x / 2, RB.velocity.y);
                        Debug.Log("Turn Around, I hit a wall RIGHT");
                    }
                    else
                    {
                        // Boost upwards
                        RB.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                        Debug.Log("Upwards boost applied");
                    }
                }
            }
            else
            {
                RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 2f, wallLayer);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2f * Vector2.left, Color.yellow);

                if (leftWall.collider != null)
                {
                    if (!playerInRange)
                    {
                        directionOfMovement = 1;
                        RB.velocity = new Vector2(RB.velocity.x / 2, RB.velocity.y);
                    }
                    else
                    {
                        // Boost upwards
                        RB.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                    }
                }
            }
        }  

        if (yDistanceFromPlayer >= 0.2)
        {
            RaycastHit2D abovePlatform = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.up, 2f, platformLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2f * Vector2.up, Color.yellow);

            if (abovePlatform.collider != null)
            {
                StartCoroutine(MoveAroundObstruction(1f));
            }
        }
        else if (yDistanceFromPlayer <= -0.2)
        {
            RaycastHit2D belowPlatform = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.down, 2f, platformLayer);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2f * Vector2.down, Color.yellow);

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
            player.GetComponent<PlayerController>().Knockback(new Vector2(xMovementDirection.x, 0.5f), knockbackPower, knockbackDuration);
            StartCoroutine(MoveOnCollision(0.5f));
            Debug.Log("Collision!");
        }
    }

    IEnumerator MoveOnCollision(float duration)
    {
        Stunned = true;
        RB.AddForce(new Vector2(-directionOfMovement * 5, 5), ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        Stunned = false;
    }

    IEnumerator MoveAroundObstruction(float duration)
    {
        Stunned = true;
        lookingForPlatforms = false;
        RB.AddForce(new Vector2(xMovementDirection.x * 0.1f, 0), ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        Stunned = false;
        lookingForPlatforms = true;
    }

    IEnumerator UpAndDownFlight(float alternatingDelay)
    {
        coroutineRunning = true;
        RB.AddForce(new Vector2(0, 1f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(alternatingDelay);

        RB.velocity = new Vector2(RB.velocity.x, 0);
        RB.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(alternatingDelay);

        RB.velocity = new Vector2(RB.velocity.x, 0);
        coroutineRunning = false;
    }
}
