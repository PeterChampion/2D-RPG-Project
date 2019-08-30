﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player movement, actions such as attacking/dodging/etc, health and stamina (Possibly should be moved to a 'Stats' class)
public class PlayerController : Character2D
{
    // General Stats
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private float currentStamina = 100;

    // Movement / Dodge / GrapplingHook
    private bool movementEnabled = true;
    private float dodgeDelay;
    private float dodgeCooldown = 1;
    public bool dodging;
    private float xMovement;
    private float jumpDelay;
    private float jumpCooldown = 0.5f;
    [SerializeField] private LayerMask wallLayer = new LayerMask();
    public DistanceJoint2D hookJoint;
    public GameObject grapplingHook;
    private float grapplingHookDelay;
    private float grapplingHookCooldown = 0.5f;
    private Coroutine HookCoroutine;

    // Stamina Recovery
    private bool recoverStamina = false;
    private Coroutine staminaCoroutine;

    // Test
    private float stunduration = 0;
    private float stunCooldown = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        RB.gravityScale = 2;
        hookJoint = GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;
        Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore collision with items layer
        Physics2D.IgnoreLayerCollision(9, 15, true); // Ignore collision with hook layer
    }
    void Update()
    {
        if (GameManager.instance.IsGamePaused == false)
        {
            PlayerActions();
            CheckHealth();
            CheckStamina();
            StaminaRecovery();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        ClampVelocity();
    }
    
    protected override void ClampVelocity()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -8, 8), Mathf.Clamp(RB.velocity.y, -15, 15));
    }

    private void PlayerMovement()
    {
        if (knockedback || dodging)
        {
            movementEnabled = false;
        }

        if (movementEnabled)
        {
            xMovement = (Input.GetAxis("Horizontal"));

            Vector2 movement = new Vector2(xMovement * speed, 0);

            if (xMovement != 0)
            { 
                RB.AddForce(movement, ForceMode2D.Impulse);
            }
            else if (IsGrounded())
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }

            //xMovement.Normalize();
            //RB.velocity = new Vector2(xMovement * speed, RB.velocity.y);            
            //float xDirection = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);

            if (RB.velocity.x > 0) // Moving Right
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                xMovementDirection = Vector2.right;
            }
            else if (RB.velocity.x < 0) // Moving Left
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                xMovementDirection = Vector2.left;
            }
        }
        else
        {
            if (Time.time > stunduration)
            {
                stunduration = Time.time + stunCooldown;

                if (IsGrounded() && (!knockedback || !dodging))
                {
                    movementEnabled = true;
                }
            }
        }
    }

    private void PlayerActions()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !knockedback && Time.time > jumpDelay)
        {
            jumpDelay = Time.time + jumpCooldown;
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time > attackDelay)
            {
                attackDelay = Time.time + attackCooldown;
                Attack();
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Block();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !knockedback)
        {
            Dodgeroll();
        }

        if (Input.GetMouseButtonDown(2))
        { 
            if (grapplingHook.activeSelf && grapplingHook.GetComponent<Rigidbody2D>().isKinematic)
            {
                grapplingHook.SetActive(false);
                grapplingHook.GetComponent<GrapplingHook>().line.enabled = false;
                grapplingHookDelay = Time.time;
            }
            else if (Time.time > grapplingHookDelay)
            {
                grapplingHookDelay = Time.time + grapplingHookCooldown;

                if (!grapplingHook.activeSelf)
                {
                    if (HookCoroutine != null)
                    {
                        print("Stopping coroutine");
                        StopCoroutine(HookCoroutine);
                    }
                    HookCoroutine = StartCoroutine(FireHook());
                    print("Starting coroutine");
                }
            }            
        }
    }

    private IEnumerator FireHook()
    {
        print("BEGIN");
        grapplingHook.GetComponent<Rigidbody2D>().isKinematic = false;
        grapplingHook.SetActive(true);
        grapplingHook.transform.position = transform.position; // remove?        
        grapplingHook.GetComponent<SpriteRenderer>().enabled = true;
        grapplingHook.transform.parent = null;
        grapplingHook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (!IsGrounded())
        {
            grapplingHook.GetComponent<Rigidbody2D>().AddForce(new Vector2(xMovementDirection.x, 0.5f) * 20, ForceMode2D.Impulse);
        }
        else
        {
            grapplingHook.GetComponent<Rigidbody2D>().AddForce(xMovementDirection * 20, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1.5f);

        if (!grapplingHook.GetComponent<Rigidbody2D>().isKinematic)
        {
            print("END");
            grapplingHook.GetComponent<Rigidbody2D>().isKinematic = true;
            grapplingHook.transform.parent = transform;
            grapplingHook.GetComponent<SpriteRenderer>().enabled = false;
            grapplingHook.transform.position = transform.position;
            grapplingHook.SetActive(false);
        }
    }

    public override void TakeDamage(int damageValue)
    {
        damageValue -= Armour;
        if (damageValue > 0)
        {
            base.TakeDamage(damageValue);
            StartCoroutine(GameManager.instance.CameraShake());
            GameManager.instance.UpdateHealthUI(currentHealth);
        }
    }

    protected override void Attack()
    {
        // Attack, plays animation + damages all targets hit
        if (DrainStamina(10))
        {
            base.Attack();
        }
    }

    private void Block()
    {
        // Block, plays animation + activates collider in front of the player that blocks incoming damage
        if (DrainStamina(0.5f))
        {

        }
    }

    private void Dodgeroll()
    {
        // Dodge, plays animation + deactives players trigger hitbox used for damage for a short interval
        if (Time.time > dodgeDelay)
        {
            dodgeDelay = (Time.time + dodgeCooldown);

            if (DrainStamina(10))
            {
                StartCoroutine(DodgerollEffect(0.3f));
            }            
        }                          
    }

    private bool DrainStamina(float drainValue)
    {
        if ((currentStamina - drainValue) >= 0)
        {
            if (staminaCoroutine == null)
            {
                staminaCoroutine = StartCoroutine(StaminaRecoveryProcess(3));
            }
            else
            {
                StopCoroutine(staminaCoroutine);
                staminaCoroutine = StartCoroutine(StaminaRecoveryProcess(3));
            }

            currentStamina -= drainValue;
            GameManager.instance.UpdateStaminaUI((int)currentStamina);
            return true;
        }
        else
        {
            Debug.Log("Cannot perform action, not enough stamina");
            return false;
        }        
    }

    private void CheckStamina()
    {
        if (currentStamina <= 0)
        {
            currentStamina = 0;
        }

        if (currentStamina > maximumStamina)
        {
            currentStamina = maximumStamina;
        }
    }

    private void StaminaRecovery()
    {
        if (recoverStamina)
        {
            currentStamina += 0.5f;
            GameManager.instance.UpdateStaminaUI((int)currentStamina);
        }
    }

    protected override void Die()
    {
        Debug.Log("Player died");
    }

    protected override bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, groundLayer);

        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, wallLayer);

        Debug.DrawRay(transform.position, Vector2.down * jumpRaycastLength, Color.red, 0.2f);

        if (hit.collider || hit2.collider != null )
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    private IEnumerator StaminaRecoveryProcess(float delay)
    {
        recoverStamina = false;
        yield return new WaitForSeconds(delay);
        recoverStamina = true;
    }
    private IEnumerator DodgerollEffect(float duration)
    {
        dodging = true;
        Debug.Log("Start Dodge");
        gameObject.layer = 13;
        RB.AddForce(xMovementDirection * 10f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        dodging = false;
        gameObject.layer = 9;
        Debug.Log("End Dodge");
    }
}