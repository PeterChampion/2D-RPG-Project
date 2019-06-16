using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Armour", menuName = "Armour")]
public class Armour : Equipment
{ 
    public int armourValue = 0;
    public int magicResist = 0;
    // Damage type resistance? Fire, Ice, Physical, Etc? - Maybe introduce, add in later but for now keep to armour/magic resist

    public override string GetTooltipInfo()
    {
        string tooltipInfo = itemName + "\nArmour: " + armourValue + "\nMagic Resist: " + magicResist + "\nValue: " + goldValue;
        return tooltipInfo;
    }
}
