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
    private Slider experienceSlider;
    private TextMeshProUGUI equipmentStatsText;
    public PlayerController player;
    private GameObject playerStatsPanel;
    private TextMeshProUGUI playerStatsText;
    private GameObject playerStatsButtons;

    // Location UI
    private TextMeshProUGUI locationNameText;
    private Coroutine locationNameFadeCoroutine;

    // QuestLog UI
    public GameObject questLog;

    // Shop UI
    public GameObject shopWindow;

    // Camera Shake
    private Camera gameCamera;
    [SerializeField] private float shakeDuration = 0.3f; // How long the shake effect lasts
    [SerializeField] private float shakeAmplitude = 1f;
    [SerializeField] private float shakeFrequency = 2f;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // AI Delegates
    public delegate void OnCharacterDeath(AI.EnemyType type);
    public OnCharacterDeath OnCharacterDeathCallback;

    // Location Delegates
    public delegate void OnLocationEntered(string name);
    public OnLocationEntered OnLocationEnteredCallback;

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
        experienceSlider = GameObject.Find("ExperienceSlider").GetComponent<Slider>();
        equipmentStatsText = GameObject.Find("EquipmentStatsText").GetComponent<TextMeshProUGUI>();
        pausePanel = GameObject.Find("PausePanel");
        pausePanel.SetActive(false);
        locationNameText = GameObject.Find("LocationNameText").GetComponent<TextMeshProUGUI>();
        locationNameText.enabled = false;

        // Delegate assignments
        EquipmentManager.instance.onEquipmentChangedCallback += UpdateEquipmentStatsUI;
        player.OnLevelUpCallback += UpdatePlayerStatsUI;
        player.OnLevelUpCallback += SetExperienceSliderMinValue;
        player.OnLevelUpCallback += UpdateEquipmentStatsUI;
        player.OnExperienceGainCallback += UpdatePlayerStatsUI;
        OnLocationEnteredCallback += DisplayLocationName;

        playerStatsPanel = GameObject.Find("StatsPanel");
        playerStatsText = GameObject.Find("StatsText").GetComponent<TextMeshProUGUI>();
        playerStatsButtons = GameObject.Find("StatsButtons");
        questLog = GameObject.Find("QuestLog");
        shopWindow = GameObject.Find("ShopPanel");

        shopWindow.SetActive(false);
        playerStatsPanel.SetActive(false);
        questLog.SetActive(false);
        //StartCoroutine(InputListener());
        UpdateEquipmentStatsUI();
        UpdatePlayerStatsUI();
    }

    private void Update()
    {
        healthSlider.maxValue = player.MaximumHealth;
        staminaSlider.maxValue = player.MaximumStamina;
        experienceSlider.maxValue = player.NextLevelExperience;

        healthSlider.value = player.CurrentHealth;
        staminaSlider.value = player.CurrentStamina;
        experienceSlider.value = player.Experience;

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !Inventory.instance.open && !shopWindow.activeSelf)
        {
            TogglePauseState();
            pausePanel.SetActive(!pausePanel.activeSelf);

            if (questLog.activeSelf)
            {
                questLog.SetActive(!questLog.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !questLog.activeSelf && !pausePanel.activeSelf && !shopWindow.activeSelf)
        {
            playerStatsPanel.SetActive(!playerStatsPanel.activeSelf);
        }

        if (player.LevelPoints > 0)
        {
            playerStatsButtons.SetActive(true);
        }
        else
        {
            playerStatsButtons.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Q) && !Inventory.instance.open && !IsGamePaused)
        {
            questLog.SetActive(!questLog.activeSelf);
        }
    }

    private void UpdateEquipmentStatsUI()
    {
        equipmentStatsText.text = "Damage: " + player.Damage +"\nArmour: " + player.Armour;
    }

    public void UpdatePlayerStatsUI()
    {
        playerStatsText.text = "Level: " + player.Level + "\nExperience: " + player.Experience + "/" + player.NextLevelExperience + "\nHealth: " + (int)player.CurrentHealth + "/" + player.MaximumHealth
            + "\nStamina: " + Mathf.RoundToInt(player.CurrentStamina) + "/" + player.MaximumStamina + "\n-----------------------------------" + "\nStrength: " + player.Strength + "\nConstitution: " 
            + player.Constitution + "\nAgility: " + player.Agility + "\nLuck: " + player.Luck + "\nPoints Left: " + player.LevelPoints;
    }

    private void SetExperienceSliderMinValue()
    {
        experienceSlider.minValue = player.Experience - (player.Experience / 10);
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

    private void DisplayLocationName(string locationName)
    {
        locationNameText.text = locationName;

        if (locationNameFadeCoroutine != null)
        {
            StopCoroutine(locationNameFadeCoroutine);
            locationNameFadeCoroutine = StartCoroutine(FadeTextEffect(locationNameText, 0.5f));
        }
        else
        {
            locationNameFadeCoroutine = StartCoroutine(FadeTextEffect(locationNameText, 0.5f));
        }
    }

    private IEnumerator FadeTextEffect(TextMeshProUGUI textElement, float duration)
    {
        textElement.enabled = true;
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, 0);

        while (textElement.color.a < 1f)
        {
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, textElement.color.a + (Time.deltaTime * 1));
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        while (textElement.color.a > 0f)
        {
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, textElement.color.a - (Time.deltaTime * 1));
            yield return null;
        }

        textElement.enabled = false;
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
