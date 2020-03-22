using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void Resume()
    {
        GameManager.instance.TogglePauseState();
        GameManager.instance.pausePanel.SetActive(!GameManager.instance.pausePanel.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void IncreaseStrength()
    {
        player.Strength++;
        player.LevelPoints--;
        player.OnLevelUpCallback.Invoke();
        player.Damage = 5 + player.Strength;
        GameManager.instance.player.OnLevelUpCallback.Invoke();
    }

    public void IncreaseConstitution()
    {
        player.Constitution++;
        player.LevelPoints--;
        player.OnLevelUpCallback.Invoke();
        player.MaximumHealth = 90 + (player.Constitution * 10);
        GameManager.instance.player.OnLevelUpCallback.Invoke();
    }

    public void IncreaseAgility()
    {
        player.Agility++;
        player.LevelPoints--;
        player.OnLevelUpCallback.Invoke();
        player.MaximumStamina = 90 + (player.Agility * 10);
        GameManager.instance.player.OnLevelUpCallback.Invoke();
    }

    public void IncreaseLuck()
    {
        player.Luck++;
        player.LevelPoints--;
        player.OnLevelUpCallback.Invoke();
        GameManager.instance.player.OnLevelUpCallback.Invoke();
    }
}
