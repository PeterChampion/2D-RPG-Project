using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon = null;
    public Item item;
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
        icon.sprite = item.sprite;
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
    }

    public void ClearSlot()
    {
        //Debug.Log("Clearing item from UI...");
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void BuyItem()
    {
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

        if (item != null)
        {
            if (Inventory.instance.inventoryItems.Count < Inventory.instance.maxInventorySpace)
            {
                if (Inventory.instance.gold >= item.goldValue)
                {
                    Inventory.instance.gold -= item.goldValue;
                    Inventory.instance.AddToInventory(item);
                }
                else
                {
                    HideToolTip();
                    print("Not enough gold");
                    ShowToolTip(transform.position, "Not enough gold!");
                }
            }
            else
            {
                HideToolTip();
                print("Not enough space");
                ShowToolTip(transform.position, "Inventory full!");
            }

            audioSource.Play();
        }
    }

    private void ShowToolTip(Vector2 position, string text)
    {
        tooltip.SetActive(true);
        tooltip.transform.position = position + new Vector2(-80, -60);
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
