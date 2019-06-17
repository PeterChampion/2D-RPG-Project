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
        EquipmentManager.instance.Equip(this);
        base.Use();
    }

    public virtual void ApplyEquipmentStats()
    {
        // Intended to be overwritten
    }

    public virtual void RemoveEquipmentStats()
    {
        // Intended to be overwritten
    }
}
