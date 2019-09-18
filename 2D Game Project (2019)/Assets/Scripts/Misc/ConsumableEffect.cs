using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableEffect : MonoBehaviour
{
    public PlayerController player;
    public Consumable.ConsumableType type;
    public int value = 0; // Such as the amount of HP a consumable may give
    public float duration = 0; // Length of the effect, 0 = instant

    private void Start()
    {
        Destroy(this, duration);
    }

    private void Update()
    {
        switch (type)
        {
            case Consumable.ConsumableType.HealthRegeneration:
                player.CurrentHealth += value / duration * Time.deltaTime;
                break;
            case Consumable.ConsumableType.StaminaRegeneration:
                player.CurrentStamina += value / duration * Time.deltaTime;
                break;
        }
        GameManager.instance.UpdatePlayerStatsUI();
    }
}
