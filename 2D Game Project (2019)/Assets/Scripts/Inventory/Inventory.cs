using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// The central class of the inventory system, handles inventory size, checking if an item can successfully be added or not and introduces a delegate for when changes to the inventory occur.
public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton
    public delegate void OnItemChanged(Item item); // Delegate
    public OnItemChanged onItemChangedCallback;
    public TextMeshProUGUI goldText;
    public int goldAmount;
    public int maxInventorySpace = 20; // Possibly change later to allow for increases based on items, allow inventory to expand/be scroll through?
    public List<Item> inventoryItems = new List<Item>();
    public int gold = 0;
    public bool open;

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

    private void Update()
    {
        goldText.text = "Gold: " + gold;
    }

    public bool AddToInventory(Item item)
    {
        //Debug.Log("Attempting to add item to inventory...");
        if (inventoryItems.Count >= maxInventorySpace)
        {
           // Debug.Log("Could not add to inventory");
            return false;
        }
        else
        {
            inventoryItems.Add(item);
            onItemChangedCallback?.Invoke(item); // '?' = a check to see if the variable is true or not, if so perform the operation 
            return true;
        }
    }
    public void RemoveFromInventory(Item item)
    {
        inventoryItems.Remove(item);
        onItemChangedCallback?.Invoke(item);
    }
}
