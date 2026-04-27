using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory size")]
    [SerializeField] private int slotCount = 49;

    [Header("Items")]
    public List<MaterialsData> items = new List<MaterialsData>();
    public List<int> amounts = new List<int>();

    public event Action OnInventoryChanged;

    private void Awake()
    {
        EnsureSize(slotCount);
    }

    private void EnsureSize(int size)
    {
        while (items.Count < size) items.Add(null);
        while (amounts.Count < size) amounts.Add(0);
    }

    public bool AddItem(MaterialsData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        EnsureSize(slotCount);

        int remaining = amount;

        // 1) fill current stacks
        for (int i = 0; i < slotCount && remaining > 0; i++)
        {
            if (items[i] == item && amounts[i] < item.maxStack)
            {
                int space = item.maxStack - amounts[i];
                int add = Mathf.Min(space, remaining);
                amounts[i] += add;
                remaining -= add;
            }
        }

        // 2) create new slots
        for (int i = 0; i < slotCount && remaining > 0; i++)
        {
            if (items[i] == null)
            {
                int add = Mathf.Min(item.maxStack, remaining);
                items[i] = item;
                amounts[i] = add;
                remaining -= add;
            }
        }

        bool addedAnything = remaining != amount;
        if (addedAnything)
            OnInventoryChanged?.Invoke();

        // no slots available
        return remaining == 0;
    }

    public bool RemoveItem(MaterialsData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        int remaining = amount;

        // remove from existing stacks
        for (int i = 0; i < slotCount && remaining > 0; i++)
        {
            if (items[i] == item && amounts[i] > 0)
            {
                int remove = Mathf.Min(amounts[i], remaining);
                amounts[i] -= remove;
                remaining -= remove;

                // clear slot if empty
                if (amounts[i] <= 0)
                {
                    items[i] = null;
                    amounts[i] = 0;
                }
            }
        }

        bool removedAnything = remaining != amount;
        if (removedAnything)
            OnInventoryChanged?.Invoke();

        // return true only if fully removed
        return remaining == 0;
    }

    public bool HasItem(MaterialsData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        int total = 0;

        int count = Mathf.Min(items.Count, amounts.Count);

        for (int i = 0; i < count; i++)
        {
            if (items[i] == item)
            {
                total += amounts[i];
                if (total >= amount)
                    return true;
            }
        }

        return false;
    }



}
