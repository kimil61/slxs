using UnityEngine;

/// <summary>
/// Animator 파라미터를 PlayerStateMachine 상태와 자동 동기화.
/// States에서 직접 Animator를 건드리는 것 외에, 공통 파라미터를 여기서 매 프레임 갱신.
/// Player 오브젝트 (Animator가 있는 자식 or 본인)에 붙일 것.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler input;

    // Animator 파라미터 해시 (문자열 비교 회피)
    private static readonly int HashMoveX = Animator.StringToHash("MoveX");
    private static readonly int HashMoveZ = Animator.StringToHash("MoveZ");
    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsGrounded = Animator.StringToHash("isGrounded");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (animator == null || stateMachine == null) return;

        // 이동 블렌드 (MoveX, MoveZ — Blend Tree용)
        Vector2 moveInput = input.MoveInput;
        animator.SetFloat(HashMoveX, moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat(HashMoveZ, moveInput.y, 0.1f, Time.deltaTime);

        // 속도 (States에서도 설정하지만, 여기서 보간 처리)
        float targetSpeed = moveInput.sqrMagnitude > 0.01f ? moveInput.magnitude : 0f;
        animator.SetFloat(HashSpeed, targetSpeed, 0.1f, Time.deltaTime);

        // 지면 체크
        animator.SetBool(HashIsGrounded, stateMachine.IsGrounded);
    }
}
