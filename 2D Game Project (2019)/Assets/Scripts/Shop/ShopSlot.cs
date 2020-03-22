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
    [SerializeField] private TextMeshProUGUI quantity;
    [SerializeField] private TextMeshProUGUI outOfStock;
    [SerializeField] private Button button;

    public int quantityInStock = 0;
    public bool unlimitedQuantity;

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
        outOfStock.enabled = false;
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

        if (quantityInStock <= 0 && !unlimitedQuantity)
        {
            outOfStock.enabled = true;
            button.interactable = false;

            //ClearSlot();
            Debug.Log("Clear");
        }
        else if (item != null)
        {
            outOfStock.enabled = false;
            icon.sprite = item.sprite;
            button.interactable = true;
        }

        if (!unlimitedQuantity)
        {
            quantity.text = quantityInStock.ToString();
        }
        else
        {
            quantity.text = string.Empty;
        }

        if (item == null)
        {
            //ShopUI.instance.shopSlots.Remove(this);
            //Destroy(gameObject);

            ClearSlot();
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void AddItem(Item newItem)
    {
        //Debug.Log("Adding item to UI...");
        item = newItem;
        icon.sprite = item.sprite;
        icon.enabled = true;
        button.interactable = true;
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
                    quantityInStock--;
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

    private void ItemSold(Item itemReceived)
    {
        if (itemReceived == item)
        {
            if (!unlimitedQuantity)
            {
                quantityInStock++;
            }
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
