using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class Consumable : Item
{
    public enum ConsumableType { Health, HealthRegeneration, Stamina, StaminaRegeneration}
    public ConsumableType type;
    public int consumableValue = 0; // Such as the amount of HP a consumable may give
    public float consumableDuration = 0; // Length of the effect, 0 = instant
    
    public override void Use()
    {
        GameManager.instance.player.UseConsumable(type, consumableValue, consumableDuration);
        base.Use();
    }

    public override string GetTooltipInfo()
    {
        string tooltipInfo = itemName + "\nRestores " + consumableValue + " " + type;

        if (consumableDuration != 0)
        {
            tooltipInfo = itemName + "\nRestores " + consumableValue;

            if (type == ConsumableType.HealthRegeneration)
            {
                tooltipInfo = tooltipInfo + " Health\nover " + consumableDuration + " seconds";
            }
            else
            {
                tooltipInfo = tooltipInfo + " Stamina\nover " + consumableDuration + " seconds";
            }
        }

        tooltipInfo = tooltipInfo + "\nValue: " + goldValue;

        return tooltipInfo;
    }
}
