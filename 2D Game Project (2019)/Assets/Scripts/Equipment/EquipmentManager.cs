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
        slotIndex = (int)newEquipment.equipSlot;
        Equipment oldEquipment = null;

        if (currentEquipment[slotIndex] != null)
        {
            currentEquipment[slotIndex].RemoveEquipmentStats();
            oldEquipment = currentEquipment[slotIndex];
            inventory.AddToInventory(oldEquipment);
        }
        currentEquipment[slotIndex] = newEquipment;
        currentEquipment[slotIndex].ApplyEquipmentStats();

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
        Equipment oldEquipment = currentEquipment[slotIndex];

        if (inventory.maxInventorySpace > inventory.inventoryItems.Count)
        {
            if (currentEquipment[slotIndex] != null)
            {
                oldEquipment = currentEquipment[slotIndex];
                inventory.AddToInventory(oldEquipment);

                currentEquipment[slotIndex].RemoveEquipmentStats();
                currentEquipment[slotIndex] = null;

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
            currentEquipment[i].RemoveEquipmentStats();
            currentEquipment[i] = null;
        }
    }
}
