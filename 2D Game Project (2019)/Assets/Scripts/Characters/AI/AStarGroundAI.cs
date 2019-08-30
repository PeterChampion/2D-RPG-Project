using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGroundAI : AI
{
    private enum NPCType { Melee, Ranged }
    [SerializeField] private NPCType attackType;
    [SerializeField] private GameObject projectile;

    public Transform target;
    Vector2[] path;
    int targetIndex;

    private void Start()
    {
        InvokeRepeating("RequestPath", 0, 0.5f);
    }

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
                //RB.AddForce(direction * speed);
                RB.MovePosition(currentWaypoint);
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
        //if (IsGrounded())
        //{
        //    base.AIMovement();
        //}

        //if (playerInRange && Mathf.Abs(yDistanceFromPlayer) > 0.5f && Mathf.Abs(xDistanceFromPlayer) < attackRange)
        //{
        //    StartCoroutine(JumpWithDelay(0.2f));
        //}

        //// If we are moving to the right...
        //if (directionOfMovement == 1)
        //{
        //    RaycastHit2D rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, 2.5f, wallLayer);
        //    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2.5f * Vector2.right, Color.yellow);

        //    if (rightWall.collider != null)
        //    {
        //        if (!playerInRange && !setToPatrol && IsGrounded())
        //        {
        //            directionOfMovement = -1;
        //            Debug.Log("Turn Around, I hit a wall RIGHT");
        //        }
        //        else if (Mathf.Abs(xDistanceFromPlayer) > minimumRange)
        //        {
        //            Jump();
        //        }
        //    }

        //    RaycastHit2D rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), Vector2.down, 1f, groundLayer);
        //    Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y), 1f * Vector2.down, Color.yellow);

        //    if (rightLedge.collider == null)
        //    {
        //        if (playerInRange)
        //        {
        //            Jump();
        //        }
        //        else if (IsGrounded())
        //        {
        //            directionOfMovement = -1;
        //            Debug.Log("Turn Around, no ground ahead RIGHT");
        //        }
        //    }
        //}
        //else
        //{
        //    RaycastHit2D leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, 2.5f, wallLayer);
        //    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), 2.5f * Vector2.left, Color.yellow);

        //    if (leftWall.collider != null)
        //    {
        //        if (!playerInRange && !setToPatrol && IsGrounded())
        //        {
        //            directionOfMovement = 1;
        //            Debug.Log("Turn Around, I hit a wall LEFT");
        //        }
        //        else if (Mathf.Abs(xDistanceFromPlayer) > minimumRange)
        //        {
        //            Jump();
        //        }
        //    }

        //    RaycastHit2D leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), Vector2.down, 1f, groundLayer);
        //    Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y), 1f * Vector2.down, Color.yellow);

        //    if (leftLedge.collider == null)
        //    {
        //        if (playerInRange)
        //        {
        //            Jump();
        //        }
        //        else if (IsGrounded())
        //        {
        //            directionOfMovement = 1;
        //            Debug.Log("Turn Around, no ground ahead LEFT");
        //        }
        //    }
        //}
    }

    IEnumerator JumpWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Jump();
    }
}
