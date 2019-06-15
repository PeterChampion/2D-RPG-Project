using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton
    public delegate void OnItemChanged(Item item); // Delegate
    public OnItemChanged onItemChangedCallback;
    public TextMeshProUGUI goldText;
    public int goldAmount;
    [SerializeField] private int maxInventorySpace = 20;
    public List<Item> inventoryItems = new List<Item>();

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

    public bool AddToInventory(Item item)
    {
        if (inventoryItems.Count >= maxInventorySpace)
        {
            return false;
        }
        else
        {
            inventoryItems.Add(item);
            if (onItemChangedCallback != null)
            {
                onItemChangedCallback(item);
            }
            return true;
        }
    }
    public void RemoveFromInventory(Item item)
    {
        inventoryItems.Remove(item);
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback(item);
        }
    }
}
