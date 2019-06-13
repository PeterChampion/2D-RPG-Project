using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private float currentStamina = 100;

    private bool movementEnabled = true;
    private bool knockedBack = false;

    private GameObject attackArea;

    private float dodgeDelay;
    private float dodgeCooldown = 1;
    public bool dodging;
    private List<Collider2D> ignoredColliders = new List<Collider2D>();

    private Rigidbody2D RB;
    [SerializeField] private float speed = 8;
    [SerializeField] private float jumpStrength = 12;
    [SerializeField] private float jumpRaycastLength = 2;
    [SerializeField] LayerMask groundLayer;
    private bool grounded;
    private Vector2 movementDirection;

    private bool recoverStamina = false;
    private Coroutine staminaCoroutine;

    // Tempoary Variables
    [SerializeField] private LayerMask EnemyLayer;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = 2;

        attackArea = GameObject.Find("AttackArea");
        attackArea.SetActive(false);
    }
    void Update()
    {
        PlayerMovement();
        PlayerActions();
        CheckHealth();
        CheckStamina();
        StaminaRecovery();
    }

    private void PlayerMovement()
    {
        if (movementEnabled)
        {
            Vector3 oldPosition = transform.position;

            float xDirection = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);

            Vector3 NewPosition = transform.position;

            if (oldPosition.x < NewPosition.x)
            {
                // Moving right
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, 0);
                movementDirection = Vector2.right;
            }
            else if (oldPosition.x > NewPosition.x)
            {
                // Moving left
                transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 180, 0);
                movementDirection = Vector2.left;
            }
        }
        else
        {
            if (IsGrounded() && !knockedBack)
            {
                movementEnabled = true;
            }
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * jumpRaycastLength, Color.red, 5f);

        if (hit.collider != null)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    private void PlayerActions()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
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

    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
        StartCoroutine(GameManager.instance.CameraShake());
        GameManager.instance.UpdateHealthUI(currentHealth);
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            // Needs functionality, respawn? Death screen?
            Debug.Log("Player has died");
        }

        if (currentHealth >= maximumHealth)
        {
            currentHealth = maximumHealth;
        }
    }

    private void Attack()
    {
        // Attack, plays animation + damages all targets hit
        DrainStamina(10);
        StartCoroutine(Attack(0.1f));
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

    public void Knockback(Vector2 knockbackDirection, float knockbackStrength, float movementLockoutDuration)
    {
        StartCoroutine(KnockBackEffect(knockbackDirection, knockbackStrength, movementLockoutDuration));
    }

    private IEnumerator StaminaRecoveryProcess(float delay)
    {
        recoverStamina = false;
        yield return new WaitForSeconds(delay);
        recoverStamina = true;
    }

    public IEnumerator KnockBackEffect(Vector2 knockbackDirection, float knockbackStrength, float movementLockoutDuration)
    {
        movementEnabled = false;
        knockedBack = true;
        RB.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        Debug.Log("Knockback force applied!");
        yield return new WaitForSeconds(movementLockoutDuration);
        knockedBack = false;
    }

    private IEnumerator Attack(float duration)
    {
        attackArea.GetComponent<AttackArea>().affectedTargets.Clear();
        attackArea.SetActive(true);
        yield return new WaitForSeconds(duration);
        attackArea.SetActive(false);
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
