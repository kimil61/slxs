using UnityEngine;

/// <summary>
/// WASD 이동 + 카메라 방향 기준 회전.
/// CharacterController 사용 (Rigidbody보다 3인칭 액션에 적합).
/// Player 오브젝트에 CharacterController + 이 스크립트 + PlayerInputHandler 붙일 것.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -20f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController controller;
    private PlayerInputHandler inputHandler;
    private Transform cameraTransform;

    private Vector3 velocity;
    private bool isGrounded;
    private bool movementLocked;

    public bool IsGrounded => isGrounded;
    public bool IsMoving => !movementLocked && inputHandler.MoveInput.sqrMagnitude > 0.01f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        CheckGround();
        ApplyMovement();
        ApplyGravity();
    }

    private void CheckGround()
    {
        Vector3 spherePos = transform.position + Vector3.down * (controller.height * 0.5f - groundCheckRadius + 0.05f);
        isGrounded = Physics.CheckSphere(spherePos, groundCheckRadius, groundLayer);
    }

    private void ApplyMovement()
    {
        if (movementLocked) return;

        Vector2 input = inputHandler.MoveInput;
        if (input.sqrMagnitude < 0.01f) return;

        // 카메라 기준 방향 계산
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;
        moveDir.Normalize();

        // 이동
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // 이동 방향으로 회전
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ── 전투 시스템용 이동 잠금 ──

    public void LockMovement()
    {
        movementLocked = true;
    }

    public void UnlockMovement()
    {
        movementLocked = false;
    }
}
