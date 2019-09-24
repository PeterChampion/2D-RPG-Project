using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// TBD
public abstract class AI : Character2D
{    
    public enum EnemyType { Bat, Goblin, Kobold, Ogre }
    [Header("Character Details")]
    public EnemyType Type;
    [Range(0,1000)] [SerializeField] private int experienceValue;
    protected GameObject player;
    [Range(0,100)] [SerializeField] protected int detectionRange = 5;
    private int detectionRangeDoubled;
    [Range(0, 10)] [SerializeField] protected float minimumRange = 2;
    protected float xDistanceFromPlayer;
    protected float yDistanceFromPlayer;

    [Header("Jump Settings")]
    [SerializeField] protected Vector2 rayCastOffset = new Vector2(1,0);
    [Range(0, 5)] [SerializeField] protected float jumpCooldown = 1.5f;
    [Range(0, 3)] protected float jumpDelay = 0;
    protected bool playerInRange = false;
    protected int directionOfMovement = -1;
    [SerializeField] protected LayerMask wallLayer = new LayerMask();

    [Header("Patrol Settings")]   
    [SerializeField] protected bool setToPatrol = false;
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    private int patrolpointIndex = 0;
    private Transform healthBar;
    private Coroutine healthBarCoroutine;

    [Header("Loot Settings")]
    [SerializeField] private GameObject lootDropPrefab;
    [SerializeField] private Item.RarityTier rarityOfLootDrop;
    [Range(0, 100)] [SerializeField] private int lootDropChance = 0;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>().gameObject;
        detectionRangeDoubled = detectionRange * 2;
        healthBar = transform.Find("Healthbar");
        healthBar.localScale = new Vector3(currentHealth / 100, healthBar.localScale.y, healthBar.localScale.z);
        healthBar.GetComponent<SpriteRenderer>().enabled = false;
        Physics2D.IgnoreLayerCollision(10, 10, true); // Ignore collisions with other enemies
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            AIMovement();
        }

        CheckHealth();

        if (!Stunned)
        {
            ClampVelocity();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!Stunned)
        {
            if (directionOfMovement == 1) // Right
            {              
                xMovementDirection = Vector2.right;
                transform.rotation = new Quaternion(transform.rotation.x, 180, 0, 0);
            }
            else if (directionOfMovement == -1) // Left
            {               
                xMovementDirection = Vector2.left;
                transform.rotation = new Quaternion(transform.rotation.x, 0, 0, 0);
            }

            if (Mathf.Abs(xDistanceFromPlayer) < minimumRange && Mathf.Abs(yDistanceFromPlayer) < minimumRange + 3 && IsGrounded())
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }
            else
            {
                RB.AddForce(new Vector2(xMovementDirection.x * speed, 0), ForceMode2D.Impulse);
            }
        }       
    }

    protected virtual void AIMovement()
    {
        xDistanceFromPlayer = player.transform.position.x - transform.position.x; // Calculate distance on X axis
        yDistanceFromPlayer = player.transform.position.y - transform.position.y; // Calculate distance on Y axis

        // If the player is within the detection range...
        if (Mathf.Abs(xDistanceFromPlayer) < detectionRange && Mathf.Abs(yDistanceFromPlayer) < detectionRange)
        {
            playerInRange = true;
            detectionRange = detectionRangeDoubled;
        }
        else // Otherwise if the player is not...
        {
            detectionRange = detectionRangeDoubled / 2;
            playerInRange = false;
            if (setToPatrol)
            {
                Patrol();
            }
        }

        if (playerInRange)
        {
            if (xDistanceFromPlayer < 0)
            {
                directionOfMovement = -1; // Player is on the left so set directionOfMovement to match
            }
            else
            {
                directionOfMovement = 1; // Player is on the right so set directionOfMovement to match
            }
        }
    }

    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);

        if (currentHealth <= 0)
        {
            healthBar.localScale = new Vector3(0, healthBar.localScale.y, healthBar.localScale.z);
        }
        else
        {
            healthBar.localScale = new Vector3(currentHealth / 100, healthBar.localScale.y, healthBar.localScale.z);
            GameObject damagePopup = Instantiate(popupText, transform.position, Quaternion.identity);
            damagePopup.GetComponent<PopupText>().content.text = damageValue.ToString();
            damagePopup.GetComponent<PopupText>().content.color = Color.red;
            damagePopup.transform.SetParent(null);
        }

        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }
        healthBarCoroutine = StartCoroutine(DisplayHealthBar(3));
    }

    protected override void Die()
    {
        isDead = true;
        GameManager.instance.OnCharacterDeathCallback?.Invoke(Type);
        GameManager.instance.player.Experience += experienceValue;
        GameManager.instance.player.OnExperienceGainCallback();

        GameObject experiencePopup = Instantiate(popupText, transform.position, Quaternion.identity);
        experiencePopup.GetComponent<PopupText>().content.text = experienceValue.ToString() + "xp";
        experiencePopup.GetComponent<PopupText>().content.color = Color.yellow;
        experiencePopup.transform.SetParent(null);

        RandomLootDrop();
        StopAllCoroutines();
        base.Die();
    }

    protected override void Jump()
    {
        if (Time.time > jumpDelay)
        {
            if (IsGrounded())
            {
                jumpDelay = Time.time + jumpCooldown;
                RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
    }

    private void Patrol()
    {
        // If player is not in range, and we are set to patrol, then we patrol between points.
        // Look at current index of list
        // Compare x positions to determine direction to travel to
        // Once difference between x positions is negligible, increment index
        // If index would exceed List, wrap back to 0.
        // Repeat

        if (patrolPoints.Count > 0)
        {
            float xDistanceFromPoint = patrolPoints[patrolpointIndex].position.x - transform.position.x;

            if (Mathf.Abs(xDistanceFromPoint) < 1)
            {
                if (patrolpointIndex + 1 > patrolPoints.Count - 1)
                {
                    patrolpointIndex = 0;
                }
                else
                {
                    patrolpointIndex++;
                }
            }

            if (xDistanceFromPoint < 0)
            {
                directionOfMovement = -1; // Point is on the left so set directionOfMovement to match
            }
            else
            {
                directionOfMovement = 1; // Point is on the right so set directionOfMovement to match
            }
        }        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ApplyDirectionalForce(Vector2 direction, float force)
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(direction * (force / 2), ForceMode2D.Impulse);
    }

    private void RandomLootDrop()
    {
        if (lootDropChance + GameManager.instance.player.Luck >= Random.Range(1, 101))
        {
            int itemToDrop;
            GameObject lootDrop = Instantiate(lootDropPrefab, transform.position, Quaternion.identity);
            lootDrop.transform.parent = null;

            switch (rarityOfLootDrop)
            {
                case Item.RarityTier.Common:
                    itemToDrop = Random.Range(0, Item.commonItems.Count - 1);
                    lootDrop.GetComponent<ItemPickUp>().item = Item.commonItems[itemToDrop];
                    break;
                case Item.RarityTier.Uncommon:
                    itemToDrop = Random.Range(0, Item.uncommonItems.Count - 1);
                    lootDrop.GetComponent<ItemPickUp>().item = Item.uncommonItems[itemToDrop];
                    break;
                case Item.RarityTier.Rare:
                    itemToDrop = Random.Range(0, Item.rareItems.Count - 1);
                    lootDrop.GetComponent<ItemPickUp>().item = Item.rareItems[itemToDrop];
                    break;
                case Item.RarityTier.VeryRare:
                    itemToDrop = Random.Range(0, Item.veryRareItems.Count - 1);
                    lootDrop.GetComponent<ItemPickUp>().item = Item.veryRareItems[itemToDrop];
                    break;
                case Item.RarityTier.Legendary:
                    itemToDrop = Random.Range(0, Item.legendaryItems.Count - 1);
                    lootDrop.GetComponent<ItemPickUp>().item = Item.legendaryItems[itemToDrop];
                    break;
            }
        }        
    }

    private IEnumerator DisplayHealthBar(float duration)
    {
        SpriteRenderer healthbarSpriterender = healthBar.GetComponent<SpriteRenderer>();
        healthbarSpriterender.enabled = true;

        yield return new WaitForSeconds(duration);

        healthbarSpriterender.enabled = false;
        healthBarCoroutine = null;
    }
}
