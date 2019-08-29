using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
public class AStarFlyingAI : AI
{
    private Vector2 yMovementDirection;
    [SerializeField] private LayerMask platformLayer;
    private bool lookingForPlatforms = true;
    private bool coroutineRunning = false;
    private Coroutine flightCoroutine = null;
    private bool descending = false;

    public Transform target;
    Vector2[] path;
    int targetIndex;

    public void OnPathFound(Vector2[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            StopCoroutine("FollowPath");
            targetIndex = 0;
            path = newPath;
            StartCoroutine("FollowPath");
        }
    }

    private void RequestPath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            Vector2 currentWaypoint = path[0];

            while (true)
            {
                float xdistanceToWaypoint = Mathf.Abs(transform.position.x - currentWaypoint.x);
                float ydistanceToWaypoint = Mathf.Abs(transform.position.y - currentWaypoint.y);

                if (xdistanceToWaypoint <= 0.3f && ydistanceToWaypoint <= 0.3f)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        targetIndex = 0;
                        path = new Vector2[0];
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                Vector2 direction = currentWaypoint - (Vector2)transform.position;
                direction = direction.normalized;
                RB.AddForce(direction * speed);
                //RB.MovePosition(currentWaypoint);
                //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.green;
                //Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    private void Start()
    {
        InvokeRepeating("RequestPath", 0, 0.5f);
    }

    // ----

    protected override void FixedUpdate()
    {
        if (!knockedback)
        {
            if (playerInRange )
            {

            }
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
            //flightCoroutine = StartCoroutine(UpAndDownFlight(1f));
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
        knockedback = true;
        RB.AddForce(new Vector2(-directionOfMovement * 5, 5), ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        knockedback = false;
    }

    IEnumerator UpAndDownFlight(float alternatingDelay)
    {
        coroutineRunning = true;
        RB.AddForce(new Vector2(0, 1f), ForceMode2D.Impulse);
        Debug.Log("Up");

        yield return new WaitForSeconds(alternatingDelay);

        RB.velocity = new Vector2(RB.velocity.x, 0);
        RB.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);
        Debug.Log("Down");

        yield return new WaitForSeconds(alternatingDelay);

        RB.velocity = new Vector2(RB.velocity.x, 0);
        coroutineRunning = false;
    }
}

