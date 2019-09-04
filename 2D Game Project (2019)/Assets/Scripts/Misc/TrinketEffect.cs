using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveTrinketEffect : MonoBehaviour
{
    public PlayerController player;
    public Trinket.TrinketTypes type;
    public float value;

    private void Update()
    {
        float duration = 30;

        if (type == Trinket.TrinketTypes.HealthRegen)
        {
            player.CurrentHealth += value / duration * Time.deltaTime;
        }
        else if (type == Trinket.TrinketTypes.StaminaRegen)
        {
            player.CurrentStamina += value / duration * Time.deltaTime;
        }
    }
}
