using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    private GameObject equipmentPanel;
    private Transform equipmentArea;
    private EquipmentManager equipmentManager;
    private EquipmentSlot[] slots;
    private GameObject tooltip;

    private void Awake()
    {
        equipmentPanel = GameObject.Find("EquipmentPanel");
        equipmentArea = GameObject.Find("EquipmentArea").transform;
        slots = equipmentArea.GetComponentsInChildren<EquipmentSlot>();
        tooltip = GameObject.Find("TooltipPanel");
    }

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;       
        equipmentManager.onEquipmentChangedCallback += UpdateUI;
        equipmentPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && DialogueTrigger.dialogueOpen == false && !GameManager.instance.pausePanel.activeInHierarchy)
        {
            tooltip.SetActive(false);
            equipmentPanel.SetActive(!equipmentPanel.activeSelf);
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < equipmentManager.currentEquipment.Length && equipmentManager.currentEquipment[i] != null)
            {
                slots[i].AddEquipment(equipmentManager.currentEquipment[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
