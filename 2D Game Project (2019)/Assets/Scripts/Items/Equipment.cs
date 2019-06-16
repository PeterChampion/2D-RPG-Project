using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
public class Equipment : Item
{
    public enum EquipmentSlot { Head, Chest, Legs, Feet, Weapon, Shield, Trinket }
    public EquipmentSlot equipSlot;

    public override void Use()
    {
        // Needs to equip item to player
        EquipmentManager.instance.Equip(this);
        base.Use();
    }
}
