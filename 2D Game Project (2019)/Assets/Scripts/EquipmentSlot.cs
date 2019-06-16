using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Button removeButton;
    private Equipment equipment;

    // Tooltip Info
    private GameObject tooltip;
    private TextMeshProUGUI tooltipText;
    private Vector2 tooltipOriginalPosition;

    private void Awake()
    {
        tooltip = GameObject.Find("TooltipPanel");
        tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        tooltipOriginalPosition = tooltip.transform.position;
    }

    private void Start()
    {
        tooltip.SetActive(false);
    }

    public void AddEquipment(Equipment newEquipment)
    {
        Debug.Log("Adding Equipment");
        equipment = newEquipment;
        icon.sprite = equipment.sprite;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        equipment = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        Debug.Log("Removing Equipment");
        EquipmentManager.instance.Unequip((int)equipment.equipSlot);
        HideToolTip();
    }

    public void UseEquipment()
    {
        Debug.Log("Using Equipment");
        if (equipment != null)
        {
            equipment.Use();
            HideToolTip();
        }
    }

    private void ShowToolTip(Vector2 position, string text)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position + new Vector2(60, 50);
        tooltipText.text = text;
    }

    private void HideToolTip()
    {
        tooltip.transform.position = tooltipOriginalPosition;
        tooltip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equipment != null)
        {
            ShowToolTip(transform.position, equipment.GetTooltipInfo());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideToolTip();
    }
}
