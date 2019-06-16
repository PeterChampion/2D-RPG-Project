using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;
    [SerializeField] public Equipment[] currentEquipment;
    public delegate void OnEquipmentChanged();
    public OnEquipmentChanged onEquipmentChangedCallback;
    public delegate void OnEquipmentUIChanged();
    public OnEquipmentUIChanged onEquipmentUIChangedCallback;
    Inventory inventory;
    private int slotIndex;

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

    private void Start()
    {
        inventory = Inventory.instance;

        int NumSlots = System.Enum.GetNames(typeof(Equipment.EquipmentSlot)).Length;
        currentEquipment = new Equipment[NumSlots];
    }

    public void Equip(Equipment newEquipment)
    {
        Debug.Log("Attempting to Equip an item");
        slotIndex = (int)newEquipment.equipSlot;
        Equipment oldEquipment = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldEquipment = currentEquipment[slotIndex];
            inventory.AddToInventory(oldEquipment);
            Debug.Log("Old equipped item returned to inventory!");
        }
        currentEquipment[slotIndex] = newEquipment;
        Debug.Log("Item Equipped!");

        if (onEquipmentChangedCallback != null)
        {
            onEquipmentChangedCallback.Invoke();
        }

        if (onEquipmentUIChangedCallback != null)
        {
            onEquipmentUIChangedCallback.Invoke();
        }
    }

    public void Unequip(int slotIndex)
    {
        Debug.Log("Attempting to Unequip an item");
        Equipment oldEquipment = currentEquipment[slotIndex];

        if (inventory.maxInventorySpace > inventory.inventoryItems.Count)
        {
            if (currentEquipment[slotIndex] != null)
            {
                oldEquipment = currentEquipment[slotIndex];
                inventory.AddToInventory(oldEquipment);

                currentEquipment[slotIndex] = null;
                Debug.Log("Attempt to Unequip an item was successful");

                if (onEquipmentChangedCallback != null)
                {
                    onEquipmentChangedCallback.Invoke();
                }

                if (onEquipmentUIChangedCallback != null)
                {
                    onEquipmentUIChangedCallback.Invoke();
                }
            }
        }
        else
        {
            // Could not be done, display error information to player and/or console
            Debug.Log("Unequiping an item failed");
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    public void ResetEquipment()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            currentEquipment[i] = null;
        }
    }
}
