using UnityEngine;

public class PlayerInteractCampfire : MonoBehaviour
{
    public Camera cam;
    public float range = 3f;
    public KeyCode interactKey = KeyCode.E;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!Input.GetKeyDown(interactKey)) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            var cook = hit.collider.GetComponentInParent<CampfireCooking>();
            if (cook != null)
            {
                cook.Interact();
            }
        }
    }
}
