using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Trinket", menuName = "Trinket")]
public class Trinket : Equipment
{
    // Provides passives such as damage increases, armour increases, speed increase, etc general buffs
    public enum TrinketTypes { Damage, Armour, FlatHealth, FlatStamina, HealthRegen, StaminaRegen  }; // Need to add
    public TrinketTypes type;
    public float value;

    public override string GetTooltipInfo()
    {
        string tooltipInfo = string.Empty;

        switch (type)
        {
            case TrinketTypes.Damage:
                tooltipInfo = itemName + " Increases damage stat by " + value;
                break;
            case TrinketTypes.Armour:
                tooltipInfo = itemName + " Increases armour stat by " + value;
                break;
            case TrinketTypes.FlatHealth:
                tooltipInfo = itemName + " Increases maximum health by " + value;
                break;
            case TrinketTypes.FlatStamina:
                tooltipInfo = itemName + " Increases maximum stamina by " + value;
                break;
            case TrinketTypes.HealthRegen:
                tooltipInfo = itemName + " Gain health regeneration of " + value / 20 + " per second";
                break;
            case TrinketTypes.StaminaRegen:
                tooltipInfo = itemName + " Gain  stamina regeneration of " + value / 20 + " per second";
                break;
        }
        tooltipInfo = tooltipInfo + "\nValue: " + goldValue;

        return tooltipInfo;
    }

    public override void ApplyEquipmentStats()
    {
        base.ApplyEquipmentStats();

        if (type == TrinketTypes.HealthRegen || type == TrinketTypes.StaminaRegen)
        {
            PassiveTrinketEffect trinketEffect = player.gameObject.AddComponent<PassiveTrinketEffect>();
            trinketEffect.player = player;
            trinketEffect.type = type;
            trinketEffect.value = value;
        }
        else
        {
            switch (type)
            {
                case TrinketTypes.Damage:
                    player.Damage += (int)value;
                    break;
                case TrinketTypes.Armour:
                    player.Armour += (int)value;
                    break;
                case TrinketTypes.FlatHealth:
                    player.MaximumHealth += value;
                    break;
                case TrinketTypes.FlatStamina:
                    player.MaximumHealth += value;
                    break;
            }
        }        
    }

    public override void RemoveEquipmentStats()
    {
        base.RemoveEquipmentStats();
        if (type == TrinketTypes.HealthRegen || type == TrinketTypes.StaminaRegen)
        {
            Destroy(player.gameObject.GetComponent<PassiveTrinketEffect>());
        }
        else
        {
            switch (type)
            {
                case TrinketTypes.Damage:
                    player.Damage -= (int)value;
                    break;
                case TrinketTypes.Armour:
                    player.Armour -= (int)value;
                    break;
                case TrinketTypes.FlatHealth:
                    player.MaximumHealth -= value;
                    break;
                case TrinketTypes.FlatStamina:
                    player.MaximumHealth -= value;
                    break;
            }
        }
    }
}
