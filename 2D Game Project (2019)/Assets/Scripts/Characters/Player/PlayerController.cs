using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player movement, actions such as attacking/dodging/etc, health and stamina (Possibly should be moved to a 'Stats' class)
public class PlayerController : Character2D
{
    // General Stats
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private float currentStamina = 100;
    public float CurrentStamina { get { return currentStamina; } set { currentStamina = value; } }

    // Movement / Dodge / GrapplingHook
    private bool movementEnabled = true;
    private float dodgeDelay;
    private float dodgeCooldown = 1;
    public bool dodging;
    private float xMovement;
    private float jumpDelay;
    private float jumpCooldown = 0.5f;
    public DistanceJoint2D hookJoint;
    private GrapplingHook grapplingHook;
    public float grapplingHookDelay;
    private float grapplingHookCooldown = 0.5f;
    private Coroutine HookCoroutine;
    private float stunduration = 0;
    private float stunCooldown = 0.1f;
    [SerializeField] private float dashSpeed = 15;
    private bool velocityClamped = true;
    [SerializeField] private AudioClip jumpAudioClip;

    // Stamina Recovery
    private bool recoverStamina = false;
    private Coroutine staminaCoroutine;

    // Combat
    private float timeMouseHeldDown;

    // Audio
    [SerializeField] private AudioClip[] attackAudioClips;

    // Respawn
    private Vector3 respawnPoint;

    protected override void Awake()
    {
        base.Awake();
        RB.gravityScale = 2;
        hookJoint = GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;
        grapplingHook = FindObjectOfType<GrapplingHook>();
        respawnPoint = transform.position;
        Physics2D.IgnoreLayerCollision(9, 11, true); // Ignore collision with items layer
        Physics2D.IgnoreLayerCollision(9, 15, true); // Ignore collision with hook layer
    }

    void Update()
    {
        if (!GameManager.instance.IsGamePaused)
        {
            CheckHealth();
            CheckStamina();
            StaminaRecovery();

            if (!Stunned)
            {
                PlayerActions();
            }

            if (RB.velocity.y < -2f)
            {
                characterAnim.SetBool("IsFalling", true);
                characterAnim.SetBool("IsJumping", false);
            }
            else
            {
                characterAnim.SetBool("IsFalling", false);
            }

            if (grapplingHook.gameObject.GetComponent<Rigidbody2D>().isKinematic && RB.velocity.y > 4f)
            {
                characterAnim.SetBool("IsJumping", true);
            }

            if (grapplingHook.gameObject.activeSelf && Vector2.Distance(transform.position, grapplingHook.transform.position) < 2f && grapplingHook.GetComponent<Rigidbody2D>().isKinematic)
            {
                movementEnabled = false;
                RB.velocity = Vector2.zero;
                xMovement = 0;
                characterAnim.SetFloat("Speed", Mathf.Abs(xMovement));

                if (grapplingHook.transform.position.x > transform.position.x)
                {
                    transform.rotation = new Quaternion(transform.rotation.x, 0, 0, 0);
                    xMovementDirection = Vector2.right;
                }
                else
                {
                    transform.rotation = new Quaternion(transform.rotation.x, 180, 0, 0);
                    xMovementDirection = Vector2.left;
                }

                if (!IsGrounded())
                {
                    characterAnim.SetBool("IsGrappled", true);
                }
            }
            else
            {
                movementEnabled = true;
                characterAnim.SetBool("IsGrappled", false);
            }
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();

        if (velocityClamped)
        {
            ClampVelocity();
        }       
    }
    
    protected override void ClampVelocity()
    {
        RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, -8, 8), Mathf.Clamp(RB.velocity.y, -15, 15));
    }

    private void PlayerMovement()
    {
        if (Stunned || dodging)
        {
            movementEnabled = false;
        }

        if (movementEnabled)
        {
            xMovement = (Input.GetAxis("Horizontal"));
            characterAnim.SetFloat("Speed", Mathf.Abs(xMovement));

            Vector2 movement = new Vector2(xMovement * speed, 0);

            if (xMovement != 0)
            { 
                RB.AddForce(movement, ForceMode2D.Impulse);
            }
            else if (IsGrounded())
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }

            if (xMovement > 0) // Moving Right
            {
                transform.rotation = new Quaternion(transform.rotation.x, 0, 0, 0);
                xMovementDirection = Vector2.right;
            }
            else if (xMovement < 0) // Moving Left
            {
                transform.rotation = new Quaternion(transform.rotation.x, 180, 0, 0);
                xMovementDirection = Vector2.left;
            }
        }
        else
        {
            if (Time.time > stunduration)
            {
                stunduration = Time.time + stunCooldown;

                if (IsGrounded() && (!Stunned || !dodging))
                {
                    movementEnabled = true;
                }
            }
        }
    }

    private void PlayerActions()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Stunned && Time.time > jumpDelay)
        {
            jumpDelay = Time.time + jumpCooldown;
            Jump();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            timeMouseHeldDown += Time.deltaTime;            
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //print("Mouse was held down for: " + timeMouseHeldDown + " seconds");
            if (timeMouseHeldDown < 0.5f)
            {
                // Light Attack
                if (Time.time > attackDelay)
                {
                    attackDelay = Time.time + attackCooldown;
                    StandardAttack();
                }
            }
            else if (timeMouseHeldDown >= 0.5f)
            {
                // Heavy Attack
                if (Time.time > attackDelay)
                {
                    attackDelay = Time.time + attackCooldown;
                    HeavyAttack();
                }
            }
            else if (timeMouseHeldDown > 1.5f)
            {
                // Cancel
            }

            timeMouseHeldDown = 0;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
           // Block();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !Stunned)
        {
            Dodgeroll();
        }
        
        if (Input.GetMouseButtonDown(2))
        { 
            if (Time.time > grapplingHookDelay)
            {
                grapplingHookDelay = Time.time + grapplingHookCooldown;

                if (!grapplingHook.gameObject.activeSelf)
                {
                    if (HookCoroutine != null)
                    {
                        StopCoroutine(HookCoroutine);
                    }
                    HookCoroutine = StartCoroutine(FireHook());
                }
            }            
        }

        if (!Input.GetMouseButton(2))
        {
            if (grapplingHook.gameObject.activeSelf && grapplingHook.GetComponent<Rigidbody2D>().isKinematic)
            {
                grapplingHookDelay = Time.time;
                grapplingHook.ToggleActiveState();
            }
        }
    }

    private IEnumerator FireHook()
    {
        grapplingHook.ToggleActiveState();

        if (!IsGrounded())
        {
            grapplingHook.JumpShot(xMovementDirection);
        }
        else
        {
            grapplingHook.StandardShot(xMovementDirection);
        }

        yield return new WaitForSeconds(1.5f);

        grapplingHook.CollisionCheck();
    }

    public override void TakeDamage(int damageValue)
    {
        damageValue -= Armour;
        if (damageValue > 0)
        {
            base.TakeDamage(damageValue);
            StartCoroutine(GameManager.instance.CameraShake());
            GameManager.instance.UpdateHealthUI(currentHealth);
            Invoke("ToggleHurtAnimation", 0);
            Invoke("ToggleHurtAnimation", 0.25f);
        }
    }

    protected override void StandardAttack()
    {
        // Attack, plays animation + damages all targets hit
        if (DrainStamina(10))
        {
            base.StandardAttack();
            Invoke("ToggleLightAttackAnimation", 0);
            Invoke("ToggleLightAttackAnimation", 0.25f);

            int clipToPlay = Random.Range(0, attackAudioClips.Length);
            audioSource.clip = attackAudioClips[clipToPlay];
            audioSource.Play();
        }
    }

    protected override void HeavyAttack()
    {
        // Attack, plays animation + damages all targets hit
        if (DrainStamina(10))
        {
            base.HeavyAttack();
            Invoke("ToggleHeavyAttackAnimation", 0);
            Invoke("ToggleHeavyAttackAnimation", 0.25f);

            int clipToPlay = Random.Range(0, attackAudioClips.Length);
            audioSource.clip = attackAudioClips[clipToPlay];
            audioSource.Play();
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
                StartCoroutine(DodgerollEffect(0.5f));
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
        transform.position = respawnPoint;
        currentHealth = maximumHealth;
        currentStamina = maximumStamina;
    }

    protected override bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpRaycastLength, jumpableLayers);;

        Debug.DrawRay(transform.position, Vector2.down * jumpRaycastLength, Color.red, 0.2f);

        if (hit.collider != null )
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
        characterAnim.SetBool("IsRolling", true);
        dodging = true;
        Debug.Log("Start Dodge");
        gameObject.layer = 13;
        velocityClamped = false;
        //RB.AddForce(new Vector2(xMovementDirection.x * dashSpeed, 0), ForceMode2D.Impulse);
        RB.velocity = new Vector2(xMovementDirection.x * dashSpeed, 0);
        
        yield return new WaitForSeconds(duration);

        velocityClamped = true;
        dodging = false;
        gameObject.layer = 9;
        Debug.Log("End Dodge");
        characterAnim.SetBool("IsRolling", false);
    }

    protected override void Jump()
    {
        if (grapplingHook.gameObject.activeSelf && hookJoint.distance < 1f)
        {
            if (grapplingHook.gameObject.activeSelf && grapplingHook.GetComponent<Rigidbody2D>().isKinematic)
            {
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
                characterAnim.SetBool("IsJumping", true);
                grapplingHook.ToggleActiveState();
                hookJoint.enabled = false;
                grapplingHookDelay = Time.time;
                audioSource.clip = jumpAudioClip;
                audioSource.Play();
            }
        }
        else if (IsGrounded() && !grapplingHook.gameObject.activeSelf)
        {
            RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            audioSource.clip = jumpAudioClip;
            audioSource.Play();
            characterAnim.SetBool("IsJumping", true);
        }
    }

    public void UseConsumable(Consumable.ConsumableType consumableType, int value, float duration)
    {
        float health = currentHealth;
        float stamina = currentStamina;

        ConsumableEffect consumableEffect;

        switch (consumableType)
        {
            case Consumable.ConsumableType.Health:
                currentHealth += value;
                break;
            case Consumable.ConsumableType.HealthRegeneration:
                consumableEffect = gameObject.AddComponent<ConsumableEffect>();

                consumableEffect.player = this;
                consumableEffect.type = consumableType;
                consumableEffect.value = value;
                consumableEffect.duration = duration;
                break;
            case Consumable.ConsumableType.Stamina:
                currentStamina += value;
                break;
            case Consumable.ConsumableType.StaminaRegeneration:
                consumableEffect = gameObject.AddComponent<ConsumableEffect>();

                consumableEffect.player = this;
                consumableEffect.type = consumableType;
                consumableEffect.value = value;
                consumableEffect.duration = duration;
                break;
        }
    }

    private void ToggleLightAttackAnimation()
    {
        if (!characterAnim.GetBool("IsLightAttacking"))
        {
            int animationToPlay = Random.Range(1, 3);

            characterAnim.SetInteger("AttackAnimation", animationToPlay);
            characterAnim.SetBool("IsLightAttacking", true);
        }
        else
        {
            characterAnim.SetBool("IsLightAttacking", false);
        }
    }

    private void ToggleHeavyAttackAnimation()
    {
        if (!characterAnim.GetBool("IsHeavyAttacking"))
        {
            characterAnim.SetBool("IsHeavyAttacking", true);
        }
        else
        {
            characterAnim.SetBool("IsHeavyAttacking", false);
        }
    }
}
