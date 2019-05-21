using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private int maximumStamina = 100;
    [SerializeField] private int currentStamina = 100;

    private Rigidbody2D RB;
    [SerializeField] private float speed;
    [SerializeField] private float jumpStrength; 

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float xDirection = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.position = new Vector2(transform.position.x + xDirection, transform.position.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
        GameManager.instance.UpdateHealthUI(currentHealth);
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            // Needs functionality, respawn? Death screen?
            Debug.Log("Player has died");
        }
    }

    private void Attack()
    {

    }
}
