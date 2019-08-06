using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : Equipment
{
    public enum WeaponType { OneHander, TwoHander, Polearm, }
    public int damageValue;

    public override void ApplyEquipmentStats()
    {
        base.ApplyEquipmentStats();
        player.Damage += damageValue;
    }

    public override void RemoveEquipmentStats()
    {
        base.RemoveEquipmentStats();
        player.Damage -= damageValue;
    }
}
