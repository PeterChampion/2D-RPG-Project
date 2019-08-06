using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player movement, actions such as attacking/dodging/etc, health and stamina (Possibly should be moved to a 'Stats' class)
public class PlayerController : Character2D
{
    // General Stats
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private float currentStamina = 100;

    // Movement + Dodge
    private bool movementEnabled = true;
    private float dodgeDelay;
    private float dodgeCooldown = 1;
    public bool dodging;
    private List<Collider2D> ignoredColliders = new List<Collider2D>();
    private float xMovement;
    [SerializeField] private LayerMask wallLayer = new LayerMask();

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
        Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore collision with items layer
    }
    void Update()
    {
        if (GameManager.instance.IsGamePaused == false)
        {
            xMovement = (Input.GetAxis("Horizontal"));
            PlayerActions();
            CheckHealth();
            CheckStamina();
            StaminaRecovery();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (knockedback || dodging)
        {
            movementEnabled = false;
        }

        if (movementEnabled)
        {
            //xMovement.Normalize();
            RB.velocity = new Vector2(xMovement * speed, RB.velocity.y);
            //float xDirection = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);

            if (RB.velocity.x > 0) // Moving Right
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                movementDirection = Vector2.right;
            }
            else if (RB.velocity.x < 0) // Moving Left
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                movementDirection = Vector2.left;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time > attackDelay)
            {
                attackDelay = (Time.time + attackCooldown);
                Attack();
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Block();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Dodgeroll();
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
        Physics2D.IgnoreLayerCollision(13, 10, true);
        RB.AddForce(movementDirection * 10f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        dodging = false;
        Physics2D.IgnoreLayerCollision(13, 10, false);
        gameObject.layer = 9;
        Debug.Log("End Dodge");
    }
}
