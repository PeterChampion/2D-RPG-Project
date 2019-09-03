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

    private void Start()
    {
        Destroy(gameObject, 0.1f);
    }
}
