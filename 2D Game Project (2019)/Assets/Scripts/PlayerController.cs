using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character2D
{
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private float currentStamina = 100;
    private bool movementEnabled = true;
    private float dodgeDelay;
    private float dodgeCooldown = 1;
    public bool dodging;
    private List<Collider2D> ignoredColliders = new List<Collider2D>();
    private Vector2 movementDirection;
    private float xMovement;

    private bool recoverStamina = false;
    private Coroutine staminaCoroutine;

    protected override void Awake()
    {
        base.Awake();
        RB.gravityScale = 2;
        Physics2D.IgnoreLayerCollision(9, 11, true);
    }
    void Update()
    {
        xMovement = (Input.GetAxis("Horizontal"));
        PlayerActions();
        CheckHealth();
        CheckStamina();
        StaminaRecovery();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (movementEnabled)
        {
            Vector2 oldPosition = transform.position;

            //xMovement.Normalize();
            RB.velocity = new Vector2(xMovement * speed, RB.velocity.y);

            //float xDirection = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            //transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);
            Vector2 NewPosition = transform.position;

            if (oldPosition.x < NewPosition.x) // Moving Right
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                movementDirection = Vector2.right;
            }
            else if (oldPosition.x > NewPosition.x) // Moving Left
            {
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                movementDirection = Vector2.left;
            }
        }
        else
        {
            if (IsGrounded() && !knockedback)
            {
                movementEnabled = true;
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
        base.TakeDamage(damageValue);
        StartCoroutine(GameManager.instance.CameraShake());
        GameManager.instance.UpdateHealthUI(currentHealth);
    }

    protected override void Attack()
    {
        // Attack, plays animation + damages all targets hit
        DrainStamina(10);
        base.Attack();
    }

    private void Block()
    {
        // Block, plays animation + activates collider in front of the player that blocks incoming damage
        DrainStamina(0.5f);
    }

    private void Dodgeroll()
    {
        // Dodge, plays animation + deactives players trigger hitbox used for damage for a short interval
        DrainStamina(10);

        if (Time.time > dodgeDelay)
        {
            dodgeDelay = (Time.time + dodgeCooldown);
            StartCoroutine(DodgerollEffect(0.3f));
        }                     
    }

    private void DrainStamina(float drainValue)
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
        Physics2D.IgnoreLayerCollision(9, 10, true);
        RB.AddForce(movementDirection * 8.5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        dodging = false;
        Physics2D.IgnoreLayerCollision(9, 10, false);
        Debug.Log("End Dodge");
    }
}
