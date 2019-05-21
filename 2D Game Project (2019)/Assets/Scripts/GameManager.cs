using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Slider healthSlider;
    private Slider staminaSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {

        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
    }
    void Update()
    {
        
    }

    public void UpdateHealthUI(int healthValue)
    {
        healthSlider.value = healthValue;
    }

    public void UpdateStaminaUI(int staminaValue)
    {
        staminaSlider.value = staminaValue;
    }
}
