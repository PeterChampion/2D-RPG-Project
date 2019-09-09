using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int Damage { get; set; }
    private Vector2 knockbackDirection;
    public Vector2 KnockbackDirection { get { return knockbackDirection; } set { knockbackDirection = value; } }
    private float knockbackPower;
    public float KnockbackPower { get { return knockbackPower; } set { knockbackPower = value; } }
    private float knockbackDuration;
    public float KnockbackDuration { get { return knockbackDuration; } set { knockbackDuration = value; } }
    public LayerMask layerToIgnore;

    private List<GameObject> hitCharacters = new List<GameObject>();

    private Collider2D attachedCollider;

    private void Awake()
    {
        attachedCollider = GetComponent<Collider2D>();
        attachedCollider.enabled = false;
    }

    private void Start()
    {
        attachedCollider.enabled = true;
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;

        if (collidedObject.GetComponent<Character2D>() && collidedObject.layer != layerToIgnore && !hitCharacters.Contains(collidedObject) && collidedObject.layer != 13)
        {
            if (collidedObject.GetComponent<AI>())
            {
                if (!collidedObject.GetComponent<AI>().isDead)
                {
                    hitCharacters.Add(collidedObject);
                    Character2D characterHit = collidedObject.GetComponent<Character2D>();
                    characterHit.TakeDamage(Damage);
                    characterHit.Knockback(new Vector2(KnockbackDirection.x, 0.5f), KnockbackPower, KnockbackDuration);
                }
            }
            else
            {
                hitCharacters.Add(collidedObject);
                Character2D characterHit = collidedObject.GetComponent<Character2D>();
                characterHit.TakeDamage(Damage);
                characterHit.Knockback(new Vector2(KnockbackDirection.x, 0.5f), KnockbackPower, KnockbackDuration);
            }
        }
    }
}
