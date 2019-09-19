using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : Interactable
{
    private GameObject inventoryPanel;
    private GameObject tooltip;

    protected override void Awake()
    {
        tooltip = GameObject.Find("TooltipPanel");
        inventoryPanel = GameObject.Find("InventoryPanel");
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if ((Mathf.Abs(transform.position.x - player.transform.position.x) > interactionRange || Mathf.Abs(transform.position.y - player.transform.position.y) > interactionRange) && GameManager.instance.shopWindow.activeSelf)
        {
            GameManager.instance.shopWindow.SetActive(false);
            inventoryPanel.SetActive(false);
            tooltip.SetActive(false);
            GameManager.instance.TogglePauseState();

        }
    }

    protected override void Effect()
    {
        GameManager.instance.shopWindow.SetActive(!GameManager.instance.shopWindow.activeSelf);
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        GameManager.instance.TogglePauseState();

        if (tooltip.activeSelf)
        {
            tooltip.SetActive(false);
        }

        base.Effect();
    }
}
