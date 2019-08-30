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

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        RB = GetComponent<Rigidbody2D>();
        line.enabled = false;
        Physics2D.IgnoreLayerCollision(15, 11, true); // Ignore collision with items layer
        Physics2D.IgnoreLayerCollision(15, 15, true); // Ignore collision with hook layer
    }

    private void Update()
    {
        if (RB.isKinematic)
        {
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1, transform.position);            

            if (player.hookJoint.distance > 1)
            {
                player.hookJoint.distance -= 0.1f;
            }
            else
            {
                //line.enabled = false;
                //player.joint.enabled = false;
            }
        }
    }

    public void FireHook(Vector2 direction)
    {
        RB.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hook landed!");
        RB.isKinematic = true;
        line.enabled = true;
        RB.velocity = Vector2.zero;
        player.hookJoint.enabled = true;
        player.hookJoint.connectedBody = RB;
        player.hookJoint.distance = Vector2.Distance(transform.position, player.transform.position);

        line.SetPosition(0, player.transform.position);
        line.SetPosition(1, transform.position);
    }    
}
