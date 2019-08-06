using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Armour", menuName = "Armour")]
public class Armour : Equipment
{ 
    public int armour = 0;
    // Damage type resistance? Fire, Ice, Physical, Etc? - Maybe introduce, add in later but for now keep to armour/magic resist

    public override string GetTooltipInfo()
    {
        string tooltipInfo = itemName + "\nArmour: " + armour +"\nValue: " + goldValue;
        return tooltipInfo;
    }
    public override void ApplyEquipmentStats()
    {
        base.ApplyEquipmentStats();
        player.Armour += armour;
    }

    public override void RemoveEquipmentStats()
    {
        base.RemoveEquipmentStats();
        player.Armour -= armour;
    }
}
