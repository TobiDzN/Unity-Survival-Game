using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    public MaterialsData item;
    public int amount = 1;

    public bool TryPickup(PlayerInventory inventory)
    {
        if (inventory == null || item == null) return false;

        bool allAdded = inventory.AddItem(item, amount);

        // if managed to import object or some of it - destroy it 
        // only if all added - destroy object
        if (allAdded)
            Destroy(gameObject);

        return allAdded;
    }
}
