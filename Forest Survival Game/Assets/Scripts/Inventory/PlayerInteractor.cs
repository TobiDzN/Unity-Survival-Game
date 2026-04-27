using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    private WorldItemPickup currentPickup;
    public MonoBehaviour playerlook;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPickup != null)
        {
            currentPickup.TryPickup(inventory);
        }
        if (Input.GetKeyDown(KeyCode.I))
            {
            playerlook.enabled = !playerlook.enabled;
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<WorldItemPickup>();
        if (pickup != null)
            currentPickup = pickup;
    }

    private void OnTriggerExit(Collider other)
    {
        var pickup = other.GetComponent<WorldItemPickup>();
        if (pickup != null && pickup == currentPickup)
            currentPickup = null;
    }

}
