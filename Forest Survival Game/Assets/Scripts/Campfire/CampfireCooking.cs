using System.Collections;
using UnityEngine;

public class CampfireCooking : MonoBehaviour
{
    public Transform meatHook;
    public GameObject rawMeatPrefab;
    public GameObject cookedMeatPrefab;
    public float cookTime = 60f;
    public PlayerVitals playerVitals;
    private GameObject currentMeat;
    private bool isCooked;
    private Coroutine cookRoutine;
    public PlayerInventory playerInventory;
    public MaterialsData meat;

    private void Start()
    {
        playerVitals = GameObject.Find("Player").GetComponent<PlayerVitals>();
        playerInventory = Object.FindFirstObjectByType<PlayerInventory>(FindObjectsInactive.Include);
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found (even including inactive).");
        }

    }

    public void Interact()
    {
        if (currentMeat == null)
        {
            if (!playerInventory) return;
            if (!playerInventory.HasItem(meat, 1)) return;

            currentMeat = Instantiate(rawMeatPrefab, meatHook.position, meatHook.rotation, meatHook);
            isCooked = false;

            playerInventory.RemoveItem(meat, 1);

            if (cookRoutine != null) StopCoroutine(cookRoutine);
            cookRoutine = StartCoroutine(CookRoutine());
            return;
        }

        if (isCooked)
        {
            Destroy(currentMeat);
            currentMeat = null;
            isCooked = false;

            if (cookRoutine != null) StopCoroutine(cookRoutine);
            cookRoutine = null;
            playerVitals.hunger += 50;
            Debug.Log("Took cooked meat!");
        }
        else
        {
            Debug.Log("Still cooking...");
        }
    }

    IEnumerator CookRoutine()
    {
        yield return new WaitForSeconds(cookTime);

        if (currentMeat == null) yield break;

        Destroy(currentMeat);
        currentMeat = Instantiate(cookedMeatPrefab, meatHook.position, meatHook.rotation, meatHook);
        isCooked = true;

        Debug.Log("Meat cooked!");
    }
}
