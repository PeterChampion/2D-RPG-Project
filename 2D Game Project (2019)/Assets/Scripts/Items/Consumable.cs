using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TBD
[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class Consumable : Item
{
    public int consumableStrength = 0; // Such as the amount of HP a consumable may give
    public int consumableDuration = 0; // Length of the effect, 0 = instant
}
