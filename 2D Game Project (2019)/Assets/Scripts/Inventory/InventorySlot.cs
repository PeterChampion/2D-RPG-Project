using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Runs on each individual slot within the inventory, stores the information of the item stored within the slot if one is present, allows for adding/removing/clearing an item to and from the inventory.
public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon = null;
    [SerializeField] private Button removeButton = null;
    private Item item;
    private AudioSource audioSource;

    // Tooltip Info
    private GameObject tooltip;
    private TextMeshProUGUI tooltipText;
    private Vector2 tooltipOriginalPosition;

    private void Awake()
    {
        tooltip = GameObject.Find("TooltipPanel");
        tooltipText = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        tooltipOriginalPosition = tooltip.transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        tooltip.SetActive(false);
    }

    private void Update()
    {
        if (tooltip.activeSelf)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.GetComponent<RectTransform>());
        }
    }

    public void AddItem(Item newItem)
    {
        //Debug.Log("Adding item to UI...");
        item = newItem;
        icon.sprite = item.sprite;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
       //Debug.Log("Clearing item from UI...");
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        //Debug.Log("Removing item from inventory...");
        HideToolTip();
        if (item != null)
        {
            if (GameManager.instance.shopWindow.activeSelf)
            {                
                Inventory.instance.gold += item.goldValue / 2;
                // Add item to shop?
            }
            Inventory.instance.RemoveFromInventory(item);
            audioSource.Play();
        }        
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            audioSource.Play();
            if (IsMouseOver())
            {
                if (item != null)
                {
                    ShowToolTip(transform.position, item.GetTooltipInfo());
                }
                else
                {
                    HideToolTip();
                }
            }            
        }
    }

    private void ShowToolTip(Vector2 position, string text)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position + new Vector2(-80,-60);
        tooltipText.text = text;
    }

    private void HideToolTip()
    {
        tooltip.transform.position = tooltipOriginalPosition;
        tooltip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ShowToolTip(transform.position, item.GetTooltipInfo());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideToolTip();
    }

    private bool IsMouseOver()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
