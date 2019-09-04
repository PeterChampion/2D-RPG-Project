using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class Consumable : Item
{
    public enum ConsumableType { Health, HealthRegen, Stamina, StaminaRegen}
    public ConsumableType type;
    public int consumableValue = 0; // Such as the amount of HP a consumable may give
    public int consumableDuration = 0; // Length of the effect, 0 = instant
    
    public override void Use()
    {
        switch (type)
        {
            case ConsumableType.Health:
                // Add health instantly
                GameManager.instance.player.CurrentHealth += consumableValue;
                break;
            case ConsumableType.HealthRegen:
                // Add health over a period of time
                break;
            case ConsumableType.Stamina:
                // Add stamina instantly
                GameManager.instance.player.CurrentStamina += consumableValue;
                break;
            case ConsumableType.StaminaRegen:
                // Add stamina over a period of time
                break;
        }

        base.Use();
    }
}
