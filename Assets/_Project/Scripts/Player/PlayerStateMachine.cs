using UnityEngine;

/// <summary>
/// 플레이어 상태 머신. 모든 상태가 이 클래스를 통해 공유 컴포넌트에 접근.
/// Player 오브젝트 루트에 붙일 것.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerStateMachine : MonoBehaviour
{
    // ── 공유 컴포넌트 (States에서 player.XXX 로 접근) ──
    [HideInInspector] public CharacterController Controller;
    [HideInInspector] public PlayerInputHandler Input;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public PlayerHealth Health;
    [HideInInspector] public PlayerStamina Stamina;
    [HideInInspector] public WeaponHitbox WeaponHitbox;
    [HideInInspector] public CombatSoundPlayer SoundPlayer;
    [HideInInspector] public CursorManager CursorManager;
    [HideInInspector] public Transform CameraTransform;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    [Header("Combat Data")]
    public AttackData[] lightAttacks;   // 경공격 1, 2, 3 (콤보)
    public AttackData heavyAttackData;
    public DodgeData dodgeData;
    public CombatTuningData combatTuning;
    public float baseDamage = 10f;

    // ── 상태 ──
    public IPlayerState CurrentState { get; private set; }
    public IPlayerState PreviousState { get; private set; }

    // ── 공유 런타임 데이터 ──
    [HideInInspector] public Vector3 Velocity;
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool IsInvincible;
    [HideInInspector] public int CurrentComboIndex;
    public float InputBufferWindow => combatTuning != null ? combatTuning.inputBufferWindow : 0.2f;

    // ── 상태 인스턴스 (재사용) ──
    public readonly PlayerIdleState IdleState = new();
    public readonly PlayerMoveState MoveState = new();
    public readonly PlayerDodgeState DodgeState = new();
    public readonly PlayerLightAttackState LightAttackState = new();
    public readonly PlayerHeavyAttackState HeavyAttackState = new();
    public readonly PlayerHitState HitState = new();
    public readonly PlayerDeathState DeathState = new();
    public readonly PlayerFallState FallState = new();

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Input = GetComponent<PlayerInputHandler>();
        Animator = GetComponentInChildren<Animator>();
        Health = GetComponent<PlayerHealth>();
        Stamina = GetComponent<PlayerStamina>();
        WeaponHitbox = GetComponentInChildren<WeaponHitbox>();
        SoundPlayer = GetComponentInChildren<CombatSoundPlayer>();
        CursorManager = FindAnyObjectByType<CursorManager>();
    }

    private void Start()
    {
        CameraTransform = Camera.main.transform;
        TransitionTo(IdleState);
    }

    private void Update()
    {
        CheckGround();
        CurrentState?.Update(this);
        ApplyGravity();
    }

    public void TransitionTo(IPlayerState newState)
    {
        if (newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState?.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    // ── 공용 유틸리티 ──

    private void CheckGround()
    {
        IsGrounded = Controller.isGrounded;
    }

    private void ApplyGravity()
    {
        if (IsGrounded && Velocity.y < 0f)
            Velocity.y = -2f;

        Velocity.y += gravity * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }

    /// <summary>
    /// 카메라 기준 이동 방향 계산. 입력 없으면 Vector3.zero.
    /// </summary>
    public Vector3 GetCameraRelativeMoveDir()
    {
        Vector2 input = Input.MoveInput;
        if (input.sqrMagnitude < 0.01f) return Vector3.zero;

        Vector3 camForward = CameraTransform.forward;
        Vector3 camRight = CameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        return (camForward * input.y + camRight * input.x).normalized;
    }

    /// <summary>
    /// 이동 방향으로 부드러운 회전.
    /// </summary>
    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// CharacterController 수평 이동 (중력 제외).
    /// </summary>
    public void Move(Vector3 direction, float speed)
    {
        Controller.Move(direction * speed * Time.deltaTime);
    }

    public bool HasBufferedAttackInput()
    {
        return Input != null && Input.HasBufferedAttack(InputBufferWindow);
    }

    public bool ConsumeBufferedAttackInput()
    {
        return Input != null && Input.ConsumeBufferedAttack(InputBufferWindow);
    }
}
