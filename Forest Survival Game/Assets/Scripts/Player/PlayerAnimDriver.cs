using UnityEngine;

public class PlayerAnimDriver : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Animator anim;

    [Header("Smoothing")]
    [SerializeField] private float dampTime = 0.1f;

    int moveXHash, moveZHash, groundedHash, jumpHash, pickupHash, punchHash;


    private void Awake()
    {
        if (movement == null) movement = GetComponent<PlayerMovement>();

        moveXHash = Animator.StringToHash("MoveX");
        moveZHash = Animator.StringToHash("MoveZ");
        groundedHash = Animator.StringToHash("Grounded");
        jumpHash = Animator.StringToHash("Jump");
        pickupHash = Animator.StringToHash("Pickup");
        punchHash = Animator.StringToHash("Punch");
    }

    private void Update()
    {
        if (movement == null || anim == null) return;

        Vector2 input = movement.MoveInput;

        float x = input.x;
        float z = input.y;

        if (input.sqrMagnitude < 0.01f)
        {
            x = 0f;
            z = 0f;
        }

        anim.SetFloat(moveXHash, x, dampTime, Time.deltaTime);
        anim.SetFloat(moveZHash, z, dampTime, Time.deltaTime);
        anim.SetFloat("YVel", movement.YVel);
        anim.SetBool(groundedHash, movement.IsGrounded);
        if (movement.JumpPressedThisFrame)
        {
            anim.SetTrigger(jumpHash);
        }
        anim.speed = movement.IsSprinting ? 1.15f : 1f;
    }
    public void PlayPickup()
    {
        if (anim == null) return;

        anim.SetTrigger(pickupHash);
    }

    public void TriggerPunch()
    {
        if (anim == null) return;

        anim.SetTrigger(punchHash);
    }


}
