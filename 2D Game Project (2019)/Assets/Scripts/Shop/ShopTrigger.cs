using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : Interactable
{
    protected override void Update()
    {
        base.Update();

        if (Mathf.Abs(transform.position.x - player.transform.position.x) > interactionRange || Mathf.Abs(transform.position.y - player.transform.position.y) > interactionRange)
        {
            GameManager.instance.shopWindow.SetActive(false);
        }
    }

    protected override void Effect()
    {
        GameManager.instance.shopWindow.SetActive(!GameManager.instance.shopWindow.activeSelf);
        base.Effect();
    }
}
