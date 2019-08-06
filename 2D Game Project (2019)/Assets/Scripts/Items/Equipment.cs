using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
public abstract class Equipment : Item
{
    public enum EquipmentSlot { Head, Chest, Legs, Feet, Weapon, Shield, Trinket }
    public EquipmentSlot equipSlot;
    protected PlayerController player;

    public override void Use()
    {
        EquipmentManager.instance.Equip(this);
        base.Use();
    }

    public virtual void ApplyEquipmentStats()
    {
        player = FindObjectOfType<PlayerController>();
        // Intended to be overwritten
    }

    public virtual void RemoveEquipmentStats()
    {
        player = FindObjectOfType<PlayerController>();
        // Intended to be overwritten
    }
}
