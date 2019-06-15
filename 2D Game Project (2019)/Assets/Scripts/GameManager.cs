using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool IsGamePaused = false;

    private Slider healthSlider;
    private Slider staminaSlider;
    private Camera gameCamera;

    // Camera Shake
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
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        gameCamera = FindObjectOfType<Camera>();
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
    }

    public void UpdateHealthUI(int healthValue)
    {
        healthSlider.value = healthValue;
    }

    public void UpdateStaminaUI(int staminaValue)
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
}
