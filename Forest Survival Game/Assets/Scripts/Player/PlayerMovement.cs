using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public bool isSprinting = false;
    public CharacterController cc;

    InputAction moveAction;
    InputAction sprintAction;
    PlayerController controls;
    PlayerAnimDriver animDriver;

    [Header("Gravity")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public bool isGrounded = false;

    public float gravity = -9.81f;
    Vector3 fallVelocity;

    [Header("Jump")]
    public float jumpHeight = 3;

    private void Awake()
    {
        controls = new PlayerController();

        //PickUp
        animDriver = GetComponent<PlayerAnimDriver>();
        controls.Player.PickUp.performed += OnPickup;

        //Move
        moveAction = controls.Player.Move;
        sprintAction = controls.Player.Sprint;

        //Sprint
        sprintAction.performed += _ => isSprinting = true;
        sprintAction.canceled += _ => isSprinting = false;

        //Jump
        controls.Player.Jump.performed += OnJump;

        //PrimaryAction
        controls.Player.PrimaryAction.performed += OnPrimary;
    }


    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public float YVel => fallVelocity.y;

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Movement();
        Gravity();
    }

    void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && fallVelocity.y < 0)
        {
            fallVelocity.y = -2;
        }
        fallVelocity.y += gravity * Time.deltaTime;
        cc.Move(fallVelocity * Time.deltaTime);
    }

    public Vector2 MoveInput { get; private set; }


    void Movement()
    {
        MoveInput = moveAction.ReadValue<Vector2>();

        float currentSpeed = isSprinting ? runSpeed : moveSpeed;

        Vector3 move = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        cc.Move(currentSpeed * Time.deltaTime * move);
    }

    public bool JumpPressedThisFrame { get; private set; }

    void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        fallVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        JumpPressedThisFrame = true;
    }
    private void LateUpdate()
    {
        JumpPressedThisFrame = false;
    }

    private void OnPickup(InputAction.CallbackContext ctx)
    {
        animDriver?.PlayPickup();
    }

    void OnPrimary(InputAction.CallbackContext ctx)
    {
        //Here We need to check what the player is holding to trigger the right animation for now only punch
        animDriver?.TriggerPunch();
    }


}
