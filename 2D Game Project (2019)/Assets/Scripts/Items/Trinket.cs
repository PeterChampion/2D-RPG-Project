using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Trinket", menuName = "Trinket")]
public class Trinket : Equipment
{
    // Provides passives such as damage increases, armour increases, speed increase, etc general buffs
    public enum TrinketTypes { Damage, Armour, MagicResist, Speed, FlatHealth, FlatStamina, HealthRegen, StaminaRegen  }; // Need to add
}
