using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 상태 머신. NavMeshAgent 기반 이동 + FSM.
/// 적 프리팹 루트에 붙일 것. NavMeshAgent, EnemyHealth 필수.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : MonoBehaviour
{
    // ── 공유 컴포넌트 ──
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public EnemyHealth Health;
    [HideInInspector] public WeaponHitbox WeaponHitbox;
    [HideInInspector] public CombatSoundPlayer SoundPlayer;

    [Header("Data")]
    public EnemyData data;

    [Header("Patrol Waypoints (선택)")]
    public Transform[] waypoints;

    // ── 런타임 ──
    public Transform Target { get; private set; }
    public IEnemyState CurrentState { get; private set; }
    public float AttackCooldownTimer { get; set; }
    public int CurrentWaypointIndex { get; set; }

    // ── 상태 인스턴스 ──
    public readonly EnemyIdleState IdleState = new();
    public readonly EnemyPatrolState PatrolState = new();
    public readonly EnemyChaseState ChaseState = new();
    public readonly EnemyAttackState AttackState = new();
    public readonly EnemyStaggerState StaggerState = new();
    public readonly EnemyDeathState DeathState = new();

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        Health = GetComponent<EnemyHealth>();
        WeaponHitbox = GetComponentInChildren<WeaponHitbox>();
        SoundPlayer = GetComponentInChildren<CombatSoundPlayer>();
    }

    private void Start()
    {
        Agent.speed = data.moveSpeed;
        TransitionTo(waypoints.Length > 0 ? (IEnemyState)PatrolState : IdleState);
    }

    private void Update()
    {
        if (Health != null && Health.IsDead) return;

        DetectPlayer();

        if (AttackCooldownTimer > 0f)
            AttackCooldownTimer -= Time.deltaTime;

        CurrentState?.Update(this);
    }

    public void TransitionTo(IEnemyState newState)
    {
        if (newState == CurrentState) return;
        CurrentState?.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    /// <summary>
    /// OverlapSphere로 플레이어 감지.
    /// </summary>
    private void DetectPlayer()
    {
        if (Target != null)
        {
            // 타겟이 너무 멀면 해제
            float dist = Vector3.Distance(transform.position, Target.position);
            if (dist > data.loseTargetRange)
                Target = null;
            return;
        }

        var colliders = Physics.OverlapSphere(transform.position, data.detectRange);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                Target = col.transform;
                return;
            }
        }
    }

    public float DistanceToTarget()
    {
        if (Target == null) return float.MaxValue;
        return Vector3.Distance(transform.position, Target.position);
    }

    public void LookAtTarget()
    {
        if (Target == null) return;
        Vector3 dir = (Target.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    /// <summary>
    /// 랜덤 공격 패턴 선택.
    /// </summary>
    public AttackData GetRandomAttack()
    {
        if (data.attackPatterns == null || data.attackPatterns.Length == 0) return null;
        return data.attackPatterns[Random.Range(0, data.attackPatterns.Length)];
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectRange);

        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }
}
