using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] private float force = 5;
    [SerializeField] private Vector2 direction;
    private Rigidbody2D RB;
    [SerializeField] public LineRenderer line;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        RB = GetComponent<Rigidbody2D>();
        line.enabled = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.IgnoreLayerCollision(15, 11, true); // Ignore collision with items layer
        Physics2D.IgnoreLayerCollision(15, 15, true); // Ignore collision with hook layer
    }

    private void Start()
    {
        gameObject.SetActive(false);
        line.startColor = Color.grey;
        line.endColor = Color.grey;
    }

    private void LateUpdate()
    {
        line.SetPosition(0, player.transform.position);
        line.SetPosition(1, transform.position);

        if (RB.isKinematic)
        {        
            if (player.hookJoint.distance > 1)
            {
                player.hookJoint.distance -= 0.2f;
            }
            else
            {
                //line.enabled = false;
                //player.joint.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        // If enemy layer...
        if (collidedObject.layer == 10)
        {
            print("Enemy hit!");
            collidedObject.GetComponent<AI>().ApplyStunEffect(1.5f);
            collidedObject.GetComponent<AI>().ApplyDirectionalForce(-direction, force * 2);
            //collidedObject.GetComponent<Rigidbody2D>().AddForce(-direction * force, ForceMode2D.Impulse);
            ToggleActiveState();
        }
        else
        {
            RB.isKinematic = true;
            RB.velocity = Vector2.zero;
            player.hookJoint.enabled = true;
            player.hookJoint.connectedBody = RB;
            player.hookJoint.distance = Vector2.Distance(transform.position, player.transform.position);
        }
    }

    public void ToggleActiveState()
    {
        if (gameObject.activeSelf)
        {
            line.enabled = false;
            RB.isKinematic = true;
            spriteRenderer.enabled = false;
            transform.position = (Vector2)player.transform.position;
            player.hookJoint.enabled = false;
            gameObject.SetActive(false);            
        }
        else
        {
            line.enabled = true;
            RB.isKinematic = false;
            spriteRenderer.enabled = true;
            transform.position = (Vector2)player.transform.position;
            RB.velocity = Vector2.zero;
            gameObject.SetActive(true);
        }
    }

    public void JumpShot(Vector2 xDirection)
    {
        direction = xDirection;
        RB.AddForce(new Vector2(xDirection.x, 0.5f) * 20, ForceMode2D.Impulse);

        if (xDirection == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -45);
            print("Rotated right");
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 45);
            print("Rotated left");
        }
    }

    public void StandardShot(Vector2 xDirection)
    {
        if (xDirection == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            print("Rotated right");
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            print("Rotated left");
        }

        direction = xDirection;
        RB.AddForce(xDirection * 20, ForceMode2D.Impulse);
    }

    public void CollisionCheck()
    {
        if (!RB.isKinematic)
        {
            ToggleActiveState();
        }
    }
}
