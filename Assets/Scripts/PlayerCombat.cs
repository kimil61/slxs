using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 전투 입력 처리: 약공격(좌클릭 탭), 강공격(좌클릭 홀드), 구르기(스페이스).
/// Player 오브젝트에 붙일 것. PlayerMovement 필수.
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Heavy Attack")]
    [SerializeField] private float heavyAttackHoldTime = 0.4f;

    [Header("Action Durations (애니메이션 길이에 맞춰 조절)")]
    [SerializeField] private float slashDuration = 0.6f;
    [SerializeField] private float heavyAttackDuration = 1.2f;
    [SerializeField] private float rollDuration = 0.8f;

    private Animator animator;
    private PlayerMovement playerMovement;
    private CursorManager cursorManager;

    private bool isAttacking;
    private bool isHoldingAttack;
    private float holdTimer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        cursorManager = FindAnyObjectByType<CursorManager>();
    }

    private void Update()
    {
        // 커서 풀려있으면 전투 입력 무시
        if (cursorManager != null && !cursorManager.IsCursorLocked) return;

        Mouse mouse = Mouse.current;
        Keyboard keyboard = Keyboard.current;
        if (mouse == null || keyboard == null) return;

        // ── 구르기 (스페이스) ──
        if (keyboard.spaceKey.wasPressedThisFrame && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Roll");
            playerMovement.LockMovement();
            Invoke(nameof(EndAction), rollDuration);
        }

        // ── 좌클릭 누름 시작 ──
        if (mouse.leftButton.wasPressedThisFrame && !isAttacking)
        {
            isHoldingAttack = true;
            holdTimer = 0f;
        }

        // ── 누르고 있는 중 ──
        if (isHoldingAttack)
        {
            holdTimer += Time.deltaTime;
        }

        // ── 좌클릭 뗌 ──
        if (mouse.leftButton.wasReleasedThisFrame && isHoldingAttack)
        {
            isHoldingAttack = false;

            if (holdTimer >= heavyAttackHoldTime)
            {
                // 강공격
                isAttacking = true;
                animator.SetTrigger("HeavyAttack");
                playerMovement.LockMovement();
                Invoke(nameof(EndAction), heavyAttackDuration);
            }
            else
            {
                // 약공격
                isAttacking = true;
                animator.SetTrigger("Attack");
                playerMovement.LockMovement();
                Invoke(nameof(EndAction), slashDuration);
            }
        }
    }

    private void EndAction()
    {
        isAttacking = false;
        playerMovement.UnlockMovement();
    }
}
