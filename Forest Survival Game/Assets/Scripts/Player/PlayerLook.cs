using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public float xRotation;
    public float yRotation;
    PlayerController controls;
    InputAction lookAction;
    public Camera cam;

    private void Awake()
    {
        controls = new PlayerController();
        lookAction = controls.Player.Look;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mouseInput = lookAction.ReadValue<Vector2>();
        float mouseX = mouseInput.x * mouseSens * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSens * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.Rotate(0, mouseX, 0);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

}
