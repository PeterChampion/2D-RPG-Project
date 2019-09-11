using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.TogglePauseState();
        GameManager.instance.pausePanel.SetActive(!GameManager.instance.pausePanel.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
