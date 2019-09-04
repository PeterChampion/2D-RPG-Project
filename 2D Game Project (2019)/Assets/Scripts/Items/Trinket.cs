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
