using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

// Central gamemanger class, handles camera behaviour, UI elements & pausing the gamestate
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Game State
    public bool IsGamePaused = false;
    public GameObject pausePanel;

    // Player Stats
    private Slider healthSlider;
    private Slider staminaSlider;
    private TextMeshProUGUI statsText;
    public PlayerController player;

    // Camera Shake
    private Camera gameCamera;
    [SerializeField] private float shakeDuration = 0.3f; // How long the shake effect lasts
    [SerializeField] private float shakeAmplitude = 1f;
    [SerializeField] private float shakeFrequency = 2f;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

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
        player = FindObjectOfType<PlayerController>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        gameCamera = FindObjectOfType<Camera>();
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
        statsText = GameObject.Find("StatsText").GetComponent<TextMeshProUGUI>();
        pausePanel = GameObject.Find("PausePanel");
        pausePanel.SetActive(false);
        EquipmentManager.instance.onEquipmentChangedCallback += UpdatePlayerStatsUI;

        StartCoroutine(InputListener());
        UpdatePlayerStatsUI();
        Canvas.ForceUpdateCanvases();
    }

    private void Update()
    {
        healthSlider.value = player.CurrentHealth;
        staminaSlider.value = player.CurrentStamina;

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !Inventory.instance.open)
        {
            TogglePauseState();
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    private void UpdatePlayerStatsUI()
    {
        statsText.text = "Damage: " + player.Damage.ToString() +"\nArmour: " + player.Armour.ToString();
    }

    public void UpdateHealthUI(float healthValue)
    {
        healthSlider.value = healthValue;
    }

    public void UpdateStaminaUI(float staminaValue)
    {
        staminaSlider.value = staminaValue;
    }

    public void TogglePauseState()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            IsGamePaused = true;
        }
        else
        {
            Time.timeScale = 1;
            IsGamePaused = false;
        }
    }

    public IEnumerator CameraShake()
    {
        virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = shakeFrequency;
        yield return new WaitForSeconds(shakeDuration);
        virtualCameraNoise.m_AmplitudeGain = 0f;
    }


    // TEST VARIABLES AND METHODS BELOW, FREE TO REMOVE

    private float doubleClickTimeLimit = 0.25f;

    private IEnumerator InputListener()
    {
        while (enabled)
        { //Run as long as this is active

            if (Input.GetMouseButtonDown(0))
                yield return ClickEvent();

            yield return null;
        }
    }

    private IEnumerator ClickEvent()
    {
        //pause a frame so you don't pick up the same mouse down event.
        yield return new WaitForEndOfFrame();

        float count = 0f;
        while (count < doubleClickTimeLimit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print("Double Click");
                yield break;
            }
            count += Time.deltaTime;// increment counter by change in time between frames
            yield return null; // wait for the next frame
        }        
        print("Single click");
    }
}
