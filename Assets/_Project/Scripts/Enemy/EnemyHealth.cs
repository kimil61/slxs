using UnityEngine;

/// <summary>
/// 적 HP 관리. IDamageable 구현.
/// 피격 시 Stagger 전환, 사망 시 Death 전환 + 드롭 + 이벤트 발행.
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageable
{
    private EnemyStateMachine stateMachine;
    private float currentHP;

    public float CurrentHP => currentHP;
    public float MaxHP => stateMachine.data.maxHP;
    public bool IsDead => currentHP <= 0f;
    public Transform Transform => transform;

    private void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        currentHP = stateMachine.data.maxHP;
    }

    public void TakeDamage(float damage, AttackData attackData, Vector3 hitPoint, Transform attacker)
    {
        if (IsDead) return;

        currentHP = Mathf.Max(0f, currentHP - damage);

        if (IsDead)
        {
            stateMachine.TransitionTo(stateMachine.DeathState);
            return;
        }

        // 경직 전환
        stateMachine.StaggerState.Setup(attacker, attackData);
        stateMachine.TransitionTo(stateMachine.StaggerState);
    }
}
