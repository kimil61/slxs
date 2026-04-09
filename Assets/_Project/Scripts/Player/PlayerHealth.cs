using UnityEngine;

/// <summary>
/// 플레이어 HP 관리. IDamageable 구현.
/// PlayerStateMachine과 같은 오브젝트에 붙일 것.
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;
    public bool IsDead => currentHP <= 0f;
    public Transform Transform => transform;

    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        currentHP = maxHP;
    }

    public void TakeDamage(float damage, AttackData attackData, Vector3 hitPoint, Transform attacker)
    {
        // 무적 중이면 무시 (구르기 i-frame)
        if (stateMachine != null && stateMachine.IsInvincible) return;
        if (IsDead) return;

        currentHP = Mathf.Max(0f, currentHP - damage);

        EventBus.Publish(new PlayerDamagedEvent
        {
            damage = damage,
            currentHP = currentHP,
            maxHP = maxHP
        });

        if (IsDead)
        {
            stateMachine?.TransitionTo(stateMachine.DeathState);
            return;
        }

        // 넉백 방향 계산 (공격자 → 플레이어)
        if (stateMachine != null && attackData != null)
        {
            Vector3 knockbackDir = (transform.position - attacker.position).normalized;
            knockbackDir.y = 0f;
            stateMachine.HitState.Setup(knockbackDir, attackData.knockbackForce);
            stateMachine.TransitionTo(stateMachine.HitState);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);

        EventBus.Publish(new PlayerDamagedEvent
        {
            damage = -amount,
            currentHP = currentHP,
            maxHP = maxHP
        });
    }

    public void ResetHP()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 런 업그레이드 등으로 최대 HP 변경 시.
    /// </summary>
    public void SetMaxHP(float newMax, bool healToFull = false)
    {
        maxHP = newMax;
        if (healToFull)
            currentHP = maxHP;
        else
            currentHP = Mathf.Min(currentHP, maxHP);
    }
}
