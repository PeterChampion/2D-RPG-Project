using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// The central class of the inventory system, handles inventory size, checking if an item can successfully be added or not and introduces a delegate for when changes to the inventory occur.
public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton
    public delegate void OnItemChanged(); // Delegate
    public OnItemChanged onItemChangedCallback;
    public TextMeshProUGUI goldText;
    public int goldAmount;
    public int maxInventorySpace = 20;
    public List<Item> inventoryItems = new List<Item>();
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
            onItemChangedCallback?.Invoke(); // '?' = a check to see if the variable is true or not, if so perform the operation 
            //Debug.Log("Item added to inventory!");
            return true;
        }
    }
    public void RemoveFromInventory(Item item)
    {
        //Debug.Log("Attempting to remove item from inventory...");
        inventoryItems.Remove(item);
        onItemChangedCallback?.Invoke();
    }
}
